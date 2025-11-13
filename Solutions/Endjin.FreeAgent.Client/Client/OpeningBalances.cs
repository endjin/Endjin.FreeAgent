// <copyright file="OpeningBalances.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing opening balances via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent opening balances, which represent the initial
/// financial position when a business starts using FreeAgent. Opening balances include bank account
/// balances, outstanding invoices, unpaid bills, fixed assets, and other account balances at the
/// point of migration to FreeAgent. This service supports retrieving and updating opening balance data.
/// </para>
/// <para>
/// Opening balance data is cached for 1 hour to improve performance, as these values change very
/// infrequently and typically only during initial system setup.
/// </para>
/// </remarks>
/// <seealso cref="OpeningBalance"/>
/// <seealso cref="BankAccount"/>
/// <seealso cref="Company"/>
public class OpeningBalances
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpeningBalances"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing opening balance data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public OpeningBalances(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Retrieves the opening balance data from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="OpeningBalance"/> object with all initial financial position data.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls GET /v2/opening_balances and caches the result for 1 hour. Opening balances
    /// represent the starting financial state when migrating to FreeAgent.
    /// </remarks>
    public async Task<OpeningBalance> GetAsync()
    {
        string cacheKey = "opening_balance";
        
        if (this.cache.TryGetValue(cacheKey, out OpeningBalance? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, "/v2/opening_balances"));
        response.EnsureSuccessStatusCode();
        
        OpeningBalanceRoot? root = await response.Content.ReadFromJsonAsync<OpeningBalanceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        
        OpeningBalance? openingBalance = root?.OpeningBalance;
        
        if (openingBalance == null)
        {
            throw new InvalidOperationException("Failed to retrieve opening balance");
        }
        
        this.cache.Set(cacheKey, openingBalance, TimeSpan.FromHours(1));
        
        return openingBalance;
    }

    /// <summary>
    /// Updates the opening balance data in FreeAgent.
    /// </summary>
    /// <param name="openingBalance">The <see cref="OpeningBalance"/> object containing the updated opening balance data.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="OpeningBalance"/> object.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="openingBalance"/> is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/opening_balances and invalidates the cache entry. Opening balances are
    /// typically set once during initial system configuration and rarely modified thereafter.
    /// </remarks>
    public async Task<OpeningBalance> UpdateAsync(OpeningBalance openingBalance)
    {
        ArgumentNullException.ThrowIfNull(openingBalance);

        await this.client.InitializeAndAuthorizeAsync();

        OpeningBalanceRoot data = new() { OpeningBalance = openingBalance };
        using JsonContent content = JsonContent.Create(data, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.client.HttpClient.PutAsync(new Uri(this.client.ApiBaseUrl, "/v2/opening_balances"), content);
        response.EnsureSuccessStatusCode();

        OpeningBalanceRoot? root = await response.Content.ReadFromJsonAsync<OpeningBalanceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove("opening_balance");

        return root?.OpeningBalance ?? throw new InvalidOperationException("Failed to update opening balance");
    }
}
