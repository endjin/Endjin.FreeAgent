// <copyright file="TrialBalances.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class TrialBalances
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    public TrialBalances(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<TrialBalance> GetAsync(DateOnly? date = null)
    {
        await this.client.InitializeAndAuthorizeAsync();

        string url = "/v2/trial_balance";
        if (date.HasValue)
        {
            url += $"?date={date.Value:yyyy-MM-dd}";
        }

        string cacheKey = $"trial_balance_{date?.ToString("yyyy-MM-dd") ?? "current"}";
        
        if (this.cache.TryGetValue(cacheKey, out TrialBalance? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, url));
        response.EnsureSuccessStatusCode();
        
        TrialBalanceRoot? root = await response.Content.ReadFromJsonAsync<TrialBalanceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        
        TrialBalance? trialBalance = root?.TrialBalance;
        
        if (trialBalance == null)
        {
            throw new InvalidOperationException("Failed to retrieve trial balance");
        }
        
        this.cache.Set(cacheKey, trialBalance, TimeSpan.FromMinutes(15));
        
        return trialBalance;
    }
}
