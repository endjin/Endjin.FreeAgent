// <copyright file="BankFeeds.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for accessing bank feeds via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent bank feeds, which represent connections to
/// external bank accounts for automatic transaction importing.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance.
/// </para>
/// </remarks>
/// <seealso cref="BankFeed"/>
public class BankFeeds
{
    private const string BankFeedsEndPoint = "v2/bank_feeds";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="BankFeeds"/> class.
    /// </summary>
    /// <param name="freeAgentClient">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing bank feed data.</param>
    public BankFeeds(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    /// <summary>
    /// Retrieves all bank feeds for the company.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="BankFeed"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/bank_feeds and caches the result for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<BankFeed>> GetAllAsync()
    {
        string cacheKey = BankFeedsEndPoint;

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<BankFeed>? results))
        {
            List<BankFeedsRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<BankFeedsRoot>(
                new Uri(this.freeAgentClient.ApiBaseUrl, BankFeedsEndPoint))
                .ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.BankFeeds ?? [])];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves a specific bank feed by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the bank feed to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="BankFeed"/> object with the specified ID.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no bank feed with the specified ID is found.</exception>
    /// <remarks>
    /// This method calls GET /v2/bank_feeds/{id} and caches the result for 5 minutes.
    /// </remarks>
    public async Task<BankFeed> GetByIdAsync(string id)
    {
        string cacheKey = $"{BankFeedsEndPoint}/{id}";

        if (!this.cache.TryGetValue(cacheKey, out BankFeed? results))
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(
                new Uri(this.freeAgentClient.ApiBaseUrl, $"{BankFeedsEndPoint}/{id}")).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            BankFeedRoot? root = await response.Content.ReadFromJsonAsync<BankFeedRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            results = root?.BankFeed;
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? throw new InvalidOperationException($"Bank feed with ID {id} not found.");
    }
}
