// <copyright file="Currencies.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for accessing currency information via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides read-only access to the list of currencies supported by FreeAgent.
/// Currency data is used when creating multi-currency invoices, bills, and bank accounts.
/// </para>
/// <para>
/// Results are cached for 24 hours as the list of supported currencies is static and rarely changes.
/// </para>
/// </remarks>
/// <seealso cref="Currency"/>
public class Currencies
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="Currencies"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing currency data.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="client"/> or <paramref name="cache"/> is <see langword="null"/>.
    /// </exception>
    public Currencies(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Retrieves all supported currencies from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="Currency"/> objects representing all supported currencies.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/currencies and caches the result for 24 hours. Each currency includes
    /// the currency code (e.g., "GBP", "USD", "EUR") and symbol (e.g., "£", "$", "€").
    /// </remarks>
    public async Task<IEnumerable<Currency>> GetAllAsync()
    {
        await this.client.InitializeAndAuthorizeAsync();

        string cacheKey = "currencies_all";

        if (this.cache.TryGetValue(cacheKey, out IEnumerable<Currency>? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, "/v2/currencies"));
        response.EnsureSuccessStatusCode();

        CurrenciesRoot? root = await response.Content.ReadFromJsonAsync<CurrenciesRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        IEnumerable<Currency> currencies = root?.Currencies ?? [];

        this.cache.Set(cacheKey, currencies, TimeSpan.FromHours(24));

        return currencies;
    }
}