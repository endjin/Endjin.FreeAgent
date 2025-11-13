// <copyright file="PayrollProfiles.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Text;
using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing payroll profiles via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides comprehensive access to FreeAgent payroll profiles, which contain employee
/// tax and National Insurance configuration data. Payroll profiles store information such as tax codes,
/// NI category letters, student loan deductions, and other HMRC-related settings needed for payroll
/// processing. This service supports creating, retrieving, updating, and deleting payroll profiles.
/// </para>
/// <para>
/// Results are cached for 30 minutes to improve performance, as payroll profiles change infrequently.
/// Cache entries are invalidated automatically when profiles are created, updated, or deleted.
/// </para>
/// </remarks>
/// <seealso cref="PayrollProfile"/>
/// <seealso cref="User"/>
/// <seealso cref="Payroll"/>
public class PayrollProfiles
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="PayrollProfiles"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing payroll profile data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public PayrollProfiles(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Retrieves all payroll profiles from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="PayrollProfile"/> objects for all employees.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/payroll_profiles and caches the result for 30 minutes.
    /// </remarks>
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

    /// <summary>
    /// Retrieves a specific payroll profile by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the payroll profile.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="PayrollProfile"/> object with all profile details.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the profile is not found or cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls GET /v2/payroll_profiles/{id} and caches the result for 30 minutes.
    /// </remarks>
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

    /// <summary>
    /// Creates a new payroll profile in FreeAgent.
    /// </summary>
    /// <param name="profile">The <see cref="PayrollProfile"/> object containing the profile details to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="PayrollProfile"/> object with server-assigned values.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/payroll_profiles. The cache is not updated as only aggregate queries are cached.
    /// </remarks>
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

    /// <summary>
    /// Updates an existing payroll profile in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the payroll profile to update.</param>
    /// <param name="profile">The <see cref="PayrollProfile"/> object containing the updated profile details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="PayrollProfile"/> object.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/payroll_profiles/{id} and invalidates the cache entries for the profile.
    /// </remarks>
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

    /// <summary>
    /// Deletes a payroll profile from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the payroll profile to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls DELETE /v2/payroll_profiles/{id} and invalidates the cache entries for the profile.
    /// The deletion is permanent and cannot be undone.
    /// </remarks>
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