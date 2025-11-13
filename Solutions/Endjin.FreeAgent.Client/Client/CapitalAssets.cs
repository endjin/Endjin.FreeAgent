// <copyright file="CapitalAssets.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing capital assets via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent capital assets, which are long-term business assets
/// such as equipment, vehicles, and property. Capital assets are tracked for depreciation and accounting
/// purposes, and are associated with capital asset types and depreciation profiles.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance. Cache entries are invalidated automatically
/// when capital assets are created, updated, or deleted.
/// </para>
/// </remarks>
/// <seealso cref="CapitalAsset"/>
/// <seealso cref="CapitalAssetType"/>
/// <seealso cref="DepreciationProfile"/>
public class CapitalAssets
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="CapitalAssets"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing capital asset data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public CapitalAssets(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Creates a new capital asset in FreeAgent.
    /// </summary>
    /// <param name="asset">The <see cref="CapitalAsset"/> object containing the capital asset details to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="CapitalAsset"/> object with server-assigned values (e.g., ID, URL).
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="asset"/> is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/capital_assets to create a new capital asset. The cache is invalidated
    /// to ensure subsequent queries return up-to-date data.
    /// </remarks>
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

    /// <summary>
    /// Retrieves all capital assets from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// all <see cref="CapitalAsset"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/capital_assets and caches the result for 5 minutes.
    /// </remarks>
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

    /// <summary>
    /// Retrieves a specific capital asset by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the capital asset to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="CapitalAsset"/> object with the specified ID.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no capital asset with the specified ID is found.</exception>
    /// <remarks>
    /// This method calls GET /v2/capital_assets/{id} and caches the result for 5 minutes.
    /// </remarks>
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

    /// <summary>
    /// Updates an existing capital asset in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the capital asset to update.</param>
    /// <param name="asset">The <see cref="CapitalAsset"/> object containing the updated capital asset details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="CapitalAsset"/> object as returned by the API.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="asset"/> is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/capital_assets/{id} to update the capital asset. The cache entries for
    /// this capital asset and all capital asset queries are invalidated after a successful update.
    /// </remarks>
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

    /// <summary>
    /// Deletes a capital asset from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the capital asset to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls DELETE /v2/capital_assets/{id} to delete the capital asset. The cache entries
    /// for this capital asset and all capital asset queries are invalidated after successful deletion.
    /// </remarks>
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

    /// <summary>
    /// Retrieves all capital asset types from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// all <see cref="CapitalAssetType"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/capital_asset_types and caches the result for 1 hour, as asset types
    /// rarely change.
    /// </remarks>
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