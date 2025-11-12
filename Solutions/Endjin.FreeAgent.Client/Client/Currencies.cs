// <copyright file="Currencies.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class Currencies
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    public Currencies(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

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