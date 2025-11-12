// <copyright file="CashFlowReports.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class CashFlowReports
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    public CashFlowReports(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<CashFlow> GetAsync(DateOnly fromDate, DateOnly toDate)
    {
        await this.client.InitializeAndAuthorizeAsync();

        string url = $"/v2/cash_flow?from_date={fromDate:yyyy-MM-dd}&to_date={toDate:yyyy-MM-dd}";
        string cacheKey = $"cash_flow_{fromDate:yyyy-MM-dd}_{toDate:yyyy-MM-dd}";
        
        if (this.cache.TryGetValue(cacheKey, out CashFlow? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, url));
        response.EnsureSuccessStatusCode();
        
        CashFlowRoot? root = await response.Content.ReadFromJsonAsync<CashFlowRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        CashFlow? cashFlow = (root?.CashFlow) ?? throw new InvalidOperationException("Failed to retrieve cash flow report");

        this.cache.Set(cacheKey, cashFlow, TimeSpan.FromMinutes(30));
        
        return cashFlow;
    }
}