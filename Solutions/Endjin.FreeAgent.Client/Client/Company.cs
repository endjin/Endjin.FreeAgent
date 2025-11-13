// <copyright file="Company.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for accessing company information via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to the authenticated company's profile and settings in FreeAgent.
/// The company endpoint (GET /v2/company) returns information about the organization using the API,
/// including business details, registration information, VAT settings, and accounting configuration.
/// </para>
/// <para>
/// Results are cached for 10 minutes to improve performance and reduce API calls, as company information
/// rarely changes during typical usage.
/// </para>
/// </remarks>
/// <seealso cref="Domain.Company"/>
public class Company
{
    private const string CompanyEndPoint = "v2/company";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Company"/> class.
    /// </summary>
    /// <param name="freeAgentClient">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing company information.</param>
    public Company(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(10));
    }

    /// <summary>
    /// Retrieves the authenticated company's information from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="Domain.Company"/> object with the company's profile and settings.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls GET /v2/company and caches the result for 10 minutes. Cached results are
    /// returned on subsequent calls within the cache window to improve performance.
    /// </remarks>
    public async Task<Domain.Company> GetAsync()
    {
        string cacheKey = CompanyEndPoint;

        if (!this.cache.TryGetValue(cacheKey, out Domain.Company? results))
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(
                new Uri(freeAgentClient.ApiBaseUrl, CompanyEndPoint)).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            CompanyRoot? root = await response.Content.ReadFromJsonAsync<CompanyRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            results = root?.Company;
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? throw new InvalidOperationException("Failed to retrieve company information.");
    }

    /// <summary>
    /// Updates the authenticated company's information in FreeAgent.
    /// </summary>
    /// <param name="company">The <see cref="Domain.Company"/> object containing the updated company details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="Domain.Company"/> object as returned by the API.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/company to update company information. The cache is automatically
    /// invalidated after a successful update to ensure subsequent calls to <see cref="GetAsync"/>
    /// retrieve the updated information from the API.
    /// </remarks>
    public async Task<Domain.Company> UpdateAsync(Domain.Company company)
    {
        CompanyRoot root = new() { Company = company };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, CompanyEndPoint),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        CompanyRoot? result = await response.Content.ReadFromJsonAsync<CompanyRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        this.cache.Remove(CompanyEndPoint);

        return result?.Company ?? throw new InvalidOperationException("Failed to deserialize company response.");
    }
}