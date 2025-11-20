// <copyright file="BankTransactions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing bank transactions via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent bank transactions, which represent individual monetary
/// movements in and out of bank accounts. Transactions can be explained or unexplained, and are used for
/// bank reconciliation and financial reporting.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance. Cache entries are invalidated automatically
/// when transactions are updated or deleted.
/// </para>
/// </remarks>
/// <seealso cref="BankTransaction"/>
/// <seealso cref="BankAccount"/>
/// <seealso cref="BankTransactionExplanation"/>
public class BankTransactions
{
    private const string BankTransactionsEndPoint = "v2/bank_transactions";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="BankTransactions"/> class.
    /// </summary>
    /// <param name="freeAgentClient">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing bank transaction data.</param>
    public BankTransactions(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    /// <summary>
    /// Retrieves bank transactions from FreeAgent filtered by view and bank account.
    /// </summary>
    /// <param name="bankAccountUri">The URI of the bank account to retrieve transactions for. This parameter is required per the FreeAgent API specification.</param>
    /// <param name="view">The view filter to apply (e.g., "all", "explained", "unexplained"). Defaults to "all".</param>
    /// <param name="fromDate">Optional start date to filter transactions from (inclusive). Format: YYYY-MM-DD.</param>
    /// <param name="toDate">Optional end date to filter transactions to (inclusive). Format: YYYY-MM-DD.</param>
    /// <param name="updatedSince">Optional timestamp to filter transactions updated since this date/time (ISO 8601 format).</param>
    /// <param name="lastUploadedOnly">If true, returns only transactions from the most recent statement upload.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="BankTransaction"/> objects matching the specified criteria.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/bank_transactions with appropriate query parameters, handles
    /// pagination automatically, and caches the result for 5 minutes. The bank_account parameter is
    /// mandatory as specified in the FreeAgent API documentation.
    /// </remarks>
    public async Task<IEnumerable<BankTransaction>> GetAllAsync(
        Uri bankAccountUri,
        string view = "all",
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        DateTimeOffset? updatedSince = null,
        bool lastUploadedOnly = false)
    {
        string queryString = $"?view={view}&bank_account={Uri.EscapeDataString(bankAccountUri.ToString())}";

        if (fromDate.HasValue)
        {
            queryString += $"&from_date={fromDate.Value:yyyy-MM-dd}";
        }

        if (toDate.HasValue)
        {
            queryString += $"&to_date={toDate.Value:yyyy-MM-dd}";
        }

        if (updatedSince.HasValue)
        {
            queryString += $"&updated_since={Uri.EscapeDataString(updatedSince.Value.UtcDateTime.ToString("o"))}";
        }

        if (lastUploadedOnly)
        {
            queryString += "&last_uploaded=true";
        }

        string cacheKey = $"{BankTransactionsEndPoint}{queryString}";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<BankTransaction>? results))
        {
            List<BankTransactionsRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<BankTransactionsRoot>(
                new Uri(this.freeAgentClient.ApiBaseUrl, $"{BankTransactionsEndPoint}{queryString}"))
                .ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.BankTransactions)];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves unexplained bank transactions from FreeAgent for a specific bank account.
    /// </summary>
    /// <param name="bankAccountUri">The URI of the bank account to retrieve transactions for. This parameter is required per the FreeAgent API specification.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// unexplained <see cref="BankTransaction"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls <see cref="GetAllAsync"/> with view="unexplained" to retrieve only transactions
    /// that have not yet been explained or reconciled.
    /// </remarks>
    public async Task<IEnumerable<BankTransaction>> GetUnexplainedAsync(Uri bankAccountUri) => await this.GetAllAsync(bankAccountUri, "unexplained").ConfigureAwait(false);

    /// <summary>
    /// Retrieves explained bank transactions from FreeAgent for a specific bank account.
    /// </summary>
    /// <param name="bankAccountUri">The URI of the bank account to retrieve transactions for. This parameter is required per the FreeAgent API specification.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// explained <see cref="BankTransaction"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls <see cref="GetAllAsync"/> with view="explained" to retrieve only transactions
    /// that have been explained or reconciled.
    /// </remarks>
    public async Task<IEnumerable<BankTransaction>> GetExplainedAsync(Uri bankAccountUri) => await this.GetAllAsync(bankAccountUri, "explained").ConfigureAwait(false);

    /// <summary>
    /// Retrieves manual bank transactions from FreeAgent for a specific bank account.
    /// </summary>
    /// <param name="bankAccountUri">The URI of the bank account to retrieve transactions for. This parameter is required per the FreeAgent API specification.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// manually-entered <see cref="BankTransaction"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls <see cref="GetAllAsync"/> with view="manual" to retrieve only transactions
    /// that were manually created by users rather than imported from bank feeds or statement uploads.
    /// </remarks>
    public async Task<IEnumerable<BankTransaction>> GetManualTransactionsAsync(Uri bankAccountUri) => await this.GetAllAsync(bankAccountUri, "manual").ConfigureAwait(false);

    /// <summary>
    /// Retrieves imported bank transactions from FreeAgent for a specific bank account.
    /// </summary>
    /// <param name="bankAccountUri">The URI of the bank account to retrieve transactions for. This parameter is required per the FreeAgent API specification.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// imported <see cref="BankTransaction"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls <see cref="GetAllAsync"/> with view="imported" to retrieve only transactions
    /// that were imported from bank feeds or statement uploads rather than manually entered.
    /// </remarks>
    public async Task<IEnumerable<BankTransaction>> GetImportedTransactionsAsync(Uri bankAccountUri) => await this.GetAllAsync(bankAccountUri, "imported").ConfigureAwait(false);

    /// <summary>
    /// Retrieves bank transactions marked for review from FreeAgent for a specific bank account.
    /// </summary>
    /// <param name="bankAccountUri">The URI of the bank account to retrieve transactions for. This parameter is required per the FreeAgent API specification.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="BankTransaction"/> objects that have been flagged for review.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls <see cref="GetAllAsync"/> with view="marked_for_review" to retrieve only transactions
    /// that require attention or verification.
    /// </remarks>
    public async Task<IEnumerable<BankTransaction>> GetMarkedForReviewAsync(Uri bankAccountUri) => await this.GetAllAsync(bankAccountUri, "marked_for_review").ConfigureAwait(false);

    /// <summary>
    /// Retrieves a specific bank transaction by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the bank transaction to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="BankTransaction"/> object with the specified ID.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no bank transaction with the specified ID is found.</exception>
    /// <remarks>
    /// This method calls GET /v2/bank_transactions/{id} and caches the result for 5 minutes.
    /// </remarks>
    public async Task<BankTransaction> GetByIdAsync(string id)
    {
        string cacheKey = $"{BankTransactionsEndPoint}/{id}";

        if (!this.cache.TryGetValue(cacheKey, out BankTransaction? results))
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(
                new Uri(freeAgentClient.ApiBaseUrl, $"{BankTransactionsEndPoint}/{id}")).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            BankTransactionRoot? content = await response.Content.ReadFromJsonAsync<BankTransactionRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            results = content?.BankTransaction;
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? throw new InvalidOperationException($"Bank transaction with ID {id} not found.");
    }

    /// <summary>
    /// Deletes a bank transaction from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the bank transaction to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// <para>
    /// This method calls DELETE /v2/bank_transactions/{id} to delete the transaction. The cache entry for this
    /// transaction is invalidated after successful deletion.
    /// </para>
    /// <para>
    /// Important: Transactions can only be deleted if they are fully unexplained or lack explanations entirely.
    /// Transactions that have partial or complete explanations cannot be deleted and will result in an error.
    /// </para>
    /// </remarks>
    public async Task DeleteAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.DeleteAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{BankTransactionsEndPoint}/{id}")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        // Invalidate cache for both the transaction list and the specific transaction
        string cacheKey = $"{BankTransactionsEndPoint}/{id}";
        this.cache.Remove(cacheKey);
    }

    /// <summary>
    /// Uploads a bank statement file to FreeAgent and creates transactions from it.
    /// </summary>
    /// <param name="bankAccountUri">The URI of the bank account the statement belongs to.</param>
    /// <param name="statementData">The statement file content as a byte array.</param>
    /// <param name="fileName">The name of the statement file (e.g., "statement.ofx", "statement.csv").</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="BankTransaction"/> objects created from the uploaded statement.
    /// </returns>
    /// <exception cref="HttpRequestException">
    /// Thrown when the API request fails. Possible error responses include:
    /// <list type="bullet">
    /// <item><description>400 Bad Request - The request is malformed or contains invalid data.</description></item>
    /// <item><description>404 Not Found - The bank account URI is invalid or the resource does not exist.</description></item>
    /// <item><description>406 Not Acceptable - The statement file is missing or empty.</description></item>
    /// </list>
    /// </exception>
    /// <remarks>
    /// This method calls POST /v2/bank_transactions/statement?bank_account={bankAccountUri} using multipart/form-data
    /// to upload and parse a bank statement file. The API automatically creates bank transactions from the statement data.
    /// Supported file formats include OFX, QBO, QIF, or supported CSV file.
    /// </remarks>
    public async Task<IEnumerable<BankTransaction>> UploadStatementAsync(Uri bankAccountUri, byte[] statementData, string fileName)
    {
        string queryString = $"?bank_account={Uri.EscapeDataString(bankAccountUri.ToString())}";

        using MultipartFormDataContent content = new();
        ByteArrayContent fileContent = new(statementData);
        content.Add(fileContent, "statement", fileName);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{BankTransactionsEndPoint}/statement{queryString}"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        BankTransactionsRoot? results = await response.Content.ReadFromJsonAsync<BankTransactionsRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return results?.BankTransactions ?? [];
    }

    /// <summary>
    /// Uploads an array of bank transactions to FreeAgent as a JSON statement.
    /// </summary>
    /// <param name="bankAccountUri">The URI of the bank account the transactions belong to.</param>
    /// <param name="transactions">The collection of transactions to upload.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="BankTransaction"/> objects created from the uploaded transactions.
    /// </returns>
    /// <exception cref="HttpRequestException">
    /// Thrown when the API request fails. Possible error responses include:
    /// <list type="bullet">
    /// <item><description>400 Bad Request - The request is malformed or contains invalid data.</description></item>
    /// <item><description>404 Not Found - The bank account URI is invalid or the resource does not exist.</description></item>
    /// <item><description>406 Not Acceptable - The statement data is missing or empty.</description></item>
    /// </list>
    /// </exception>
    /// <remarks>
    /// <para>
    /// This method calls POST /v2/bank_transactions/statement?bank_account={bankAccountUri} using application/json
    /// to upload an array of transaction objects. The API automatically creates bank transactions and performs
    /// deduplication based on date, amount, and description matching.
    /// </para>
    /// <para>
    /// Best practice is to include all of a day's transactions in a single statement upload. Each transaction
    /// must include at minimum a dated_on field; other fields (description, amount, fitid, transaction_type) are optional.
    /// </para>
    /// <para>
    /// Valid transaction types include: CREDIT, DEBIT, INT, DIV, FEE, SRVCHG, DEP, ATM, POS, XFER, CHECK, PAYMENT,
    /// CASH, DIRECTDEP, DIRECTDEBIT, REPEATPMT, OTHER (default).
    /// </para>
    /// </remarks>
    /// <seealso cref="BankTransactionUpload"/>
    public async Task<IEnumerable<BankTransaction>> UploadStatementAsJsonAsync(Uri bankAccountUri, IEnumerable<BankTransactionUpload> transactions)
    {
        string queryString = $"?bank_account={Uri.EscapeDataString(bankAccountUri.ToString())}";

        BankTransactionUploadRoot uploadRoot = new()
        {
            Statement = transactions
        };

        using JsonContent content = JsonContent.Create(uploadRoot, options: SharedJsonOptions.SourceGenOptions);
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{BankTransactionsEndPoint}/statement{queryString}"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        BankTransactionsRoot? results = await response.Content.ReadFromJsonAsync<BankTransactionsRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return results?.BankTransactions ?? [];
    }
}