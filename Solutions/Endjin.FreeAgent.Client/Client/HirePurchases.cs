// <copyright file="HirePurchases.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for accessing hire purchase records via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides read-only access to FreeAgent hire purchase records, which represent
/// financing arrangements for purchasing assets. Hire purchases are automatically created and managed
/// through the Bills API when a bill is set up with hire purchase financing.
/// </para>
/// <para>
/// This endpoint is only available for UK companies and requires minimum "Bills" access level.
/// </para>
/// <para>
/// Results from GetAllAsync are cached for 5 minutes to reduce API calls for repeated requests.
/// </para>
/// </remarks>
/// <seealso cref="HirePurchase"/>
/// <seealso cref="Bill"/>
public class HirePurchases
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="HirePurchases"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing hire purchase data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public HirePurchases(FreeAgentClient client, IMemoryCache cache)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(cache);

        this.client = client;
        this.cache = cache;
    }

    /// <summary>
    /// Retrieves all hire purchases from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// all <see cref="HirePurchase"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// <para>
    /// This method calls GET /v2/hire_purchases and caches the result for 5 minutes.
    /// </para>
    /// <para>
    /// This endpoint is only available for UK companies. Non-UK companies will receive an error response.
    /// </para>
    /// </remarks>
    public async Task<IEnumerable<HirePurchase>> GetAllAsync()
    {
        await this.client.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        string cacheKey = "hire_purchases_all";

        if (this.cache.TryGetValue(cacheKey, out IEnumerable<HirePurchase>? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, "/v2/hire_purchases")).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        HirePurchasesRoot? root = await response.Content.ReadFromJsonAsync<HirePurchasesRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        IEnumerable<HirePurchase> hirePurchases = root?.HirePurchases ?? [];

        this.cache.Set(cacheKey, hirePurchases, TimeSpan.FromMinutes(5));

        return hirePurchases;
    }

    /// <summary>
    /// Retrieves a specific hire purchase by its ID.
    /// </summary>
    /// <param name="id">The ID of the hire purchase to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="HirePurchase"/> object.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// <para>
    /// This method calls GET /v2/hire_purchases/:id.
    /// </para>
    /// <para>
    /// This endpoint is only available for UK companies. Non-UK companies will receive an error response.
    /// </para>
    /// </remarks>
    public async Task<HirePurchase> GetByIdAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        await this.client.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        string cacheKey = $"hire_purchase_{id}";

        if (this.cache.TryGetValue(cacheKey, out HirePurchase? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(
            new Uri(this.client.ApiBaseUrl, $"/v2/hire_purchases/{id}")).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        HirePurchaseRoot? root = await response.Content.ReadFromJsonAsync<HirePurchaseRoot>(
            SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        HirePurchase hirePurchase = root?.HirePurchase ?? throw new InvalidOperationException("Response did not contain a hire purchase.");

        this.cache.Set(cacheKey, hirePurchase, TimeSpan.FromMinutes(5));

        return hirePurchase;
    }
}
