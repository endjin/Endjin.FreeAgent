// <copyright file="CreditNoteReconciliations.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing credit note reconciliations via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent credit note reconciliations, which link credit notes
/// to invoices to offset amounts owed. When a credit note is reconciled with an invoice, it reduces the
/// outstanding balance on that invoice by the reconciled amount.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance. Cache entries are invalidated automatically
/// when reconciliations are created, updated, or deleted.
/// </para>
/// </remarks>
/// <seealso cref="CreditNoteReconciliation"/>
/// <seealso cref="CreditNote"/>
/// <seealso cref="Invoice"/>
public class CreditNoteReconciliations
{
    private const string CreditNoteReconciliationsEndPoint = "v2/credit_note_reconciliations";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CreditNoteReconciliations"/> class.
    /// </summary>
    /// <param name="freeAgentClient">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing credit note reconciliation data.</param>
    public CreditNoteReconciliations(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    /// <summary>
    /// Creates a new credit note reconciliation in FreeAgent.
    /// </summary>
    /// <param name="reconciliation">The <see cref="CreditNoteReconciliation"/> object containing the reconciliation details to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="CreditNoteReconciliation"/> object with server-assigned values (e.g., URL, timestamps).
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/credit_note_reconciliations to create a new reconciliation. The reconciliation
    /// links a credit note to an invoice, reducing the invoice's outstanding balance by the gross value.
    /// </remarks>
    public async Task<CreditNoteReconciliation> CreateAsync(CreditNoteReconciliation reconciliation)
    {
        CreditNoteReconciliationRoot root = new() { CreditNoteReconciliation = reconciliation };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, CreditNoteReconciliationsEndPoint),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        CreditNoteReconciliationRoot? result = await response.Content.ReadFromJsonAsync<CreditNoteReconciliationRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache for list endpoint (no filters case)
        this.cache.Remove($"{CreditNoteReconciliationsEndPoint}/all/all/all");

        return result?.CreditNoteReconciliation ?? throw new InvalidOperationException("Failed to deserialize credit note reconciliation response.");
    }

    /// <summary>
    /// Retrieves credit note reconciliations from FreeAgent with optional filtering.
    /// </summary>
    /// <param name="updatedSince">
    /// Optional timestamp to filter reconciliations updated since the specified time.
    /// Format: YYYY-MM-DDTHH:MM:SS.000Z
    /// </param>
    /// <param name="fromDate">
    /// Optional start date to filter reconciliations dated on or after this date.
    /// Format: YYYY-MM-DD
    /// </param>
    /// <param name="toDate">
    /// Optional end date to filter reconciliations dated on or before this date.
    /// Format: YYYY-MM-DD
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="CreditNoteReconciliation"/> objects matching the specified filters.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/credit_note_reconciliations with optional query parameters, handles pagination
    /// automatically, and caches the result for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<CreditNoteReconciliation>> GetAllAsync(DateTimeOffset? updatedSince = null, string? fromDate = null, string? toDate = null)
    {
        string cacheKey = $"{CreditNoteReconciliationsEndPoint}/{updatedSince?.ToString("O") ?? "all"}/{fromDate ?? "all"}/{toDate ?? "all"}";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<CreditNoteReconciliation>? results))
        {
            List<string> queryParams = [];

            if (updatedSince.HasValue)
            {
                queryParams.Add($"updated_since={Uri.EscapeDataString(updatedSince.Value.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))}");
            }

            if (!string.IsNullOrEmpty(fromDate))
            {
                queryParams.Add($"from_date={Uri.EscapeDataString(fromDate)}");
            }

            if (!string.IsNullOrEmpty(toDate))
            {
                queryParams.Add($"to_date={Uri.EscapeDataString(toDate)}");
            }

            string queryString = queryParams.Count > 0 ? $"?{string.Join("&", queryParams)}" : string.Empty;

            List<CreditNoteReconciliationsRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<CreditNoteReconciliationsRoot>(
                new Uri(this.freeAgentClient.ApiBaseUrl, $"{CreditNoteReconciliationsEndPoint}{queryString}"))
                .ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.CreditNoteReconciliations)];
            this.cache.Set(cacheKey, results, this.cacheEntryOptions);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves a specific credit note reconciliation by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the credit note reconciliation to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="CreditNoteReconciliation"/> object with the specified ID.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no credit note reconciliation with the specified ID is found.</exception>
    /// <remarks>
    /// This method calls GET /v2/credit_note_reconciliations/{id} and caches the result for 5 minutes.
    /// </remarks>
    public async Task<CreditNoteReconciliation> GetByIdAsync(string id)
    {
        string cacheKey = $"{CreditNoteReconciliationsEndPoint}/{id}";

        if (!this.cache.TryGetValue(cacheKey, out CreditNoteReconciliation? results))
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(
                new Uri(this.freeAgentClient.ApiBaseUrl, $"{CreditNoteReconciliationsEndPoint}/{id}")).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            CreditNoteReconciliationRoot? root = await response.Content.ReadFromJsonAsync<CreditNoteReconciliationRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            results = root?.CreditNoteReconciliation;
            this.cache.Set(cacheKey, results, this.cacheEntryOptions);
        }

        return results ?? throw new InvalidOperationException($"Credit note reconciliation with ID {id} not found.");
    }

    /// <summary>
    /// Updates an existing credit note reconciliation in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the credit note reconciliation to update.</param>
    /// <param name="reconciliation">The <see cref="CreditNoteReconciliation"/> object containing the updated reconciliation details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="CreditNoteReconciliation"/> object as returned by the API.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/credit_note_reconciliations/{id} to update the reconciliation. The cache entry
    /// for this reconciliation is invalidated after a successful update.
    /// </remarks>
    public async Task<CreditNoteReconciliation> UpdateAsync(string id, CreditNoteReconciliation reconciliation)
    {
        CreditNoteReconciliationRoot root = new() { CreditNoteReconciliation = reconciliation };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{CreditNoteReconciliationsEndPoint}/{id}"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        CreditNoteReconciliationRoot? result = await response.Content.ReadFromJsonAsync<CreditNoteReconciliationRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache for this reconciliation and list endpoint
        string cacheKey = $"{CreditNoteReconciliationsEndPoint}/{id}";
        this.cache.Remove(cacheKey);
        this.cache.Remove($"{CreditNoteReconciliationsEndPoint}/all/all/all");

        return result?.CreditNoteReconciliation ?? throw new InvalidOperationException("Failed to deserialize credit note reconciliation response.");
    }

    /// <summary>
    /// Deletes a credit note reconciliation from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the credit note reconciliation to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls DELETE /v2/credit_note_reconciliations/{id} to delete the reconciliation. The cache entry
    /// for this reconciliation is invalidated after successful deletion.
    /// </remarks>
    public async Task DeleteAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.DeleteAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{CreditNoteReconciliationsEndPoint}/{id}")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        // Invalidate cache for this reconciliation and list endpoint
        string cacheKey = $"{CreditNoteReconciliationsEndPoint}/{id}";
        this.cache.Remove(cacheKey);
        this.cache.Remove($"{CreditNoteReconciliationsEndPoint}/all/all/all");
    }
}
