// <copyright file="SalesTaxRates.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for accessing sales tax rates via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides read-only access to FreeAgent sales tax rates (e.g., VAT, GST),
/// which define the tax percentages applied to sales transactions. Sales tax rates are used
/// when creating invoices, credit notes, and estimates to calculate the correct tax amounts.
/// </para>
/// <para>
/// Results are cached for 24 hours as sales tax rates rarely change.
/// </para>
/// </remarks>
/// <seealso cref="SalesTaxRate"/>
/// <seealso cref="Invoice"/>
/// <seealso cref="CreditNote"/>
public class SalesTaxRates
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="SalesTaxRates"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing sales tax rate data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public SalesTaxRates(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Retrieves all sales tax rates from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// all <see cref="SalesTaxRate"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/sales_tax_rates and caches the result for 24 hours, as sales
    /// tax rates rarely change during typical usage.
    /// </remarks>
    public async Task<IEnumerable<SalesTaxRate>> GetAllAsync()
    {
        await this.client.InitializeAndAuthorizeAsync();

        string cacheKey = "sales_tax_rates_all";

        if (this.cache.TryGetValue(cacheKey, out IEnumerable<SalesTaxRate>? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, "/v2/sales_tax_rates"));
        response.EnsureSuccessStatusCode();

        SalesTaxRatesRoot? root = await response.Content.ReadFromJsonAsync<SalesTaxRatesRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        IEnumerable<SalesTaxRate> rates = root?.SalesTaxRates ?? [];

        this.cache.Set(cacheKey, rates, TimeSpan.FromHours(24));

        return rates;
    }
}