// <copyright file="BankStatementUploads.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.IO;
using System.Text;
using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides functionality for uploading bank statements to the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This class handles the upload of bank statement files in various formats (OFX, QIF, CSV)
/// to automatically import bank transactions into FreeAgent. The uploaded statements are
/// parsed by the FreeAgent API and converted into bank transactions that can be explained
/// and categorized.
/// </para>
/// <para>
/// The class supports multiple input methods:
/// <list type="bullet">
/// <item><description>Base64-encoded string data</description></item>
/// <item><description>File system paths</description></item>
/// <item><description>Byte arrays</description></item>
/// <item><description>Streams</description></item>
/// <item><description>CSV content as strings</description></item>
/// </list>
/// </para>
/// <para>
/// After successfully uploading a statement, the bank transaction cache is automatically
/// cleared to ensure subsequent queries retrieve the latest imported transactions.
/// </para>
/// </remarks>
/// <seealso cref="BankTransactions"/>
/// <seealso cref="BankTransactionExplanations"/>
public class BankStatementUploads
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="BankStatementUploads"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent client instance for API communication.</param>
    /// <param name="cache">The memory cache for managing cached bank transaction data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public BankStatementUploads(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Uploads a bank statement file to the FreeAgent API in OFX, QIF, or CSV format.
    /// </summary>
    /// <param name="bankAccountUrl">The URL of the bank account to associate the statement with.</param>
    /// <param name="statementData">The statement file content as a base64-encoded string.</param>
    /// <param name="fileType">The file type: "ofx", "qif", or "csv" (case-insensitive).</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the import summary with details about imported transactions.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="statementData"/> is null, empty, or whitespace, or when <paramref name="fileType"/> is not one of the supported formats.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized or the upload fails.</exception>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails.</exception>
    /// <remarks>
    /// This method posts the statement to the FreeAgent API endpoint /v2/bank_transactions/statement.
    /// The statement data should be base64-encoded before passing it to this method. After a successful
    /// upload, the bank transaction cache is automatically cleared to ensure fresh data on subsequent queries.
    /// The API will parse the statement and create bank transaction records that can then be explained using
    /// the <see cref="BankTransactionExplanations"/> API.
    /// </remarks>
    public async Task<BankStatementUploadResponse> UploadStatementAsync(Uri bankAccountUrl, string statementData, string fileType)
    {
        await this.client.InitializeAndAuthorizeAsync();

        if (string.IsNullOrWhiteSpace(statementData))
        {
            throw new ArgumentException("Statement data cannot be empty", nameof(statementData));
        }

        string normalizedFileType = fileType.ToLowerInvariant();
        if (normalizedFileType is not "ofx" and not "qif" and not "csv")
        {
            throw new ArgumentException("File type must be 'ofx', 'qif', or 'csv'", nameof(fileType));
        }

        BankStatementUpload upload = new()
        {
            BankAccount = bankAccountUrl,
            Statement = statementData,
            FileType = normalizedFileType
        };

        BankStatementUploadRoot root = new() { Statement = upload };
        string json = JsonSerializer.Serialize(root, SharedJsonOptions.Instance);

        HttpResponseMessage response = await this.client.HttpClient.PostAsync(
            new Uri(this.client.ApiBaseUrl, "/v2/bank_transactions/statement"),
            new StringContent(json, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        BankStatementUploadResponseRoot? result = await response.Content.ReadFromJsonAsync<BankStatementUploadResponseRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Clear bank transactions cache since we've imported new ones
        this.ClearBankTransactionCache();

        return result?.ImportSummary ?? throw new InvalidOperationException("Failed to upload bank statement");
    }

    /// <summary>
    /// Uploads a bank statement from a file path on the local file system.
    /// </summary>
    /// <param name="bankAccountUrl">The URL of the bank account to associate the statement with.</param>
    /// <param name="filePath">The full path to the statement file. Must have an .ofx, .qif, or .csv extension.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the import summary with details about imported transactions.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the file specified by <paramref name="filePath"/> does not exist.</exception>
    /// <exception cref="ArgumentException">Thrown when the file does not have a supported extension (.ofx, .qif, or .csv).</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized or the upload fails.</exception>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails.</exception>
    /// <remarks>
    /// This method reads the file content, converts it to a base64-encoded string, and calls
    /// <see cref="UploadStatementAsync"/> to perform the upload. The file type is automatically
    /// determined from the file extension.
    /// </remarks>
    public async Task<BankStatementUploadResponse> UploadStatementFromFileAsync(Uri bankAccountUrl, string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Statement file not found", filePath);
        }

        string extension = Path.GetExtension(filePath).ToLowerInvariant().TrimStart('.');
        if (extension is not "ofx" and not "qif" and not "csv")
        {
            throw new ArgumentException("File must have .ofx, .qif, or .csv extension", nameof(filePath));
        }

        byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
        string base64Content = Convert.ToBase64String(fileBytes);

        return await UploadStatementAsync(bankAccountUrl, base64Content, extension);
    }

    /// <summary>
    /// Uploads a bank statement from a byte array.
    /// </summary>
    /// <param name="bankAccountUrl">The URL of the bank account to associate the statement with.</param>
    /// <param name="statementBytes">The statement file content as a byte array.</param>
    /// <param name="fileType">The file type: "ofx", "qif", or "csv" (case-insensitive).</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the import summary with details about imported transactions.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="statementBytes"/> is null or empty, or when <paramref name="fileType"/> is not one of the supported formats.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized or the upload fails.</exception>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails.</exception>
    /// <remarks>
    /// This method converts the byte array to a base64-encoded string and calls
    /// <see cref="UploadStatementAsync"/> to perform the upload. This is useful when you have
    /// statement data in memory or received from a network source.
    /// </remarks>
    public async Task<BankStatementUploadResponse> UploadStatementFromBytesAsync(Uri bankAccountUrl, byte[] statementBytes, string fileType)
    {
        if (statementBytes == null || statementBytes.Length == 0)
        {
            throw new ArgumentException("Statement bytes cannot be empty", nameof(statementBytes));
        }

        string base64Content = Convert.ToBase64String(statementBytes);
        return await UploadStatementAsync(bankAccountUrl, base64Content, fileType);
    }

    /// <summary>
    /// Uploads a bank statement from a stream.
    /// </summary>
    /// <param name="bankAccountUrl">The URL of the bank account to associate the statement with.</param>
    /// <param name="statementStream">The statement file content as a readable stream.</param>
    /// <param name="fileType">The file type: "ofx", "qif", or "csv" (case-insensitive).</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the import summary with details about imported transactions.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="statementStream"/> is null or not readable, or when <paramref name="fileType"/> is not one of the supported formats.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized or the upload fails.</exception>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails.</exception>
    /// <remarks>
    /// This method reads the stream into memory, converts it to a byte array, and calls
    /// <see cref="UploadStatementFromBytesAsync"/> to perform the upload. This is useful when
    /// working with file uploads from web forms or other streaming sources.
    /// </remarks>
    public async Task<BankStatementUploadResponse> UploadStatementFromStreamAsync(Uri bankAccountUrl, Stream statementStream, string fileType)
    {
        if (statementStream == null || !statementStream.CanRead)
        {
            throw new ArgumentException("Statement stream must be readable", nameof(statementStream));
        }

        using MemoryStream memoryStream = new();
        await statementStream.CopyToAsync(memoryStream);
        byte[] bytes = memoryStream.ToArray();

        return await UploadStatementFromBytesAsync(bankAccountUrl, bytes, fileType);
    }

    /// <summary>
    /// Uploads a CSV bank statement from string content.
    /// </summary>
    /// <param name="bankAccountUrl">The URL of the bank account to associate the statement with.</param>
    /// <param name="csvContent">The CSV content as a string with transaction data.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the import summary with details about imported transactions.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="csvContent"/> is null, empty, or whitespace.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized or the upload fails.</exception>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails.</exception>
    /// <remarks>
    /// This convenience method is specifically for CSV format statements. It converts the string
    /// content to UTF-8 bytes, encodes it as base64, and calls <see cref="UploadStatementAsync"/>
    /// with the file type set to "csv". This is useful when you're generating CSV content
    /// programmatically or have received it as text.
    /// </remarks>
    public async Task<BankStatementUploadResponse> UploadCsvStatementAsync(Uri bankAccountUrl, string csvContent)
    {
        if (string.IsNullOrWhiteSpace(csvContent))
        {
            throw new ArgumentException("CSV content cannot be empty", nameof(csvContent));
        }

        byte[] bytes = Encoding.UTF8.GetBytes(csvContent);
        string base64Content = Convert.ToBase64String(bytes);

        return await UploadStatementAsync(bankAccountUrl, base64Content, "csv");
    }

    /// <summary>
    /// Clears the bank transaction cache to ensure fresh data is retrieved after importing new transactions.
    /// </summary>
    /// <remarks>
    /// This method removes various cached bank transaction queries including all transactions,
    /// unexplained transactions, explained transactions, imported transactions, and manually entered transactions.
    /// This ensures that subsequent API calls will fetch the latest data from the server including
    /// the newly imported transactions.
    /// </remarks>
    private void ClearBankTransactionCache()
    {
        // Clear any cached bank transaction data since we've imported new transactions
        // This ensures subsequent queries will fetch the latest data
        string[] cacheKeys =
        [
            "bank_transactions_all",
            "bank_transactions_unexplained",
            "bank_transactions_explained",
            "bank_transactions_imported",
            "bank_transactions_manual"
        ];

        foreach (string key in cacheKeys)
        {
            this.cache.Remove(key);
        }
    }
}