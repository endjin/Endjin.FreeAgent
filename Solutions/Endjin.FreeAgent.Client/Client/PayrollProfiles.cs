// <copyright file="PayrollProfiles.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for accessing payroll profiles via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides read-only access to FreeAgent payroll profiles for UK companies.
/// Payroll profiles contain employee personal information relevant for RTI (Real Time Information)
/// reporting to HMRC, including address details, demographic information, and previous employment data.
/// </para>
/// <para>
/// The API uses UK tax years (April to March). When specifying a year parameter, use the
/// tax year end (e.g., 2026 for the tax year April 2025 - March 2026).
/// </para>
/// <para>
/// Results are cached for 5 minutes to reduce API calls while maintaining reasonably current data.
/// </para>
/// <para>
/// Minimum Access Level: Tax and Limited Accounting. Only available for UK companies.
/// </para>
/// </remarks>
/// <seealso cref="PayrollProfile"/>
/// <seealso cref="User"/>
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
    /// Retrieves all payroll profiles for a specific tax year.
    /// </summary>
    /// <param name="year">The tax year end (e.g., 2026 for April 2025 - March 2026).</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// all <see cref="PayrollProfile"/> objects for the specified tax year.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/payroll_profiles/{year} and caches the result for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<PayrollProfile>> GetAllAsync(int year)
    {
        await this.client.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        string cacheKey = $"payroll_profiles_{year}";

        if (this.cache.TryGetValue(cacheKey, out IEnumerable<PayrollProfile>? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(
            new Uri(this.client.ApiBaseUrl, $"v2/payroll_profiles/{year}")).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        PayrollProfilesRoot? root = await response.Content.ReadFromJsonAsync<PayrollProfilesRoot>(
            SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        IEnumerable<PayrollProfile> profiles = root?.Profiles ?? [];

        this.cache.Set(cacheKey, profiles, TimeSpan.FromMinutes(5));

        return profiles;
    }

    /// <summary>
    /// Retrieves a payroll profile for a specific user in a tax year.
    /// </summary>
    /// <param name="year">The tax year end (e.g., 2026 for April 2025 - March 2026).</param>
    /// <param name="userUrl">The URL of the user whose payroll profile to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="PayrollProfile"/> objects for the specified user in the tax year.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="userUrl"/> is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/payroll_profiles/{year}?user={url} to retrieve the payroll profile
    /// for a specific user. Results are not cached as they are typically requested on-demand.
    /// </remarks>
    public async Task<IEnumerable<PayrollProfile>> GetByUserAsync(int year, Uri userUrl)
    {
        ArgumentNullException.ThrowIfNull(userUrl);

        await this.client.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        string encodedUserUrl = Uri.EscapeDataString(userUrl.ToString());
        HttpResponseMessage response = await this.client.HttpClient.GetAsync(
            new Uri(this.client.ApiBaseUrl, $"v2/payroll_profiles/{year}?user={encodedUserUrl}")).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        PayrollProfilesRoot? root = await response.Content.ReadFromJsonAsync<PayrollProfilesRoot>(
            SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return root?.Profiles ?? [];
    }
}
