// <copyright file="FinalAccountsReports.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;
using Endjin.FreeAgent.Domain.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing Final Accounts reports via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent Final Accounts reports, which are statutory accounts
/// that UK limited companies must file with Companies House. Final Accounts summarize the company's
/// financial position at the end of the accounting period and must be filed within 9 months of the
/// period end date for private companies. This service allows retrieval of report data and marking
/// reports as filed with Companies House.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance. Cache entries are invalidated automatically
/// when reports are marked as filed or when report status changes.
/// </para>
/// </remarks>
/// <seealso cref="FinalAccountsReport"/>
/// <seealso cref="Company"/>
public class FinalAccountsReports
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="FinalAccountsReports"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing Final Accounts report data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public FinalAccountsReports(FreeAgentClient client, IMemoryCache cache)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(cache);

        this.client = client;
        this.cache = cache;
    }

    /// <summary>
    /// Retrieves all Final Accounts reports from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="FinalAccountsReport"/> objects for all accounting periods.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/final_accounts_reports and caches the result for 5 minutes.
    /// Returns include both filed and unfiled Final Accounts periods.
    /// </remarks>
    public async Task<IEnumerable<FinalAccountsReport>> GetAllAsync()
    {
        string cacheKey = "final_accounts_reports_all";

        if (this.cache.TryGetValue(cacheKey, out IEnumerable<FinalAccountsReport>? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, "/v2/final_accounts_reports")).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        FinalAccountsReportsRoot? root = await response.Content.ReadFromJsonAsync<FinalAccountsReportsRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        IEnumerable<FinalAccountsReport> reports = root?.FinalAccountsReports ?? [];

        this.cache.Set(cacheKey, reports, TimeSpan.FromMinutes(5));

        return reports;
    }

    /// <summary>
    /// Retrieves a specific Final Accounts report by its period end date from FreeAgent.
    /// </summary>
    /// <param name="periodEndsOn">The period end date of the Final Accounts report.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="FinalAccountsReport"/> object with all report details.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the report is not found or cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls GET /v2/final_accounts_reports/{period_ends_on} where period_ends_on is formatted as yyyy-MM-dd.
    /// The result is cached for 5 minutes.
    /// </remarks>
    public async Task<FinalAccountsReport> GetByPeriodEndDateAsync(DateOnly periodEndsOn)
    {
        string periodEndsOnFormatted = periodEndsOn.ToString("yyyy-MM-dd");
        string cacheKey = $"final_accounts_report_{periodEndsOnFormatted}";

        if (this.cache.TryGetValue(cacheKey, out FinalAccountsReport? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, $"/v2/final_accounts_reports/{periodEndsOnFormatted}")).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        FinalAccountsReportRoot? root = await response.Content.ReadFromJsonAsync<FinalAccountsReportRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        FinalAccountsReport? report = (root?.FinalAccountsReport) ?? throw new InvalidOperationException($"Final Accounts report for period ending {periodEndsOnFormatted} not found");

        this.cache.Set(cacheKey, report, TimeSpan.FromMinutes(5));

        return report;
    }

    /// <summary>
    /// Marks a Final Accounts report as filed with Companies House.
    /// </summary>
    /// <param name="periodEndsOn">The period end date of the Final Accounts report to mark as filed.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="FinalAccountsReport"/> object reflecting the filed status.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/final_accounts_reports/{period_ends_on}/mark_as_filed where period_ends_on is formatted as yyyy-MM-dd.
    /// It invalidates the cache entries for the report. The filing_status is updated to "marked_as_filed" automatically.
    /// </remarks>
    public async Task<FinalAccountsReport> MarkAsFiledAsync(DateOnly periodEndsOn)
    {
        string periodEndsOnFormatted = periodEndsOn.ToString("yyyy-MM-dd");

        await this.client.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.client.HttpClient.PutAsync(
            new Uri(this.client.ApiBaseUrl, $"/v2/final_accounts_reports/{periodEndsOnFormatted}/mark_as_filed"),
            null).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        FinalAccountsReportRoot? root = await response.Content.ReadFromJsonAsync<FinalAccountsReportRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove($"final_accounts_report_{periodEndsOnFormatted}");
        this.cache.Remove("final_accounts_reports_all");

        return root?.FinalAccountsReport ?? throw new InvalidOperationException("Failed to mark Final Accounts report as filed");
    }

    /// <summary>
    /// Marks a Final Accounts report as unfiled with Companies House.
    /// </summary>
    /// <param name="periodEndsOn">The period end date of the Final Accounts report to mark as unfiled.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="FinalAccountsReport"/> object reflecting the unfiled status.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/final_accounts_reports/{period_ends_on}/mark_as_unfiled where period_ends_on is formatted as yyyy-MM-dd.
    /// It invalidates the cache entries for the report. The filing_status is updated to "unfiled" automatically.
    /// </remarks>
    public async Task<FinalAccountsReport> MarkAsUnfiledAsync(DateOnly periodEndsOn)
    {
        string periodEndsOnFormatted = periodEndsOn.ToString("yyyy-MM-dd");

        await this.client.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.client.HttpClient.PutAsync(
            new Uri(this.client.ApiBaseUrl, $"/v2/final_accounts_reports/{periodEndsOnFormatted}/mark_as_unfiled"),
            null).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        FinalAccountsReportRoot? root = await response.Content.ReadFromJsonAsync<FinalAccountsReportRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove($"final_accounts_report_{periodEndsOnFormatted}");
        this.cache.Remove("final_accounts_reports_all");

        return root?.FinalAccountsReport ?? throw new InvalidOperationException("Failed to mark Final Accounts report as unfiled");
    }
}
