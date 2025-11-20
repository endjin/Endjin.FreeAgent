// <copyright file="DepreciationProfiles.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for accessing depreciation profiles via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides read-only access to FreeAgent depreciation profiles, which define
/// how capital assets depreciate over time. Depreciation profiles specify the rate and method
/// (e.g., straight-line, reducing balance) used to calculate asset depreciation for accounting
/// and tax purposes.
/// </para>
/// <para>
/// Results are cached for 24 hours as depreciation profiles rarely change.
/// </para>
/// </remarks>
/// <seealso cref="DepreciationProfile"/>
/// <seealso cref="CapitalAsset"/>
public class DepreciationProfiles
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="DepreciationProfiles"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing depreciation profile data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public DepreciationProfiles(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Retrieves all depreciation profiles from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// all <see cref="DepreciationProfile"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/depreciation_profiles and caches the result for 24 hours, as
    /// depreciation profiles rarely change during typical usage.
    /// </remarks>
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