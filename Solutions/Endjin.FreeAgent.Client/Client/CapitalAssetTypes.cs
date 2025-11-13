// <copyright file="CapitalAssetTypes.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for accessing capital asset types via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides read-only access to FreeAgent capital asset types, which define
/// the categories of capital assets (e.g., vehicles, equipment, property). Asset types determine
/// how assets are classified for accounting and tax purposes.
/// </para>
/// <para>
/// Results are cached for 24 hours as capital asset types rarely change.
/// </para>
/// </remarks>
/// <seealso cref="CapitalAssetType"/>
/// <seealso cref="CapitalAsset"/>
public class CapitalAssetTypes
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="CapitalAssetTypes"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing capital asset type data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public CapitalAssetTypes(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
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
    /// This method calls GET /v2/capital_asset_types and caches the result for 24 hours, as capital
    /// asset types rarely change during typical usage.
    /// </remarks>
    public async Task<IEnumerable<CapitalAssetType>> GetAllAsync()
    {
        await this.client.InitializeAndAuthorizeAsync();

        string cacheKey = "capital_asset_types_all";

        if (this.cache.TryGetValue(cacheKey, out IEnumerable<CapitalAssetType>? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, "/v2/capital_asset_types"));
        response.EnsureSuccessStatusCode();

        CapitalAssetTypesRoot? root = await response.Content.ReadFromJsonAsync<CapitalAssetTypesRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        IEnumerable<CapitalAssetType> types = root?.CapitalAssetTypes ?? [];

        this.cache.Set(cacheKey, types, TimeSpan.FromHours(24));

        return types;
    }
}