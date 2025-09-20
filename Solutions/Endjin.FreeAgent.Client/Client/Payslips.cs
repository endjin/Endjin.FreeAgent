// <copyright file="Payslips.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class Payslips
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    public Payslips(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<IEnumerable<Payslip>> GetAllAsync(DateOnly? fromDate = null, DateOnly? toDate = null)
    {
        await this.client.InitializeAndAuthorizeAsync();

        string url = "/v2/payslips";
        List<string> queryParams = [];

        if (fromDate.HasValue)
        {
            queryParams.Add($"from_date={fromDate.Value:yyyy-MM-dd}");
        }

        if (toDate.HasValue)
        {
            queryParams.Add($"to_date={toDate.Value:yyyy-MM-dd}");
        }

        if (queryParams.Count > 0)
        {
            url += "?" + string.Join("&", queryParams);
        }

        string cacheKey = $"payslips_{fromDate?.ToString("yyyy-MM-dd") ?? "all"}_{toDate?.ToString("yyyy-MM-dd") ?? "all"}";

        if (this.cache.TryGetValue(cacheKey, out IEnumerable<Payslip>? cached))
        {
            return cached!;
        }
        
        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, url));
        response.EnsureSuccessStatusCode();

        PayslipsRoot? root = await response.Content.ReadFromJsonAsync<PayslipsRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        IEnumerable<Payslip> payslips = root?.Payslips ?? [];

        this.cache.Set(cacheKey, payslips, TimeSpan.FromMinutes(30));

        return payslips;
    }

    public async Task<IEnumerable<Payslip>> GetByUserAsync(Uri userUrl, DateOnly? fromDate = null, DateOnly? toDate = null)
    {
        await this.client.InitializeAndAuthorizeAsync();

        string url = $"/v2/payslips?user={Uri.EscapeDataString(userUrl.ToString())}";

        if (fromDate.HasValue)
        {
            url += $"&from_date={fromDate.Value:yyyy-MM-dd}";
        }

        if (toDate.HasValue)
        {
            url += $"&to_date={toDate.Value:yyyy-MM-dd}";
        }

        string cacheKey = $"payslips_user_{userUrl}_{fromDate?.ToString("yyyy-MM-dd") ?? "all"}_{toDate?.ToString("yyyy-MM-dd") ?? "all"}";

        if (this.cache.TryGetValue(cacheKey, out IEnumerable<Payslip>? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, url));
        response.EnsureSuccessStatusCode();

        PayslipsRoot? root = await response.Content.ReadFromJsonAsync<PayslipsRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        IEnumerable<Payslip> payslips = root?.Payslips ?? [];

        this.cache.Set(cacheKey, payslips, TimeSpan.FromMinutes(30));

        return payslips;
    }
}
