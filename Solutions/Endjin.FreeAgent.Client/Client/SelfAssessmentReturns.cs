// <copyright file="SelfAssessmentReturns.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class SelfAssessmentReturns
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    public SelfAssessmentReturns(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<IEnumerable<SelfAssessmentReturn>> GetAllAsync(string? userId = null)
    {
        string url = "/v2/self_assessment_returns";
        if (!string.IsNullOrWhiteSpace(userId))
        {
            url += $"?user={userId}";
        }

        string cacheKey = $"self_assessment_returns_{userId ?? "all"}";
        
        if (this.cache.TryGetValue(cacheKey, out IEnumerable<SelfAssessmentReturn>? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        
        SelfAssessmentReturnsRoot? root = await response.Content.ReadFromJsonAsync<SelfAssessmentReturnsRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        
        IEnumerable<SelfAssessmentReturn> returns = root?.SelfAssessmentReturns ?? [];
        
        this.cache.Set(cacheKey, returns, TimeSpan.FromMinutes(5));
        
        return returns;
    }

    public async Task<SelfAssessmentReturn> GetByIdAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        
        string cacheKey = $"self_assessment_return_{id}";
        
        if (this.cache.TryGetValue(cacheKey, out SelfAssessmentReturn? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, $"/v2/self_assessment_returns/{id}"));
        response.EnsureSuccessStatusCode();
        
        SelfAssessmentReturnRoot? root = await response.Content.ReadFromJsonAsync<SelfAssessmentReturnRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        
        SelfAssessmentReturn? taxReturn = root?.SelfAssessmentReturn;
        
        if (taxReturn == null)
        {
            throw new InvalidOperationException($"Self assessment return {id} not found");
        }
        
        this.cache.Set(cacheKey, taxReturn, TimeSpan.FromMinutes(5));
        
        return taxReturn;
    }

    public async Task<SelfAssessmentReturn> MarkAsFiledAsync(string id, DateOnly filedOn, bool filedOnline = true, string? utrNumber = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        SelfAssessmentReturnFilingRoot data = new()
        {
            SelfAssessmentReturn = new SelfAssessmentReturnFiling
            {
                FiledOn = filedOn.ToString("yyyy-MM-dd"),
                FiledOnline = filedOnline,
                UtrNumber = utrNumber
            }
        };

        using JsonContent content = JsonContent.Create(data, options: SharedJsonOptions.SourceGenOptions);

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.PutAsync($"/v2/self_assessment_returns/{id}/mark_as_filed", content);
        response.EnsureSuccessStatusCode();

        SelfAssessmentReturnRoot? root = await response.Content.ReadFromJsonAsync<SelfAssessmentReturnRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove($"self_assessment_return_{id}");
        this.cache.Remove("self_assessment_returns_all");

        return root?.SelfAssessmentReturn ?? throw new InvalidOperationException("Failed to mark self assessment return as filed");
    }
}
