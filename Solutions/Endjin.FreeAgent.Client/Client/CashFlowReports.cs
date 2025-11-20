// <copyright file="CashFlowReports.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for retrieving cash flow reports from the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent cash flow reports, which show monthly aggregated
/// incoming and outgoing cash flow over a specific period. The report provides a simple summary of
/// revenue and expenses broken down by month, with totals for the entire period.
/// </para>
/// <para>
/// Cash flow data is cached for 30 minutes to improve performance, as financial reports are
/// computation-intensive to generate and relatively stable for historical periods.
/// </para>
/// </remarks>
/// <seealso cref="CashFlow"/>
public class CashFlowReports
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="CashFlowReports"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing cash flow data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public CashFlowReports(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Retrieves a cash flow report for a specific date range from FreeAgent.
    /// </summary>
    /// <param name="fromDate">The start date of the reporting period.</param>
    /// <param name="toDate">The end date of the reporting period.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="CashFlow"/> object with monthly aggregated incoming and outgoing cash flow data.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls GET /v2/cashflow?from_date={fromDate}&amp;to_date={toDate} and caches the
    /// result for 30 minutes. The report provides monthly aggregated cash flow data during the specified period.
    /// </remarks>
    public async Task<CashFlow> GetAsync(DateOnly fromDate, DateOnly toDate)
    {
        await this.client.InitializeAndAuthorizeAsync();

        string url = $"/v2/cashflow?from_date={fromDate:yyyy-MM-dd}&to_date={toDate:yyyy-MM-dd}";
        string cacheKey = $"cash_flow_{fromDate:yyyy-MM-dd}_{toDate:yyyy-MM-dd}";
        
        if (this.cache.TryGetValue(cacheKey, out CashFlow? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, url));
        response.EnsureSuccessStatusCode();
        
        CashFlowRoot? root = await response.Content.ReadFromJsonAsync<CashFlowRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        CashFlow? cashFlow = (root?.CashFlow) ?? throw new InvalidOperationException("Failed to retrieve cash flow report");

        this.cache.Set(cacheKey, cashFlow, TimeSpan.FromMinutes(30));
        
        return cashFlow;
    }
}