// <copyright file="Payslips.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for retrieving payslips via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent payslips, which are the detailed salary statements
/// issued to employees showing gross pay, deductions, net pay, tax, National Insurance, and other
/// payment details. Payslips are generated from payroll payments and serve as official records of
/// employee compensation. This service supports retrieving payslips filtered by date range and user.
/// </para>
/// <para>
/// Results are cached for 30 minutes to improve performance, as payslips are historical records that
/// do not change frequently once generated.
/// </para>
/// </remarks>
/// <seealso cref="Payslip"/>
/// <seealso cref="PayrollPayment"/>
/// <seealso cref="User"/>
public class Payslips
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="Payslips"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing payslip data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public Payslips(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Retrieves payslips from FreeAgent, optionally filtered by date range.
    /// </summary>
    /// <param name="fromDate">Optional start date to filter payslips. If null, no lower date bound is applied.</param>
    /// <param name="toDate">Optional end date to filter payslips. If null, no upper date bound is applied.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="Payslip"/> objects matching the filter criteria.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/payslips?from_date={fromDate}&amp;to_date={toDate} and caches the
    /// result for 30 minutes. The cache key includes all filter parameters.
    /// </remarks>
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

    /// <summary>
    /// Retrieves payslips for a specific user from FreeAgent, optionally filtered by date range.
    /// </summary>
    /// <param name="userUrl">The URL of the user to retrieve payslips for.</param>
    /// <param name="fromDate">Optional start date to filter payslips. If null, no lower date bound is applied.</param>
    /// <param name="toDate">Optional end date to filter payslips. If null, no upper date bound is applied.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="Payslip"/> objects for the specified user matching the filter criteria.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/payslips?user={userUrl}&amp;from_date={fromDate}&amp;to_date={toDate}
    /// and caches the result for 30 minutes. The cache key includes the user URL and all filter parameters.
    /// </remarks>
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