// <copyright file="Bills.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing bills via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent bills, which represent purchase documents and expenses
/// payable to suppliers. Bills track money owed by the business and can be associated with contacts and
/// projects.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance. Cache entries are invalidated automatically
/// when bills are updated, deleted, or marked as paid.
/// </para>
/// </remarks>
/// <seealso cref="Bill"/>
/// <seealso cref="Contact"/>
/// <seealso cref="Project"/>
public class Bills
{
    private const string BillsEndPoint = "v2/bills";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Bills"/> class.
    /// </summary>
    /// <param name="freeAgentClient">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing bill data.</param>
    public Bills(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    /// <summary>
    /// Creates a new bill in FreeAgent.
    /// </summary>
    /// <param name="bill">The <see cref="Bill"/> object containing the bill details to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="Bill"/> object with server-assigned values (e.g., ID, URL, reference).
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/bills to create a new bill. The cache is not updated as
    /// only aggregate queries are cached.
    /// </remarks>
    public async Task<Bill> CreateAsync(Bill bill)
    {
        BillRoot root = new() { Bill = bill };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, BillsEndPoint),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        BillRoot? result = await response.Content.ReadFromJsonAsync<BillRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.Bill ?? throw new InvalidOperationException("Failed to deserialize bill response.");
    }

    /// <summary>
    /// Retrieves bills from FreeAgent with optional filtering by view and other criteria.
    /// </summary>
    /// <param name="view">
    /// The view filter to apply. Valid values: "all" (default), "open", "overdue", "open_or_overdue",
    /// "open_or_overdue_payments", "open_or_overdue_refunds", "paid", "recurring", "hire_purchase", "cis".
    /// </param>
    /// <param name="nestedBillItems">If true, includes bill line items in the response. Defaults to false.</param>
    /// <param name="updatedSince">Optional filter to retrieve only bills updated after this timestamp (ISO 8601 format).</param>
    /// <param name="fromDate">Optional start date to filter bills dated on or after this date.</param>
    /// <param name="toDate">Optional end date to filter bills dated on or before this date.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="Bill"/> objects matching the specified criteria.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/bills with the specified query parameters, handles pagination automatically,
    /// and caches the result for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<Bill>> GetAllAsync(
        string view = "all",
        bool nestedBillItems = false,
        DateTimeOffset? updatedSince = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null)
    {
        var queryParams = new List<string> { $"view={view}" };

        if (nestedBillItems)
        {
            queryParams.Add("nested_bill_items=true");
        }

        if (updatedSince.HasValue)
        {
            queryParams.Add($"updated_since={Uri.EscapeDataString(updatedSince.Value.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))}");
        }

        if (fromDate.HasValue)
        {
            queryParams.Add($"from_date={fromDate.Value:yyyy-MM-dd}");
        }

        if (toDate.HasValue)
        {
            queryParams.Add($"to_date={toDate.Value:yyyy-MM-dd}");
        }

        string queryString = string.Join("&", queryParams);
        string cacheKey = $"{BillsEndPoint}?{queryString}";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<Bill>? results))
        {
            List<BillsRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<BillsRoot>(
                new Uri(this.freeAgentClient.ApiBaseUrl, $"{BillsEndPoint}?{queryString}"))
                .ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Bills)];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves a specific bill by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the bill to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="Bill"/> object with the specified ID.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no bill with the specified ID is found.</exception>
    /// <remarks>
    /// This method calls GET /v2/bills/{id} and caches the result for 5 minutes.
    /// </remarks>
    public async Task<Bill> GetByIdAsync(string id)
    {
        string cacheKey = $"{BillsEndPoint}/{id}";

        if (!this.cache.TryGetValue(cacheKey, out Bill? results))
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(
                new Uri(freeAgentClient.ApiBaseUrl, $"{BillsEndPoint}/{id}")).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            BillRoot? root = await response.Content.ReadFromJsonAsync<BillRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            results = root?.Bill;
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? throw new InvalidOperationException($"Bill with ID {id} not found.");
    }

    /// <summary>
    /// Updates an existing bill in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the bill to update.</param>
    /// <param name="bill">The <see cref="Bill"/> object containing the updated bill details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="Bill"/> object as returned by the API.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/bills/{id} to update the bill. The cache entry for this bill
    /// is invalidated after a successful update.
    /// </remarks>
    public async Task<Bill> UpdateAsync(string id, Bill bill)
    {
        BillRoot root = new() { Bill = bill };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{BillsEndPoint}/{id}"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        BillRoot? result = await response.Content.ReadFromJsonAsync<BillRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache for this bill
        string cacheKey = $"{BillsEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return result?.Bill ?? throw new InvalidOperationException("Failed to deserialize bill response.");
    }

    /// <summary>
    /// Deletes a bill from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the bill to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls DELETE /v2/bills/{id} to delete the bill. The cache entry for this bill
    /// is invalidated after successful deletion.
    /// </remarks>
    public async Task DeleteAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.DeleteAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{BillsEndPoint}/{id}")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        // Invalidate cache
        string cacheKey = $"{BillsEndPoint}/{id}";
        this.cache.Remove(cacheKey);
    }

    /// <summary>
    /// Retrieves all bills associated with a specific contact from FreeAgent.
    /// </summary>
    /// <param name="contactUri">The URI of the contact to filter bills by.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="Bill"/> objects associated with the specified contact.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/bills?contact={contactUri}, handles pagination automatically, and caches
    /// the result for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<Bill>> GetAllByContactAsync(Uri contactUri)
    {
        string cacheKey = $"{BillsEndPoint}/contact/{contactUri}";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<Bill>? results))
        {
            List<BillsRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<BillsRoot>(
                new Uri(this.freeAgentClient.ApiBaseUrl, $"{BillsEndPoint}?contact={Uri.EscapeDataString(contactUri.ToString())}"))
                .ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Bills)];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves all bills associated with a specific project from FreeAgent.
    /// </summary>
    /// <param name="projectUri">The URI of the project to filter bills by.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="Bill"/> objects associated with the specified project.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/bills?project={projectUri}, handles pagination automatically, and caches
    /// the result for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<Bill>> GetAllByProjectAsync(Uri projectUri)
    {
        string cacheKey = $"{BillsEndPoint}/project/{projectUri}";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<Bill>? results))
        {
            List<BillsRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<BillsRoot>(
                new Uri(this.freeAgentClient.ApiBaseUrl, $"{BillsEndPoint}?project={Uri.EscapeDataString(projectUri.ToString())}"))
                .ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Bills)];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }
}