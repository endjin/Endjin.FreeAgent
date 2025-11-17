// <copyright file="StockItems.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing stock items via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent stock items, which represent products or services
/// that can be added to invoices and estimates. Stock items include details like description, price,
/// and sales tax rate, making invoice creation faster and more consistent.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance. Cache entries are invalidated automatically
/// when stock items are created, updated, or deleted.
/// </para>
/// </remarks>
/// <seealso cref="StockItem"/>
/// <seealso cref="Invoice"/>
/// <seealso cref="Estimate"/>
public class StockItems
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="StockItems"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing stock item data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public StockItems(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Creates a new stock item in FreeAgent.
    /// </summary>
    /// <param name="item">The <see cref="StockItem"/> object containing the stock item details to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="StockItem"/> object with server-assigned values (e.g., ID, URL).
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/stock_items to create a new stock item. The cache is invalidated
    /// to ensure subsequent queries return up-to-date data.
    /// </remarks>
    public async Task<StockItem> CreateAsync(StockItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        StockItemRoot data = new() { StockItem = item };
        using JsonContent content = JsonContent.Create(data, options: SharedJsonOptions.SourceGenOptions);

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.PostAsync(new Uri(this.client.ApiBaseUrl, "/v2/stock_items"), content);
        response.EnsureSuccessStatusCode();

        StockItemRoot? root = await response.Content.ReadFromJsonAsync<StockItemRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove("stock_items_all");

        return root?.StockItem ?? throw new InvalidOperationException("Failed to create stock item");
    }

    /// <summary>
    /// Retrieves all stock items from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// all <see cref="StockItem"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/stock_items and caches the result for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<StockItem>> GetAllAsync()
    {
        string cacheKey = "stock_items_all";

        if (this.cache.TryGetValue(cacheKey, out IEnumerable<StockItem>? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, "/v2/stock_items"));
        response.EnsureSuccessStatusCode();

        StockItemsRoot? root = await response.Content.ReadFromJsonAsync<StockItemsRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        IEnumerable<StockItem> items = root?.StockItems ?? [];

        this.cache.Set(cacheKey, items, TimeSpan.FromMinutes(5));

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

        string cacheKey = $"stock_item_{id}";

        if (this.cache.TryGetValue(cacheKey, out StockItem? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, $"/v2/stock_items/{id}"));
        response.EnsureSuccessStatusCode();

        StockItemRoot? root = await response.Content.ReadFromJsonAsync<StockItemRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        StockItem? item = root?.StockItem;

        if (item == null)
        {
            throw new InvalidOperationException($"Stock item {id} not found");
        }

        this.cache.Set(cacheKey, item, TimeSpan.FromMinutes(5));

        return item;
    }

    /// <summary>
    /// Updates an existing stock item in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the stock item to update.</param>
    /// <param name="item">The <see cref="StockItem"/> object containing the updated stock item details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="StockItem"/> object as returned by the API.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/stock_items/{id} to update the stock item. The cache entries for this
    /// stock item and all stock item queries are invalidated after a successful update.
    /// </remarks>
    public async Task<StockItem> UpdateAsync(string id, StockItem item)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(item);

        StockItemRoot data = new() { StockItem = item };
        using JsonContent content = JsonContent.Create(data, options: SharedJsonOptions.SourceGenOptions);

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.PutAsync(new Uri(this.client.ApiBaseUrl, $"/v2/stock_items/{id}"), content);
        response.EnsureSuccessStatusCode();

        StockItemRoot? root = await response.Content.ReadFromJsonAsync<StockItemRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove($"stock_item_{id}");
        this.cache.Remove("stock_items_all");

        return root?.StockItem ?? throw new InvalidOperationException("Failed to update stock item");
    }

    /// <summary>
    /// Deletes a stock item from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the stock item to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls DELETE /v2/stock_items/{id} to delete the stock item. The cache entries for
    /// this stock item and all stock item queries are invalidated after successful deletion.
    /// </remarks>
    public async Task DeleteAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.DeleteAsync(new Uri(this.client.ApiBaseUrl, $"/v2/stock_items/{id}"));
        response.EnsureSuccessStatusCode();

        this.cache.Remove($"stock_item_{id}");
        this.cache.Remove("stock_items_all");
    }
}