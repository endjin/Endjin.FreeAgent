// <copyright file="CapitalAssetTypes.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class CapitalAssetTypes
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    public CapitalAssetTypes(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

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