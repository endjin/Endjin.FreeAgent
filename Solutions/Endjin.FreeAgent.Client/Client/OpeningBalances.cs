// <copyright file="OpeningBalances.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class OpeningBalances
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    public OpeningBalances(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

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
