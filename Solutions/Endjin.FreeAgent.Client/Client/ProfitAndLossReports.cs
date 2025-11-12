// <copyright file="ProfitAndLossReports.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class ProfitAndLossReports
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    public ProfitAndLossReports(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<ProfitAndLoss> GetAsync(DateOnly fromDate, DateOnly toDate)
    {
        await this.client.InitializeAndAuthorizeAsync();

        string url = $"/v2/profit_and_loss?from_date={fromDate:yyyy-MM-dd}&to_date={toDate:yyyy-MM-dd}";
        string cacheKey = $"profit_loss_{fromDate:yyyy-MM-dd}_{toDate:yyyy-MM-dd}";
        
        if (this.cache.TryGetValue(cacheKey, out ProfitAndLoss? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, url));
        response.EnsureSuccessStatusCode();
        
        ProfitAndLossRoot? root = await response.Content.ReadFromJsonAsync<ProfitAndLossRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        
        ProfitAndLoss? profitAndLoss = root?.ProfitAndLoss;
        
        if (profitAndLoss == null)
        {
            throw new InvalidOperationException("Failed to retrieve profit and loss report");
        }
        
        this.cache.Set(cacheKey, profitAndLoss, TimeSpan.FromMinutes(30));
        
        return profitAndLoss;
    }
}
