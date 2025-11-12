// <copyright file="BankStatementUploads.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.IO;
using System.Text;
using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class BankStatementUploads
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    public BankStatementUploads(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Upload a bank statement file (OFX, QIF, or CSV format)
    /// </summary>
    /// <param name="bankAccountUrl">The URL of the bank account</param>
    /// <param name="statementData">The statement file content as base64 encoded string</param>
    /// <param name="fileType">The file type: "ofx", "qif", or "csv"</param>
    /// <returns>Import summary with transaction details</returns>
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
    /// Upload a bank statement from a file path
    /// </summary>
    /// <param name="bankAccountUrl">The URL of the bank account</param>
    /// <param name="filePath">Path to the statement file</param>
    /// <returns>Import summary with transaction details</returns>
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
    /// Upload a bank statement from a byte array
    /// </summary>
    /// <param name="bankAccountUrl">The URL of the bank account</param>
    /// <param name="statementBytes">The statement file content as bytes</param>
    /// <param name="fileType">The file type: "ofx", "qif", or "csv"</param>
    /// <returns>Import summary with transaction details</returns>
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
    /// Upload a bank statement from a stream
    /// </summary>
    /// <param name="bankAccountUrl">The URL of the bank account</param>
    /// <param name="statementStream">The statement file content as a stream</param>
    /// <param name="fileType">The file type: "ofx", "qif", or "csv"</param>
    /// <returns>Import summary with transaction details</returns>
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
    /// Upload a CSV bank statement with custom parsing options
    /// </summary>
    /// <param name="bankAccountUrl">The URL of the bank account</param>
    /// <param name="csvContent">The CSV content as a string</param>
    /// <returns>Import summary with transaction details</returns>
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