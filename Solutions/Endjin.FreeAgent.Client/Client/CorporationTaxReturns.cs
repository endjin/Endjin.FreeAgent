// <copyright file="CorporationTaxReturns.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class CorporationTaxReturns
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    public CorporationTaxReturns(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<IEnumerable<CorporationTaxReturn>> GetAllAsync()
    {
        string cacheKey = "corporation_tax_returns_all";
        
        if (this.cache.TryGetValue(cacheKey, out IEnumerable<CorporationTaxReturn>? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, "/v2/corporation_tax_returns"));
        response.EnsureSuccessStatusCode();
        
        CorporationTaxReturnsRoot? root = await response.Content.ReadFromJsonAsync<CorporationTaxReturnsRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        
        IEnumerable<CorporationTaxReturn> returns = root?.CorporationTaxReturns ?? [];
        
        this.cache.Set(cacheKey, returns, TimeSpan.FromMinutes(5));
        
        return returns;
    }

    public async Task<CorporationTaxReturn> GetByIdAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        
        string cacheKey = $"corporation_tax_return_{id}";
        
        if (this.cache.TryGetValue(cacheKey, out CorporationTaxReturn? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, $"/v2/corporation_tax_returns/{id}"));
        response.EnsureSuccessStatusCode();
        
        CorporationTaxReturnRoot? root = await response.Content.ReadFromJsonAsync<CorporationTaxReturnRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        CorporationTaxReturn? taxReturn = (root?.CorporationTaxReturn) ?? throw new InvalidOperationException($"Corporation tax return {id} not found");

        this.cache.Set(cacheKey, taxReturn, TimeSpan.FromMinutes(5));
        
        return taxReturn;
    }

    public async Task<CorporationTaxReturn> MarkAsFiledAsync(
        string id,
        DateOnly filedOn,
        bool filedOnline = true,
        string? hmrcReference = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        CorporationTaxReturnFilingRoot data = new()
        {
            CorporationTaxReturn = new CorporationTaxReturnFiling
            {
                FiledOn = filedOn.ToString("yyyy-MM-dd"),
                FiledOnline = filedOnline,
                HmrcReference = hmrcReference
            }
        };

        using JsonContent content = JsonContent.Create(data, options: SharedJsonOptions.SourceGenOptions);

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.PutAsync($"/v2/corporation_tax_returns/{id}/mark_as_filed", content);
        response.EnsureSuccessStatusCode();

        CorporationTaxReturnRoot? root = await response.Content.ReadFromJsonAsync<CorporationTaxReturnRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove($"corporation_tax_return_{id}");
        this.cache.Remove("corporation_tax_returns_all");

        return root?.CorporationTaxReturn ?? throw new InvalidOperationException("Failed to mark corporation tax return as filed");
    }
}