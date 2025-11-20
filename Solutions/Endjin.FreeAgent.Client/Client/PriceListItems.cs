// <copyright file="PriceListItems.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing price list items via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent price list items, which are pre-configured
/// products or services that can be quickly added to invoices and estimates. Price list items
/// store standard information like description, quantity, unit price, and tax settings, making
/// document creation faster and more consistent.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance. Cache entries are invalidated automatically
/// when price list items are created, updated, or deleted.
/// </para>
/// </remarks>
/// <seealso cref="PriceListItem"/>
/// <seealso cref="Invoice"/>
/// <seealso cref="Estimate"/>
public class PriceListItems
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="PriceListItems"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing price list item data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public PriceListItems(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Creates a new price list item in FreeAgent.
    /// </summary>
    /// <param name="item">The <see cref="PriceListItem"/> object containing the price list item details to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="PriceListItem"/> object with server-assigned values (e.g., ID, URL).
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/price_list_items to create a new price list item. The cache is invalidated
    /// to ensure subsequent queries return up-to-date data.
    /// </remarks>
    public async Task<PriceListItem> CreateAsync(PriceListItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        PriceListItemRoot data = new() { PriceListItem = item };
        using JsonContent content = JsonContent.Create(data, options: SharedJsonOptions.SourceGenOptions);

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.PostAsync(new Uri(this.client.ApiBaseUrl, "/v2/price_list_items"), content);
        response.EnsureSuccessStatusCode();

        PriceListItemRoot? root = await response.Content.ReadFromJsonAsync<PriceListItemRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.InvalidateAllCache();

        return root?.PriceListItem ?? throw new InvalidOperationException("Failed to create price list item");
    }

    /// <summary>
    /// Retrieves all price list items from FreeAgent.
    /// </summary>
    /// <param name="sort">
    /// Optional sort order for the results. Valid values are: created_at (default), code, updated_at.
    /// Prefix with a hyphen for descending order (e.g., "-created_at").
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// all <see cref="PriceListItem"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/price_list_items and caches the result for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<PriceListItem>> GetAllAsync(string? sort = null)
    {
        string cacheKey = string.IsNullOrEmpty(sort) ? "price_list_items_all" : $"price_list_items_all_{sort}";

        if (this.cache.TryGetValue(cacheKey, out IEnumerable<PriceListItem>? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync();

        string endpoint = "/v2/price_list_items";
        if (!string.IsNullOrEmpty(sort))
        {
            endpoint += $"?sort={Uri.EscapeDataString(sort)}";
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, endpoint));
        response.EnsureSuccessStatusCode();

        PriceListItemsRoot? root = await response.Content.ReadFromJsonAsync<PriceListItemsRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        IEnumerable<PriceListItem> items = root?.PriceListItems ?? [];

        this.cache.Set(cacheKey, items, TimeSpan.FromMinutes(5));

        return items;
    }

    /// <summary>
    /// Retrieves a specific price list item by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the price list item to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="PriceListItem"/> object with the specified ID.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no price list item with the specified ID is found.</exception>
    /// <remarks>
    /// This method calls GET /v2/price_list_items/{id} and caches the result for 5 minutes.
    /// </remarks>
    public async Task<PriceListItem> GetByIdAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        string cacheKey = $"price_list_item_{id}";

        if (this.cache.TryGetValue(cacheKey, out PriceListItem? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, $"/v2/price_list_items/{id}"));
        response.EnsureSuccessStatusCode();

        PriceListItemRoot? root = await response.Content.ReadFromJsonAsync<PriceListItemRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        PriceListItem? item = root?.PriceListItem;

        if (item == null)
        {
            throw new InvalidOperationException($"Price list item {id} not found");
        }

        this.cache.Set(cacheKey, item, TimeSpan.FromMinutes(5));

        return item;
    }

    /// <summary>
    /// Updates an existing price list item in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the price list item to update.</param>
    /// <param name="item">The <see cref="PriceListItem"/> object containing the updated price list item details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="PriceListItem"/> object as returned by the API.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/price_list_items/{id} to update the price list item. The cache entries for this
    /// price list item and all price list item queries are invalidated after a successful update.
    /// </remarks>
    public async Task<PriceListItem> UpdateAsync(string id, PriceListItem item)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(item);

        PriceListItemRoot data = new() { PriceListItem = item };
        using JsonContent content = JsonContent.Create(data, options: SharedJsonOptions.SourceGenOptions);

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.PutAsync(new Uri(this.client.ApiBaseUrl, $"/v2/price_list_items/{id}"), content);
        response.EnsureSuccessStatusCode();

        PriceListItemRoot? root = await response.Content.ReadFromJsonAsync<PriceListItemRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove($"price_list_item_{id}");
        this.InvalidateAllCache();

        return root?.PriceListItem ?? throw new InvalidOperationException("Failed to update price list item");
    }

    /// <summary>
    /// Deletes a price list item from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the price list item to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls DELETE /v2/price_list_items/{id} to delete the price list item. The cache entries for
    /// this price list item and all price list item queries are invalidated after successful deletion.
    /// </remarks>
    public async Task DeleteAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.DeleteAsync(new Uri(this.client.ApiBaseUrl, $"/v2/price_list_items/{id}"));
        response.EnsureSuccessStatusCode();

        this.cache.Remove($"price_list_item_{id}");
        this.InvalidateAllCache();
    }

    /// <summary>
    /// Invalidates all cached price list item queries.
    /// </summary>
    /// <remarks>
    /// This method removes all cache entries that start with "price_list_items_all" to ensure
    /// that subsequent queries return fresh data from the API. This is called after any
    /// mutation operation (create, update, delete).
    /// </remarks>
    private void InvalidateAllCache()
    {
        // Remove the default cache key and any sorted variants
        this.cache.Remove("price_list_items_all");
        this.cache.Remove("price_list_items_all_created_at");
        this.cache.Remove("price_list_items_all_-created_at");
        this.cache.Remove("price_list_items_all_code");
        this.cache.Remove("price_list_items_all_-code");
        this.cache.Remove("price_list_items_all_updated_at");
        this.cache.Remove("price_list_items_all_-updated_at");
    }
}
