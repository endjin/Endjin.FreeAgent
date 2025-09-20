// <copyright file="SalesTaxRates.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class SalesTaxRates
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    public SalesTaxRates(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

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
