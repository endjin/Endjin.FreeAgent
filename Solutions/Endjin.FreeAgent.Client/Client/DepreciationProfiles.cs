// <copyright file="DepreciationProfiles.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class DepreciationProfiles
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    public DepreciationProfiles(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<IEnumerable<DepreciationProfile>> GetAllAsync()
    {
        await this.client.InitializeAndAuthorizeAsync();

        string cacheKey = "depreciation_profiles_all";

        if (this.cache.TryGetValue(cacheKey, out IEnumerable<DepreciationProfile>? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, "/v2/depreciation_profiles"));
        response.EnsureSuccessStatusCode();

        DepreciationProfilesRoot? root = await response.Content.ReadFromJsonAsync<DepreciationProfilesRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        IEnumerable<DepreciationProfile> profiles = root?.DepreciationProfiles ?? [];

        this.cache.Set(cacheKey, profiles, TimeSpan.FromHours(24));

        return profiles;
    }
}