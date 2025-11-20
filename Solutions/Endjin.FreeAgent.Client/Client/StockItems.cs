// <copyright file="StockItems.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides read-only access to stock items via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent stock items, which track inventory for businesses
/// that buy and sell physical goods. Stock items manage quantities, values, and movements of stock,
/// including opening quantities, opening balances, stock on hand, and cost of sales categories.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance.
/// </para>
/// </remarks>
/// <seealso cref="StockItem"/>
/// <seealso cref="Invoice"/>
/// <seealso cref="Estimate"/>
public class StockItems
{
    private const string StockItemsEndPoint = "v2/stock_items";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="StockItems"/> class.
    /// </summary>
    /// <param name="freeAgentClient">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing stock item data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="freeAgentClient"/> or <paramref name="cache"/> is null.</exception>
    public StockItems(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient ?? throw new ArgumentNullException(nameof(freeAgentClient));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    /// <summary>
    /// Retrieves all stock items from FreeAgent.
    /// </summary>
    /// <param name="sort">
    /// Optional sort order for the results. Valid values are: created_at (default), description, updated_at.
    /// Prefix with a hyphen for descending order (e.g., "-created_at").
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// all <see cref="StockItem"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/stock_items and caches the result for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<StockItem>> GetAllAsync(string? sort = null)
    {
        string cacheKey = string.IsNullOrEmpty(sort) ? StockItemsEndPoint : $"{StockItemsEndPoint}_{sort}";

        if (this.cache.TryGetValue(cacheKey, out IEnumerable<StockItem>? cached))
        {
            return cached!;
        }

        await this.freeAgentClient.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        string endpoint = StockItemsEndPoint;
        if (!string.IsNullOrEmpty(sort))
        {
            endpoint += $"?sort={Uri.EscapeDataString(sort)}";
        }

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(new Uri(this.freeAgentClient.ApiBaseUrl, endpoint)).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        StockItemsRoot? root = await response.Content.ReadFromJsonAsync<StockItemsRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        IEnumerable<StockItem> items = root?.StockItems ?? [];

        this.cache.Set(cacheKey, items, this.cacheEntryOptions);

        return items;
    }

    /// <summary>
    /// Retrieves a specific stock item by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the stock item to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="StockItem"/> object with the specified ID.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no stock item with the specified ID is found.</exception>
    /// <remarks>
    /// This method calls GET /v2/stock_items/{id} and caches the result for 5 minutes.
    /// </remarks>
    public async Task<StockItem> GetByIdAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        string cacheKey = $"{StockItemsEndPoint}/{id}";

        if (this.cache.TryGetValue(cacheKey, out StockItem? cached))
        {
            return cached!;
        }

        await this.freeAgentClient.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(new Uri(this.freeAgentClient.ApiBaseUrl, $"{StockItemsEndPoint}/{id}")).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        StockItemRoot? root = await response.Content.ReadFromJsonAsync<StockItemRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        StockItem? item = root?.StockItem;

        if (item == null)
        {
            throw new InvalidOperationException($"Stock item {id} not found");
        }

        this.cache.Set(cacheKey, item, this.cacheEntryOptions);

        return item;
    }
}