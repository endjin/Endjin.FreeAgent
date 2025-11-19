// <copyright file="EcMossSalesTaxRates.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for accessing EC MOSS (Mini One Stop Shop) sales tax rates via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides read-only access to EU VAT rates for the Mini One Stop Shop scheme,
/// which applies to cross-border digital services within the European Union. The rates vary by
/// EU member state and are used when selling digital services to consumers in different countries.
/// </para>
/// <para>
/// Results are cached for 24 hours per country/date combination as VAT rates rarely change.
/// </para>
/// <para>
/// Supported countries: Austria, Belgium, Bulgaria, Croatia, Cyprus, Czech Republic, Denmark,
/// Estonia, Finland, France, Germany, Greece, Hungary, Ireland, Italy, Latvia, Lithuania,
/// Luxembourg, Malta, Netherlands, Poland, Portugal, Romania, Slovakia, Slovenia, Spain, Sweden.
/// </para>
/// </remarks>
/// <seealso cref="EcMossSalesTaxRate"/>
/// <seealso cref="SalesTaxRate"/>
public class EcMossSalesTaxRates
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="EcMossSalesTaxRates"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing EC MOSS sales tax rate data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public EcMossSalesTaxRates(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Retrieves EC MOSS sales tax rates for a specific EU member state and date.
    /// </summary>
    /// <param name="country">The EU member state name (e.g., "Austria", "Germany", "France").</param>
    /// <param name="date">The transaction date for which to retrieve the applicable tax rates.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="EcMossSalesTaxRate"/> objects for the specified country and date.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="country"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// <para>
    /// This method calls GET /v2/ec_moss/sales_tax_rates?country={country}&amp;date={date} and caches
    /// the result for 24 hours per country/date combination.
    /// </para>
    /// <para>
    /// Minimum Access Level: Read Only
    /// </para>
    /// </remarks>
    public async Task<IEnumerable<EcMossSalesTaxRate>> GetAsync(string country, DateOnly date)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(country);

        await this.client.InitializeAndAuthorizeAsync();

        string dateString = date.ToString("yyyy-MM-dd");
        string cacheKey = $"ec_moss_sales_tax_rates_{country}_{dateString}";

        if (this.cache.TryGetValue(cacheKey, out IEnumerable<EcMossSalesTaxRate>? cached))
        {
            return cached!;
        }

        string encodedCountry = Uri.EscapeDataString(country);
        HttpResponseMessage response = await this.client.HttpClient.GetAsync(
            new Uri(this.client.ApiBaseUrl, $"/v2/ec_moss/sales_tax_rates?country={encodedCountry}&date={dateString}"));
        response.EnsureSuccessStatusCode();

        EcMossSalesTaxRatesRoot? root = await response.Content.ReadFromJsonAsync<EcMossSalesTaxRatesRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        IEnumerable<EcMossSalesTaxRate> rates = root?.SalesTaxRates ?? [];

        this.cache.Set(cacheKey, rates, TimeSpan.FromHours(24));

        return rates;
    }
}