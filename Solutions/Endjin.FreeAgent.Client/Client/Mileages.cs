// <copyright file="Mileages.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class Mileages
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    public Mileages(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<Mileage> CreateAsync(Mileage mileage)
    {
        ArgumentNullException.ThrowIfNull(mileage);
        await this.client.InitializeAndAuthorizeAsync();

        MileageRoot data = new() { Mileage = mileage };
        using JsonContent content = JsonContent.Create(data, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.client.HttpClient.PostAsync(new Uri(this.client.ApiBaseUrl, "/v2/mileages"), content);
        response.EnsureSuccessStatusCode();

        MileageRoot? root = await response.Content.ReadFromJsonAsync<MileageRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove("mileages_all");

        return root?.Mileage ?? throw new InvalidOperationException("Failed to create mileage");
    }

    public async Task<IEnumerable<Mileage>> GetAllAsync(string? userId = null, DateOnly? fromDate = null, DateOnly? toDate = null)
    {
        await this.client.InitializeAndAuthorizeAsync();

        List<string> queryParams = [];
        if (!string.IsNullOrWhiteSpace(userId))
        {
            queryParams.Add($"user={userId}");
        }

        if (fromDate.HasValue)
        {
            queryParams.Add($"from_date={fromDate.Value:yyyy-MM-dd}");
        }

        if (toDate.HasValue)
        {
            queryParams.Add($"to_date={toDate.Value:yyyy-MM-dd}");
        }

        string url = "/v2/mileages";
        if (queryParams.Count > 0)
        {
            url += "?" + string.Join("&", queryParams);
        }

        string cacheKey = $"mileages_{userId ?? "all"}_{fromDate?.ToString("yyyyMMdd")}_{toDate?.ToString("yyyyMMdd")}";
        
        if (this.cache.TryGetValue(cacheKey, out IEnumerable<Mileage>? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(
            new Uri(this.client.ApiBaseUrl, url));
        response.EnsureSuccessStatusCode();
        
        MileagesRoot? root = await response.Content.ReadFromJsonAsync<MileagesRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        
        IEnumerable<Mileage> mileages = root?.Mileages ?? [];
        
        this.cache.Set(cacheKey, mileages, TimeSpan.FromMinutes(5));
        
        return mileages;
    }

    public async Task<Mileage> GetByIdAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        await this.client.InitializeAndAuthorizeAsync();
        
        string cacheKey = $"mileage_{id}";
        
        if (this.cache.TryGetValue(cacheKey, out Mileage? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, $"/v2/mileages/{id}"));
        response.EnsureSuccessStatusCode();
        
        MileageRoot? root = await response.Content.ReadFromJsonAsync<MileageRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        
        Mileage? mileage = root?.Mileage;
        
        if (mileage == null)
        {
            throw new InvalidOperationException($"Mileage {id} not found");
        }
        
        this.cache.Set(cacheKey, mileage, TimeSpan.FromMinutes(5));
        
        return mileage;
    }

    public async Task<Mileage> UpdateAsync(string id, Mileage mileage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(mileage);
        await this.client.InitializeAndAuthorizeAsync();

        MileageRoot data = new() { Mileage = mileage };
        using JsonContent content = JsonContent.Create(data, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.client.HttpClient.PutAsync(new Uri(this.client.ApiBaseUrl, $"/v2/mileages/{id}"), content);
        response.EnsureSuccessStatusCode();

        MileageRoot? root = await response.Content.ReadFromJsonAsync<MileageRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove($"mileage_{id}");
        this.cache.Remove("mileages_all");

        return root?.Mileage ?? throw new InvalidOperationException("Failed to update mileage");
    }

    public async Task DeleteAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.DeleteAsync(new Uri(this.client.ApiBaseUrl, $"/v2/mileages/{id}"));
        response.EnsureSuccessStatusCode();

        this.cache.Remove($"mileage_{id}");
        this.cache.Remove("mileages_all");
    }
}
