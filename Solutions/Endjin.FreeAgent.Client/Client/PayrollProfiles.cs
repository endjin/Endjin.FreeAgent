// <copyright file="PayrollProfiles.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Text;
using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class PayrollProfiles
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    public PayrollProfiles(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<IEnumerable<PayrollProfile>> GetAllAsync()
    {
        await this.client.InitializeAndAuthorizeAsync();

        string cacheKey = "payroll_profiles_all";

        if (this.cache.TryGetValue(cacheKey, out IEnumerable<PayrollProfile>? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, "/v2/payroll_profiles"));
        response.EnsureSuccessStatusCode();

        PayrollProfilesRoot? root = await response.Content.ReadFromJsonAsync<PayrollProfilesRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        IEnumerable<PayrollProfile> profiles = root?.PayrollProfiles ?? [];

        this.cache.Set(cacheKey, profiles, TimeSpan.FromMinutes(30));

        return profiles;
    }

    public async Task<PayrollProfile> GetAsync(long id)
    {
        await this.client.InitializeAndAuthorizeAsync();

        string cacheKey = $"payroll_profile_{id}";

        if (this.cache.TryGetValue(cacheKey, out PayrollProfile? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, $"/v2/payroll_profiles/{id}"));
        response.EnsureSuccessStatusCode();

        PayrollProfileRoot? root = await response.Content.ReadFromJsonAsync<PayrollProfileRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        PayrollProfile? profile = root?.PayrollProfile;

        if (profile == null)
        {
            throw new InvalidOperationException($"PayrollProfile {id} not found");
        }

        this.cache.Set(cacheKey, profile, TimeSpan.FromMinutes(30));

        return profile;
    }

    public async Task<PayrollProfile> CreateAsync(PayrollProfile profile)
    {
        await this.client.InitializeAndAuthorizeAsync();

        PayrollProfileRoot root = new() { PayrollProfile = profile };
        string json = JsonSerializer.Serialize(root, SharedJsonOptions.Instance);

        HttpResponseMessage response = await this.client.HttpClient.PostAsync(new Uri(this.client.ApiBaseUrl, "/v2/payroll_profiles"), new StringContent(json, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        PayrollProfileRoot? result = await response.Content.ReadFromJsonAsync<PayrollProfileRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.PayrollProfile ?? throw new InvalidOperationException("Failed to create payroll profile");
    }

    public async Task<PayrollProfile> UpdateAsync(long id, PayrollProfile profile)
    {
        await this.client.InitializeAndAuthorizeAsync();

        PayrollProfileRoot root = new() { PayrollProfile = profile };
        string json = JsonSerializer.Serialize(root, SharedJsonOptions.Instance);

        HttpResponseMessage response = await this.client.HttpClient.PutAsync(
            new Uri(this.client.ApiBaseUrl, $"/v2/payroll_profiles/{id}"),
            new StringContent(json, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        PayrollProfileRoot? result = await response.Content.ReadFromJsonAsync<PayrollProfileRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove($"payroll_profile_{id}");
        this.cache.Remove("payroll_profiles_all");

        return result?.PayrollProfile ?? throw new InvalidOperationException("Failed to update payroll profile");
    }

    public async Task DeleteAsync(long id)
    {
        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.DeleteAsync(
            new Uri(this.client.ApiBaseUrl, $"/v2/payroll_profiles/{id}"));
        response.EnsureSuccessStatusCode();

        this.cache.Remove($"payroll_profile_{id}");
        this.cache.Remove("payroll_profiles_all");
    }
}
