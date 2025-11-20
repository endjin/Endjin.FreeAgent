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
    private const string BusinessCategoriesEndPoint = "v2/company/business_categories";
    private const string TaxTimelineEndPoint = "v2/company/tax_timeline";
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
    /// <para>
    /// This method calls GET /v2/company and caches the result for 10 minutes. Cached results are
    /// returned on subsequent calls within the cache window to improve performance.
    /// </para>
    /// <para>
    /// Minimum Access Level: Time
    /// </para>
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
    /// Retrieves the list of valid business categories from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a
    /// list of valid business category names that can be assigned to a company.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// <para>
    /// This method calls GET /v2/company/business_categories and returns the standardized list
    /// of business categories available in FreeAgent. These values can be used to set the
    /// business_category field on a company.
    /// </para>
    /// <para>
    /// Minimum Access Level: Time
    /// </para>
    /// </remarks>
    public async Task<List<string>> GetBusinessCategoriesAsync()
    {
        string cacheKey = BusinessCategoriesEndPoint;

        if (!this.cache.TryGetValue(cacheKey, out List<string>? results))
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(
                new Uri(freeAgentClient.ApiBaseUrl, BusinessCategoriesEndPoint)).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            BusinessCategoriesRoot? root = await response.Content.ReadFromJsonAsync<BusinessCategoriesRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            results = root?.BusinessCategories ?? [];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves the tax timeline with upcoming tax events and deadlines from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a
    /// list of <see cref="TaxTimelineItem"/> objects representing upcoming tax obligations.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// <para>
    /// This method calls GET /v2/company/tax_timeline and returns information about upcoming
    /// tax events including VAT returns, Companies House filings, Corporation Tax deadlines,
    /// and other compliance obligations.
    /// </para>
    /// <para>
    /// Note: Unlike company details and business categories, tax timeline data is not cached
    /// because it changes frequently as deadlines approach and new obligations are added.
    /// Each call retrieves the most current tax timeline information.
    /// </para>
    /// <para>
    /// Minimum Access Level: Tax, Accounting and Users
    /// </para>
    /// </remarks>
    public async Task<List<TaxTimelineItem>> GetTaxTimelineAsync()
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(
            new Uri(freeAgentClient.ApiBaseUrl, TaxTimelineEndPoint)).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        TaxTimelineRoot? root = await response.Content.ReadFromJsonAsync<TaxTimelineRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return root?.TimelineItems ?? [];
    }
}