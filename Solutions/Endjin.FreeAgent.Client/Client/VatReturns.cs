// <copyright file="VatReturns.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class VatReturns
{
    private const string VatReturnsEndPoint = "v2/vat_returns";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    public VatReturns(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    public async Task<IEnumerable<VatReturn>> GetAllAsync()
    {
        string cacheKey = VatReturnsEndPoint;

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<VatReturn>? results))
        {
            List<VatReturnsRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<VatReturnsRoot>(new Uri(this.freeAgentClient.ApiBaseUrl, VatReturnsEndPoint)).ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.VatReturns)];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    public async Task<VatReturn> GetByIdAsync(string id)
    {
        string cacheKey = $"{VatReturnsEndPoint}/{id}";

        if (!this.cache.TryGetValue(cacheKey, out VatReturn? results))
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(new Uri(freeAgentClient.ApiBaseUrl, $"{VatReturnsEndPoint}/{id}")).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            VatReturnRoot? root = await response.Content.ReadFromJsonAsync<VatReturnRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            results = root?.VatReturn;
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? throw new InvalidOperationException($"VAT return with ID {id} not found.");
    }

    public async Task<VatReturn> MarkAsFiledAsync(string id, DateOnly filedOn, bool filedOnline, string? hmrcReference = null)
    {
        VatReturnFilingRoot filingData = new()
        {
            VatReturn = new VatReturnFiling
            {
                FiledOn = filedOn.ToString("yyyy-MM-dd"),
                FiledOnline = filedOnline,
                HmrcReference = hmrcReference
            }
        };

        using JsonContent content = JsonContent.Create(filingData, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{VatReturnsEndPoint}/{id}/mark_as_filed"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        VatReturnRoot? root = await response.Content.ReadFromJsonAsync<VatReturnRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        string cacheKey = $"{VatReturnsEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return root?.VatReturn ?? throw new InvalidOperationException("Failed to deserialize VAT return response.");
    }
}
