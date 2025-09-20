// <copyright file="Categories.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class Categories
{
    private const string CategoriesEndPoint = "v2/categories";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    public Categories(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(30)); // Categories don't change often
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        string cacheKey = CategoriesEndPoint;

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<Category>? results))
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(new Uri(freeAgentClient.ApiBaseUrl, CategoriesEndPoint)).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            CategoriesRoot? root = await response.Content.ReadFromJsonAsync<CategoriesRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            results = root?.Categories ?? [];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    public async Task<Category> GetByNominalCodeAsync(string nominalCode)
    {
        IEnumerable<Category> categories = await GetAllAsync().ConfigureAwait(false);
        return categories.FirstOrDefault(c => c.NominalCode == nominalCode) ?? throw new InvalidOperationException($"Category with nominal code {nominalCode} not found.");
    }
}