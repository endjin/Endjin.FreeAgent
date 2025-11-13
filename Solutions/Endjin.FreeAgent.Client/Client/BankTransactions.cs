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
    /// Creates a new bank transaction in FreeAgent.
    /// </summary>
    /// <param name="bankTransaction">The <see cref="BankTransaction"/> object containing the transaction details to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="BankTransaction"/> object with server-assigned values (e.g., ID, URL).
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/bank_transactions to create a new transaction. The cache is not updated as
    /// only aggregate queries are cached.
    /// </remarks>
    public async Task<BankTransaction> CreateAsync(BankTransaction bankTransaction)
    {
        BankTransactionRoot root = new() { BankTransaction = bankTransaction };

        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, BankTransactionsEndPoint),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        BankTransactionRoot? results = await response.Content.ReadFromJsonAsync<BankTransactionRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return results?.BankTransaction ?? throw new InvalidOperationException("Failed to deserialize bank transaction response.");
    }

    /// <summary>
    /// Retrieves bank transactions from FreeAgent filtered by view and optionally by bank account.
    /// </summary>
    /// <param name="bankAccountUri">Optional URI of the bank account to filter transactions by. If null, transactions from all accounts are returned.</param>
    /// <param name="view">The view filter to apply (e.g., "all", "explained", "unexplained"). Defaults to "all".</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="BankTransaction"/> objects matching the specified criteria.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/bank_transactions?view={view}&amp;bank_account={bankAccountUri}, handles
    /// pagination automatically, and caches the result for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<BankTransaction>> GetAllAsync(Uri? bankAccountUri = null, string view = "all")
    {
        string queryString = $"?view={view}";
        if (bankAccountUri != null)
        {
            queryString += $"&bank_account={Uri.EscapeDataString(bankAccountUri.ToString())}";
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
    /// Retrieves unexplained bank transactions from FreeAgent, optionally filtered by bank account.
    /// </summary>
    /// <param name="bankAccountUri">Optional URI of the bank account to filter transactions by.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// unexplained <see cref="BankTransaction"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls <see cref="GetAllAsync"/> with view="unexplained" to retrieve only transactions
    /// that have not yet been explained or reconciled.
    /// </remarks>
    public async Task<IEnumerable<BankTransaction>> GetUnexplainedAsync(Uri? bankAccountUri = null) => await this.GetAllAsync(bankAccountUri, "unexplained").ConfigureAwait(false);

    /// <summary>
    /// Retrieves explained bank transactions from FreeAgent, optionally filtered by bank account.
    /// </summary>
    /// <param name="bankAccountUri">Optional URI of the bank account to filter transactions by.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// explained <see cref="BankTransaction"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls <see cref="GetAllAsync"/> with view="explained" to retrieve only transactions
    /// that have been explained or reconciled.
    /// </remarks>
    public async Task<IEnumerable<BankTransaction>> GetExplainedAsync(Uri? bankAccountUri = null) => await this.GetAllAsync(bankAccountUri, "explained").ConfigureAwait(false);

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
    /// Updates an existing bank transaction in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the bank transaction to update.</param>
    /// <param name="bankTransaction">The <see cref="BankTransaction"/> object containing the updated transaction details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="BankTransaction"/> object as returned by the API.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/bank_transactions/{id} to update the transaction. The cache entry for this
    /// transaction is invalidated after a successful update.
    /// </remarks>
    public async Task<BankTransaction> UpdateAsync(string id, BankTransaction bankTransaction)
    {
        BankTransactionRoot root = new() { BankTransaction = bankTransaction };

        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{BankTransactionsEndPoint}/{id}"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        BankTransactionRoot? results = await response.Content.ReadFromJsonAsync<BankTransactionRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache for this bank transaction
        string cacheKey = $"{BankTransactionsEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return results?.BankTransaction ?? throw new InvalidOperationException("Failed to deserialize bank transaction response.");
    }

    /// <summary>
    /// Deletes a bank transaction from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the bank transaction to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls DELETE /v2/bank_transactions/{id} to delete the transaction. The cache entry for this
    /// transaction is invalidated after successful deletion.
    /// </remarks>
    public async Task DeleteAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.DeleteAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{BankTransactionsEndPoint}/{id}")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        // Invalidate cache
        string cacheKey = $"{BankTransactionsEndPoint}/{id}";
        this.cache.Remove(cacheKey);
    }

    /// <summary>
    /// Uploads a bank statement to FreeAgent and creates transactions from it.
    /// </summary>
    /// <param name="bankAccountUri">The URI of the bank account the statement belongs to.</param>
    /// <param name="statementData">The statement data in the specified file format (e.g., OFX, QIF, CSV).</param>
    /// <param name="fileType">The file type of the statement (e.g., "ofx", "qif", "csv").</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="BankTransaction"/> objects created from the uploaded statement.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls POST /v2/bank_transactions/statement to upload and parse a bank statement file.
    /// The API automatically creates bank transactions from the statement data.
    /// </remarks>
    public async Task<IEnumerable<BankTransaction>> UploadStatementAsync(Uri bankAccountUri, string statementData, string fileType)
    {
        StatementUploadRoot statementRoot = new()
        {
            Statement = new StatementUpload
            {
                BankAccount = bankAccountUri.ToString(),
                Statement = statementData,
                FileType = fileType
            }
        };

        using JsonContent content = JsonContent.Create(statementRoot, options: SharedJsonOptions.SourceGenOptions);
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{BankTransactionsEndPoint}/statement"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        BankTransactionsRoot? results = await response.Content.ReadFromJsonAsync<BankTransactionsRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return results?.BankTransactions ?? [];
    }
}