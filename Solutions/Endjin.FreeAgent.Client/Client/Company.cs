// <copyright file="Company.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class Company
{
    private const string CompanyEndPoint = "v2/company";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    public Company(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(10));
    }

    public async Task<Domain.Company> GetAsync()
    {
        string cacheKey = CompanyEndPoint;

        if (!this.cache.TryGetValue(cacheKey, out Domain.Company? results))
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(
                new Uri(freeAgentClient.ApiBaseUrl, CompanyEndPoint)).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            CompanyRoot? root = await response.Content.ReadFromJsonAsync<CompanyRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            results = root?.Company;
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? throw new InvalidOperationException("Failed to retrieve company information.");
    }

    public async Task<Domain.Company> UpdateAsync(Domain.Company company)
    {
        CompanyRoot root = new() { Company = company };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, CompanyEndPoint),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        CompanyRoot? result = await response.Content.ReadFromJsonAsync<CompanyRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        this.cache.Remove(CompanyEndPoint);

        return result?.Company ?? throw new InvalidOperationException("Failed to deserialize company response.");
    }
}