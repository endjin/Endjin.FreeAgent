// <copyright file="BankTransactionExplanations.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing bank transaction explanations via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent bank transaction explanations, which link bank transactions
/// to their accounting entries (invoices, bills, expenses, transfers, etc.). Explanations are essential for
/// bank reconciliation and maintaining accurate financial records.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance. Cache entries are invalidated automatically
/// when explanations are updated or deleted.
/// </para>
/// </remarks>
/// <seealso cref="BankTransactionExplanation"/>
/// <seealso cref="BankTransaction"/>
public class BankTransactionExplanations
{
    private const string BankTransactionExplanationsEndPoint = "v2/bank_transaction_explanations";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="BankTransactionExplanations"/> class.
    /// </summary>
    /// <param name="freeAgentClient">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing bank transaction explanation data.</param>
    public BankTransactionExplanations(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    /// <summary>
    /// Creates a new bank transaction explanation in FreeAgent.
    /// </summary>
    /// <param name="explanation">The <see cref="BankTransactionExplanation"/> object containing the explanation details to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="BankTransactionExplanation"/> object with server-assigned values (e.g., ID, URL).
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/bank_transaction_explanations to create a new explanation, effectively
    /// reconciling a bank transaction with its accounting entry. The cache is not updated as only aggregate
    /// queries are cached.
    /// </remarks>
    public async Task<BankTransactionExplanation> CreateAsync(BankTransactionExplanation explanation)
    {
        BankTransactionExplanationRoot root = new() { BankTransactionExplanation = explanation };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, BankTransactionExplanationsEndPoint),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        BankTransactionExplanationRoot? result = await response.Content.ReadFromJsonAsync<BankTransactionExplanationRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.BankTransactionExplanation ?? throw new InvalidOperationException("Failed to deserialize bank transaction explanation response.");
    }

    /// <summary>
    /// Retrieves bank transaction explanations from FreeAgent, optionally filtered by various criteria.
    /// </summary>
    /// <param name="bankAccountUri">Optional URI of the bank account to filter explanations by. Per API docs, this is a required parameter for the API.</param>
    /// <param name="bankTransactionUri">Optional URI of the bank transaction to filter explanations by.</param>
    /// <param name="fromDate">Optional start date to filter explanations by transaction date (inclusive).</param>
    /// <param name="toDate">Optional end date to filter explanations by transaction date (inclusive).</param>
    /// <param name="updatedSince">Optional timestamp to retrieve only explanations modified after this date and time. Useful for incremental synchronization.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="BankTransactionExplanation"/> objects matching the specified criteria.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// <para>
    /// This method calls GET /v2/bank_transaction_explanations with optional query parameters,
    /// handles pagination automatically, and caches the result for 5 minutes.
    /// </para>
    /// <para>
    /// According to the FreeAgent API documentation, the bank_account parameter is required when querying
    /// bank transaction explanations. However, this implementation makes it optional for backward compatibility
    /// and to support scenarios where the API may not enforce this requirement.
    /// </para>
    /// </remarks>
    public async Task<IEnumerable<BankTransactionExplanation>> GetAllAsync(
        Uri? bankAccountUri = null,
        Uri? bankTransactionUri = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        DateTimeOffset? updatedSince = null)
    {
        List<string> queryParams = [];

        if (bankAccountUri != null)
        {
            queryParams.Add($"bank_account={Uri.EscapeDataString(bankAccountUri.ToString())}");
        }

        if (bankTransactionUri != null)
        {
            queryParams.Add($"bank_transaction={Uri.EscapeDataString(bankTransactionUri.ToString())}");
        }

        if (fromDate.HasValue)
        {
            queryParams.Add($"from_date={fromDate.Value:yyyy-MM-dd}");
        }

        if (toDate.HasValue)
        {
            queryParams.Add($"to_date={toDate.Value:yyyy-MM-dd}");
        }

        if (updatedSince.HasValue)
        {
            queryParams.Add($"updated_since={Uri.EscapeDataString(updatedSince.Value.UtcDateTime.ToString("o"))}");
        }

        string queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
        string cacheKey = $"{BankTransactionExplanationsEndPoint}{queryString}";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<BankTransactionExplanation>? results))
        {
            List<BankTransactionExplanationsRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<BankTransactionExplanationsRoot>(
                new Uri(this.freeAgentClient.ApiBaseUrl, $"{BankTransactionExplanationsEndPoint}{queryString}"))
                .ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.BankTransactionExplanations)];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves a specific bank transaction explanation by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the bank transaction explanation to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="BankTransactionExplanation"/> object with the specified ID.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no explanation with the specified ID is found.</exception>
    /// <remarks>
    /// This method calls GET /v2/bank_transaction_explanations/{id} and caches the result for 5 minutes.
    /// </remarks>
    public async Task<BankTransactionExplanation> GetByIdAsync(string id)
    {
        string cacheKey = $"{BankTransactionExplanationsEndPoint}/{id}";

        if (!this.cache.TryGetValue(cacheKey, out BankTransactionExplanation? results))
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(
                new Uri(freeAgentClient.ApiBaseUrl, $"{BankTransactionExplanationsEndPoint}/{id}")).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            BankTransactionExplanationRoot? root = await response.Content.ReadFromJsonAsync<BankTransactionExplanationRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            results = root?.BankTransactionExplanation;
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? throw new InvalidOperationException($"Bank transaction explanation with ID {id} not found.");
    }

    /// <summary>
    /// Updates an existing bank transaction explanation in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the explanation to update.</param>
    /// <param name="explanation">The <see cref="BankTransactionExplanation"/> object containing the updated explanation details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="BankTransactionExplanation"/> object as returned by the API.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/bank_transaction_explanations/{id} to update the explanation. The cache entry
    /// for this explanation is invalidated after a successful update.
    /// </remarks>
    public async Task<BankTransactionExplanation> UpdateAsync(string id, BankTransactionExplanation explanation)
    {
        BankTransactionExplanationRoot root = new() { BankTransactionExplanation = explanation };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{BankTransactionExplanationsEndPoint}/{id}"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        BankTransactionExplanationRoot? result = await response.Content.ReadFromJsonAsync<BankTransactionExplanationRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache for this explanation
        string cacheKey = $"{BankTransactionExplanationsEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return result?.BankTransactionExplanation ?? throw new InvalidOperationException("Failed to deserialize bank transaction explanation response.");
    }

    /// <summary>
    /// Deletes a bank transaction explanation from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the explanation to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls DELETE /v2/bank_transaction_explanations/{id} to delete the explanation, effectively
    /// un-reconciling the bank transaction. The cache entry for this explanation is invalidated after successful deletion.
    /// </remarks>
    public async Task DeleteAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.DeleteAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{BankTransactionExplanationsEndPoint}/{id}")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        // Invalidate cache
        string cacheKey = $"{BankTransactionExplanationsEndPoint}/{id}";
        this.cache.Remove(cacheKey);
    }
}