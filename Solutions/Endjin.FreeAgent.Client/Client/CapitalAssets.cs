// <copyright file="CapitalAssets.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides read-only methods for retrieving capital assets via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides read-only access to FreeAgent capital assets, which are long-term business
/// assets such as equipment, vehicles, and property. Capital assets are tracked for depreciation and accounting
/// purposes, and are associated with capital asset types and depreciation profiles.
/// </para>
/// <para>
/// NOTE: The documented FreeAgent Capital Assets API endpoints are read-only (GET operations only).
/// Create, update, and delete operations are not documented in the official API specification.
/// Assets may need to be managed through the FreeAgent web interface.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance.
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
    /// Retrieves all capital assets from FreeAgent.
    /// </summary>
    /// <param name="view">Optional filter for asset status: "all", "disposed", or "disposable". If not specified, returns all assets.</param>
    /// <param name="includeHistory">If <c>true</c>, includes the lifecycle event history for each asset. Defaults to <c>false</c>.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// all <see cref="CapitalAsset"/> objects matching the filter criteria.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/capital_assets with optional query parameters and caches the result for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<CapitalAsset>> GetAllAsync(string? view = null, bool includeHistory = false)
    {
        string cacheKey = $"capital_assets_view_{view ?? "all"}_history_{includeHistory}";

        if (this.cache.TryGetValue(cacheKey, out IEnumerable<CapitalAsset>? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync();

        string url = "/v2/capital_assets";
        List<string> queryParams = [];

        if (!string.IsNullOrWhiteSpace(view))
        {
            queryParams.Add($"view={Uri.EscapeDataString(view)}");
        }

        if (includeHistory)
        {
            queryParams.Add("include_history=true");
        }

        if (queryParams.Count > 0)
        {
            url += "?" + string.Join("&", queryParams);
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, url));
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
    /// <param name="includeHistory">If <c>true</c>, includes the lifecycle event history for the asset. Defaults to <c>false</c>.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="CapitalAsset"/> object with the specified ID.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no capital asset with the specified ID is found.</exception>
    /// <remarks>
    /// This method calls GET /v2/capital_assets/{id} with optional query parameters and caches the result for 5 minutes.
    /// </remarks>
    public async Task<CapitalAsset> GetByIdAsync(string id, bool includeHistory = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        string cacheKey = $"capital_asset_{id}_history_{includeHistory}";

        if (this.cache.TryGetValue(cacheKey, out CapitalAsset? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync();

        string url = $"/v2/capital_assets/{id}";
        if (includeHistory)
        {
            url += "?include_history=true";
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, url));
        response.EnsureSuccessStatusCode();

        CapitalAssetRoot? root = await response.Content.ReadFromJsonAsync<CapitalAssetRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        CapitalAsset? asset = (root?.CapitalAsset) ?? throw new InvalidOperationException($"Capital asset {id} not found");

        this.cache.Set(cacheKey, asset, TimeSpan.FromMinutes(5));

        return asset;
    }
}