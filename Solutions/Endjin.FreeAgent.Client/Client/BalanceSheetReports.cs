// <copyright file="BalanceSheetReports.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class BalanceSheetReports
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    public BalanceSheetReports(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<BalanceSheet> GetAsync(DateOnly? date = null)
    {
        await this.client.InitializeAndAuthorizeAsync();

        string url = "/v2/balance_sheet";
        if (date.HasValue)
        {
            url += $"?date={date.Value:yyyy-MM-dd}";
        }

        string cacheKey = $"balance_sheet_{date?.ToString("yyyy-MM-dd") ?? "current"}";
        
        if (this.cache.TryGetValue(cacheKey, out BalanceSheet? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, url));
        response.EnsureSuccessStatusCode();
        
        BalanceSheetRoot? root = await response.Content.ReadFromJsonAsync<BalanceSheetRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        BalanceSheet? balanceSheet = (root?.BalanceSheet) ?? throw new InvalidOperationException("Failed to retrieve balance sheet");

        this.cache.Set(cacheKey, balanceSheet, TimeSpan.FromMinutes(30));
        
        return balanceSheet;
    }
}