// <copyright file="BankAccounts.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing bank accounts via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent bank accounts, which represent real-world bank accounts
/// linked to the business. Bank accounts are used to track transactions, reconcile statements, and manage
/// cash flow.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance. Cache entries are invalidated automatically
/// when bank accounts are updated or deleted.
/// </para>
/// </remarks>
/// <seealso cref="BankAccount"/>
/// <seealso cref="BankTransaction"/>
public class BankAccounts
{
    private const string BankAccountsEndPoint = "v2/bank_accounts";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="BankAccounts"/> class.
    /// </summary>
    /// <param name="freeAgentClient">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing bank account data.</param>
    public BankAccounts(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    /// <summary>
    /// Creates a new bank account in FreeAgent.
    /// </summary>
    /// <param name="bankAccount">The <see cref="BankAccount"/> object containing the bank account details to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="BankAccount"/> object with server-assigned values (e.g., ID, URL).
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/bank_accounts to create a new bank account. The cache is not updated as
    /// only aggregate queries are cached.
    /// </remarks>
    public async Task<BankAccount> CreateAsync(BankAccount bankAccount)
    {
        BankAccountRoot root = new() { BankAccount = bankAccount };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, BankAccountsEndPoint),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        BankAccountRoot? result = await response.Content.ReadFromJsonAsync<BankAccountRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.BankAccount ?? throw new InvalidOperationException("Failed to deserialize bank account response.");
    }

    /// <summary>
    /// Retrieves bank accounts from FreeAgent filtered by view.
    /// </summary>
    /// <param name="view">The view filter to apply. Valid values are "standard_bank_accounts", "credit_card_accounts", "paypal_accounts", or "all". Defaults to "all".</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="BankAccount"/> objects matching the specified view.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/bank_accounts?view={view}, handles pagination automatically, and caches the
    /// result for 5 minutes. Use specific view filters to retrieve only accounts of a particular type.
    /// </remarks>
    public async Task<IEnumerable<BankAccount>> GetAllAsync(string view = "all")
    {
        string cacheKey = $"{BankAccountsEndPoint}/{view}";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<BankAccount>? results))
        {
            List<BankAccountsRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<BankAccountsRoot>(
                new Uri(this.freeAgentClient.ApiBaseUrl, $"{BankAccountsEndPoint}?view={view}"))
                .ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.BankAccounts)];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves a specific bank account by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the bank account to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="BankAccount"/> object with the specified ID.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no bank account with the specified ID is found.</exception>
    /// <remarks>
    /// This method calls GET /v2/bank_accounts/{id} and caches the result for 5 minutes.
    /// </remarks>
    public async Task<BankAccount> GetByIdAsync(string id)
    {
        string cacheKey = $"{BankAccountsEndPoint}/{id}";

        if (!this.cache.TryGetValue(cacheKey, out BankAccount? results))
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(
                new Uri(freeAgentClient.ApiBaseUrl, $"{BankAccountsEndPoint}/{id}")).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            BankAccountRoot? root = await response.Content.ReadFromJsonAsync<BankAccountRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            results = root?.BankAccount;
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? throw new InvalidOperationException($"Bank account with ID {id} not found.");
    }

    /// <summary>
    /// Updates an existing bank account in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the bank account to update.</param>
    /// <param name="bankAccount">The <see cref="BankAccount"/> object containing the updated bank account details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="BankAccount"/> object as returned by the API.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/bank_accounts/{id} to update the bank account. The cache entry for this
    /// bank account is invalidated after a successful update.
    /// </remarks>
    public async Task<BankAccount> UpdateAsync(string id, BankAccount bankAccount)
    {
        BankAccountRoot root = new() { BankAccount = bankAccount };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{BankAccountsEndPoint}/{id}"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        BankAccountRoot? result = await response.Content.ReadFromJsonAsync<BankAccountRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache for this bank account
        string cacheKey = $"{BankAccountsEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return result?.BankAccount ?? throw new InvalidOperationException("Failed to deserialize bank account response.");
    }

    /// <summary>
    /// Deletes a bank account from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the bank account to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls DELETE /v2/bank_accounts/{id} to delete the bank account. The cache entry for this
    /// bank account is invalidated after successful deletion.
    /// </remarks>
    public async Task DeleteAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.DeleteAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{BankAccountsEndPoint}/{id}")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        // Invalidate cache
        string cacheKey = $"{BankAccountsEndPoint}/{id}";
        this.cache.Remove(cacheKey);
    }
}