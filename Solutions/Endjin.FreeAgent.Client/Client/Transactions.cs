// <copyright file="Transactions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides read-only access to accounting transactions via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent accounting transactions, which represent
/// individual entries in the general ledger. Transactions are automatically generated when
/// invoices, bills, expenses, and other financial documents are processed.
/// </para>
/// <para>
/// Transactions can be filtered by date range and nominal code. Note that requested date periods
/// must be equal to or less than 12 months, or be contained within a single accounting year.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance.
/// </para>
/// </remarks>
/// <seealso cref="Transaction"/>
/// <seealso cref="Category"/>
public class Transactions
{
    private const string TransactionsEndPoint = "v2/accounting/transactions";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Transactions"/> class.
    /// </summary>
    /// <param name="freeAgentClient">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing transaction data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="freeAgentClient"/> or <paramref name="cache"/> is null.</exception>
    public Transactions(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient ?? throw new ArgumentNullException(nameof(freeAgentClient));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    /// <summary>
    /// Retrieves all accounting transactions from FreeAgent with optional filtering.
    /// </summary>
    /// <param name="fromDate">
    /// Optional start date for filtering transactions (inclusive). Must be used with <paramref name="toDate"/>.
    /// </param>
    /// <param name="toDate">
    /// Optional end date for filtering transactions (inclusive). Must be used with <paramref name="fromDate"/>.
    /// </param>
    /// <param name="nominalCode">
    /// Optional nominal code to filter transactions by a specific account in the chart of accounts.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// all <see cref="Transaction"/> objects matching the specified filters.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// <para>
    /// This method calls GET /v2/accounting/transactions and caches the result for 5 minutes.
    /// </para>
    /// <para>
    /// Date periods must be equal to or less than 12 months, or be contained within a single accounting year.
    /// </para>
    /// </remarks>
    public async Task<IEnumerable<Transaction>> GetAllAsync(
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        string? nominalCode = null)
    {
        List<string> queryParams = [];

        if (fromDate.HasValue)
        {
            queryParams.Add($"from_date={fromDate.Value:yyyy-MM-dd}");
        }

        if (toDate.HasValue)
        {
            queryParams.Add($"to_date={toDate.Value:yyyy-MM-dd}");
        }

        if (!string.IsNullOrEmpty(nominalCode))
        {
            queryParams.Add($"nominal_code={Uri.EscapeDataString(nominalCode)}");
        }

        string queryString = queryParams.Count > 0 ? $"?{string.Join("&", queryParams)}" : string.Empty;
        string cacheKey = $"{TransactionsEndPoint}{queryString}";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<Transaction>? results))
        {
            List<TransactionsRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<TransactionsRoot>(
                new Uri(this.freeAgentClient.ApiBaseUrl, $"{TransactionsEndPoint}{queryString}"))
                .ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Transactions ?? [])];
            this.cache.Set(cacheKey, results, this.cacheEntryOptions);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves a specific accounting transaction by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the transaction to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="Transaction"/> object with the specified ID.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no transaction with the specified ID is found.</exception>
    /// <remarks>
    /// This method calls GET /v2/accounting/transactions/{id} and caches the result for 5 minutes.
    /// </remarks>
    public async Task<Transaction> GetByIdAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        string cacheKey = $"{TransactionsEndPoint}/{id}";

        if (this.cache.TryGetValue(cacheKey, out Transaction? cached))
        {
            return cached!;
        }

        await this.freeAgentClient.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{TransactionsEndPoint}/{id}")).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        TransactionRoot? root = await response.Content.ReadFromJsonAsync<TransactionRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        Transaction? item = root?.Transaction;

        if (item == null)
        {
            throw new InvalidOperationException($"Transaction {id} not found");
        }

        this.cache.Set(cacheKey, item, this.cacheEntryOptions);

        return item;
    }
}
