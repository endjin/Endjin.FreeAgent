// <copyright file="StockItems.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class StockItems
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    public StockItems(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<StockItem> CreateAsync(StockItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        StockItemRoot data = new() { StockItem = item };
        using JsonContent content = JsonContent.Create(data, options: SharedJsonOptions.SourceGenOptions);

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.PostAsync("/v2/stock_items", content);
        response.EnsureSuccessStatusCode();

        StockItemRoot? root = await response.Content.ReadFromJsonAsync<StockItemRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove("stock_items_all");

        return root?.StockItem ?? throw new InvalidOperationException("Failed to create stock item");
    }

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

    public async Task<StockItem> UpdateAsync(string id, StockItem item)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(item);

        StockItemRoot data = new() { StockItem = item };
        using JsonContent content = JsonContent.Create(data, options: SharedJsonOptions.SourceGenOptions);

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.PutAsync($"/v2/stock_items/{id}", content);
        response.EnsureSuccessStatusCode();

        StockItemRoot? root = await response.Content.ReadFromJsonAsync<StockItemRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove($"stock_item_{id}");
        this.cache.Remove("stock_items_all");

        return root?.StockItem ?? throw new InvalidOperationException("Failed to update stock item");
    }

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
