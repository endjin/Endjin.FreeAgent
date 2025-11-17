// <copyright file="CisBands.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for accessing Construction Industry Scheme (CIS) deduction bands via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides read-only access to CIS bands, which define the tax deduction rates
/// that apply to subcontractors under the UK Construction Industry Scheme. Each band specifies a
/// deduction rate (0%, 20%, or 30%) and corresponding accounting nominal codes.
/// </para>
/// <para>
/// CIS bands are read-only reference data that changes infrequently. Results are cached for 24 hours
/// to minimize API calls, as recommended by FreeAgent. The cache improves performance when repeatedly
/// accessing the available CIS bands.
/// </para>
/// <para>
/// This endpoint is only available to UK companies enrolled in the Construction Industry Scheme for
/// Subcontractors and requires "Estimates &amp; Invoices" access level.
/// </para>
/// <para>
/// API Endpoint: /v2/cis_bands
/// </para>
/// </remarks>
/// <seealso cref="CisBand"/>
/// <seealso cref="CisBandsResponse"/>
/// <seealso cref="Bill"/>
/// <seealso cref="Invoice"/>
public class CisBands
{
    private const string CisBandsEndPoint = "v2/cis_bands";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CisBands"/> class.
    /// </summary>
    /// <param name="freeAgentClient">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing CIS bands data.</param>
    public CisBands(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromHours(24)); // CIS bands rarely change
    }

    /// <summary>
    /// Retrieves all available Construction Industry Scheme deduction bands from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="CisBand"/> objects representing the available CIS deduction bands.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// <para>
    /// This method calls GET /v2/cis_bands and caches the result for 24 hours. The standard bands include:
    /// <list type="bullet">
    /// <item><strong>cis_gross</strong> - No deduction (0% rate, nominal code 061)</item>
    /// <item><strong>cis_standard</strong> - Standard rate (20% deduction, nominal code 062)</item>
    /// <item><strong>cis_higher</strong> - Higher rate (30% deduction, nominal code 063)</item>
    /// </list>
    /// </para>
    /// <para>
    /// CIS bands are read-only reference data and cannot be created, modified, or deleted via the API.
    /// The bands define the deduction rates and accounting codes used when processing subcontractor
    /// invoices and bills under the UK Construction Industry Scheme.
    /// </para>
    /// <para>
    /// This endpoint is only available to UK companies enrolled in CIS for Subcontractors and requires
    /// "Estimates &amp; Invoices" access level. If your company is not enrolled in CIS, this method will
    /// return an error.
    /// </para>
    /// </remarks>
    public async Task<IEnumerable<CisBand>> GetAvailableBandsAsync()
    {
        string cacheKey = CisBandsEndPoint;

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<CisBand>? results))
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(new Uri(freeAgentClient.ApiBaseUrl, CisBandsEndPoint)).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            CisBandsResponse? cisBandsResponse = await response.Content.ReadFromJsonAsync<CisBandsResponse>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            results = cisBandsResponse?.AvailableBands ?? [];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }
}
