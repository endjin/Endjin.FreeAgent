// <copyright file="AgedDebtorsAndCreditors.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class AgedDebtorsAndCreditors
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    public AgedDebtorsAndCreditors(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<SalesAgedDebtors> GetSalesAgedDebtorsAsync(DateOnly? date = null)
    {
        await this.client.InitializeAndAuthorizeAsync();

        string url = "/v2/sales_aged_debtors";
        if (date.HasValue)
        {
            url += $"?date={date.Value:yyyy-MM-dd}";
        }

        string cacheKey = $"sales_aged_debtors_{date?.ToString("yyyy-MM-dd") ?? "current"}";
        
        if (this.cache.TryGetValue(cacheKey, out SalesAgedDebtors? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, url));
        response.EnsureSuccessStatusCode();
        
        SalesAgedDebtorsRoot? root = await response.Content.ReadFromJsonAsync<SalesAgedDebtorsRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        
        SalesAgedDebtors? debtors = (root?.SalesAgedDebtors) ?? throw new InvalidOperationException("Failed to retrieve sales aged debtors");
        this.cache.Set(cacheKey, debtors, TimeSpan.FromMinutes(30));
        
        return debtors;
    }

    public async Task<PurchaseAgedCreditors> GetPurchaseAgedCreditorsAsync(DateOnly? date = null)
    {
        await this.client.InitializeAndAuthorizeAsync();

        string url = "/v2/purchase_aged_creditors";
        if (date.HasValue)
        {
            url += $"?date={date.Value:yyyy-MM-dd}";
        }

        string cacheKey = $"purchase_aged_creditors_{date?.ToString("yyyy-MM-dd") ?? "current"}";
        
        if (this.cache.TryGetValue(cacheKey, out PurchaseAgedCreditors? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, url));
        response.EnsureSuccessStatusCode();
        
        PurchaseAgedCreditorsRoot? root = await response.Content.ReadFromJsonAsync<PurchaseAgedCreditorsRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        PurchaseAgedCreditors? creditors = (root?.PurchaseAgedCreditors) ?? throw new InvalidOperationException("Failed to retrieve purchase aged creditors");

        this.cache.Set(cacheKey, creditors, TimeSpan.FromMinutes(30));
        
        return creditors;
    }
}