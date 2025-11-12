// <copyright file="CapitalAssets.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class CapitalAssets
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    public CapitalAssets(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<CapitalAsset> CreateAsync(CapitalAsset asset)
    {
        ArgumentNullException.ThrowIfNull(asset);

        await this.client.InitializeAndAuthorizeAsync();

        CapitalAssetRoot data = new() { CapitalAsset = asset };
        using JsonContent content = JsonContent.Create(data, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.client.HttpClient.PostAsync(
            new Uri(this.client.ApiBaseUrl, "/v2/capital_assets"),
            content);
        response.EnsureSuccessStatusCode();

        CapitalAssetRoot? root = await response.Content.ReadFromJsonAsync<CapitalAssetRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove("capital_assets_all");

        return root?.CapitalAsset ?? throw new InvalidOperationException("Failed to create capital asset");
    }

    public async Task<IEnumerable<CapitalAsset>> GetAllAsync()
    {
        string cacheKey = "capital_assets_all";
        
        if (this.cache.TryGetValue(cacheKey, out IEnumerable<CapitalAsset>? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(
            new Uri(this.client.ApiBaseUrl, "/v2/capital_assets"));
        response.EnsureSuccessStatusCode();

        CapitalAssetsRoot? root = await response.Content.ReadFromJsonAsync<CapitalAssetsRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        
        IEnumerable<CapitalAsset> assets = root?.CapitalAssets ?? [];
        
        this.cache.Set(cacheKey, assets, TimeSpan.FromMinutes(5));
        
        return assets;
    }

    public async Task<CapitalAsset> GetByIdAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        
        string cacheKey = $"capital_asset_{id}";
        
        if (this.cache.TryGetValue(cacheKey, out CapitalAsset? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, $"/v2/capital_assets/{id}"));
        response.EnsureSuccessStatusCode();

        CapitalAssetRoot? root = await response.Content.ReadFromJsonAsync<CapitalAssetRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        CapitalAsset? asset = (root?.CapitalAsset) ?? throw new InvalidOperationException($"Capital asset {id} not found");

        this.cache.Set(cacheKey, asset, TimeSpan.FromMinutes(5));
        
        return asset;
    }

    public async Task<CapitalAsset> UpdateAsync(string id, CapitalAsset asset)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(asset);

        await this.client.InitializeAndAuthorizeAsync();

        CapitalAssetRoot data = new() { CapitalAsset = asset };
        using JsonContent content = JsonContent.Create(data, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.client.HttpClient.PutAsync(
            new Uri(this.client.ApiBaseUrl, $"/v2/capital_assets/{id}"),
            content);
        response.EnsureSuccessStatusCode();

        CapitalAssetRoot? root = await response.Content.ReadFromJsonAsync<CapitalAssetRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove($"capital_asset_{id}");
        this.cache.Remove("capital_assets_all");

        return root?.CapitalAsset ?? throw new InvalidOperationException("Failed to update capital asset");
    }

    public async Task DeleteAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.DeleteAsync(
            new Uri(this.client.ApiBaseUrl, $"/v2/capital_assets/{id}"));
        response.EnsureSuccessStatusCode();

        this.cache.Remove($"capital_asset_{id}");
        this.cache.Remove("capital_assets_all");
    }

    public async Task<IEnumerable<CapitalAssetType>> GetTypesAsync()
    {
        string cacheKey = "capital_asset_types";
        
        if (this.cache.TryGetValue(cacheKey, out IEnumerable<CapitalAssetType>? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(
            new Uri(this.client.ApiBaseUrl, "/v2/capital_asset_types"));
        response.EnsureSuccessStatusCode();

        CapitalAssetTypesRoot? root = await response.Content.ReadFromJsonAsync<CapitalAssetTypesRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        
        IEnumerable<CapitalAssetType> types = root?.CapitalAssetTypes ?? [];
        
        this.cache.Set(cacheKey, types, TimeSpan.FromHours(1)); // Cache longer as types rarely change
        
        return types;
    }
}