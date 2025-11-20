// <copyright file="ProfitAndLossReports.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for retrieving profit and loss summary reports from the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent profit and loss (P&amp;L) summary reports, which show the
/// financial performance of the company over a specific period. The P&amp;L summary displays income,
/// expenses, operating profit, deductions, and retained profit figures.
/// </para>
/// <para>
/// Profit and loss data is cached for 30 minutes to improve performance, as financial reports are
/// computation-intensive to generate and relatively stable for historical periods.
/// </para>
/// <para>
/// Minimum Access Level: Tax, Accounting &amp; Users
/// </para>
/// </remarks>
/// <seealso cref="ProfitAndLoss"/>
/// <seealso cref="ProfitAndLossDeduction"/>
public class ProfitAndLossReports
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProfitAndLossReports"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing profit and loss data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public ProfitAndLossReports(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Retrieves a profit and loss summary report for a specific date range from FreeAgent.
    /// </summary>
    /// <param name="fromDate">The start date of the reporting period.</param>
    /// <param name="toDate">The end date of the reporting period.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="ProfitAndLoss"/> summary with income, expenses, and profit data for the period.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// <para>
    /// This method calls GET /v2/accounting/profit_and_loss/summary?from_date={fromDate}&amp;to_date={toDate}
    /// and caches the result for 30 minutes.
    /// </para>
    /// <para>
    /// Note: Requested date periods must be equal to or less than 12 months, or be contained within
    /// a single accounting year.
    /// </para>
    /// </remarks>
    public async Task<ProfitAndLoss> GetAsync(DateOnly fromDate, DateOnly toDate)
    {
        await this.client.InitializeAndAuthorizeAsync();

        string url = $"/v2/accounting/profit_and_loss/summary?from_date={fromDate:yyyy-MM-dd}&to_date={toDate:yyyy-MM-dd}";
        string cacheKey = $"profit_loss_{fromDate:yyyy-MM-dd}_{toDate:yyyy-MM-dd}";

        if (this.cache.TryGetValue(cacheKey, out ProfitAndLoss? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, url));
        response.EnsureSuccessStatusCode();

        ProfitAndLossRoot? root = await response.Content.ReadFromJsonAsync<ProfitAndLossRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        ProfitAndLoss? profitAndLoss = root?.ProfitAndLoss;

        if (profitAndLoss == null)
        {
            throw new InvalidOperationException("Failed to retrieve profit and loss summary");
        }

        this.cache.Set(cacheKey, profitAndLoss, TimeSpan.FromMinutes(30));

        return profitAndLoss;
    }

    /// <summary>
    /// Retrieves a profit and loss summary report for an accounting period from FreeAgent.
    /// </summary>
    /// <param name="accountingPeriod">
    /// The accounting period in the format "YYYY/YY" (e.g., "2022/23" for the fiscal year 2022-2023).
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="ProfitAndLoss"/> summary with income, expenses, and profit data for the period.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="accountingPeriod"/> is null or empty.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// <para>
    /// This method calls GET /v2/accounting/profit_and_loss/summary?accounting_period={accountingPeriod}
    /// and caches the result for 30 minutes.
    /// </para>
    /// <para>
    /// The accounting period format follows the fiscal year convention (e.g., "2022/23" represents
    /// the accounting year starting in 2022 and ending in 2023).
    /// </para>
    /// </remarks>
    public async Task<ProfitAndLoss> GetByAccountingPeriodAsync(string accountingPeriod)
    {
        if (string.IsNullOrWhiteSpace(accountingPeriod))
        {
            throw new ArgumentException("Accounting period cannot be null or empty", nameof(accountingPeriod));
        }

        await this.client.InitializeAndAuthorizeAsync();

        string url = $"/v2/accounting/profit_and_loss/summary?accounting_period={Uri.EscapeDataString(accountingPeriod)}";
        string cacheKey = $"profit_loss_period_{accountingPeriod}";

        if (this.cache.TryGetValue(cacheKey, out ProfitAndLoss? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, url));
        response.EnsureSuccessStatusCode();

        ProfitAndLossRoot? root = await response.Content.ReadFromJsonAsync<ProfitAndLossRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        ProfitAndLoss? profitAndLoss = root?.ProfitAndLoss;

        if (profitAndLoss == null)
        {
            throw new InvalidOperationException("Failed to retrieve profit and loss summary");
        }

        this.cache.Set(cacheKey, profitAndLoss, TimeSpan.FromMinutes(30));

        return profitAndLoss;
    }

    /// <summary>
    /// Retrieves the profit and loss summary report for the current accounting year to date from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="ProfitAndLoss"/> summary with income, expenses, and profit data for the current year to date.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// <para>
    /// This method calls GET /v2/accounting/profit_and_loss/summary without any date parameters,
    /// which returns the current accounting year to date. The result is cached for 30 minutes.
    /// </para>
    /// </remarks>
    public async Task<ProfitAndLoss> GetCurrentYearToDateAsync()
    {
        await this.client.InitializeAndAuthorizeAsync();

        string url = "/v2/accounting/profit_and_loss/summary";
        string cacheKey = "profit_loss_current_ytd";

        if (this.cache.TryGetValue(cacheKey, out ProfitAndLoss? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, url));
        response.EnsureSuccessStatusCode();

        ProfitAndLossRoot? root = await response.Content.ReadFromJsonAsync<ProfitAndLossRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        ProfitAndLoss? profitAndLoss = root?.ProfitAndLoss;

        if (profitAndLoss == null)
        {
            throw new InvalidOperationException("Failed to retrieve profit and loss summary");
        }

        this.cache.Set(cacheKey, profitAndLoss, TimeSpan.FromMinutes(30));

        return profitAndLoss;
    }
}
