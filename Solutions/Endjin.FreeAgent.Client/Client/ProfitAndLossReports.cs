// <copyright file="ProfitAndLossReports.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for retrieving profit and loss reports from the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent profit and loss (P&amp;L) reports, which show the
/// financial performance of the company over a specific period. The P&amp;L statement displays revenue,
/// costs, and expenses, ultimately showing the net profit or loss for the period.
/// </para>
/// <para>
/// Profit and loss data is cached for 30 minutes to improve performance, as financial reports are
/// computation-intensive to generate and relatively stable for historical periods.
/// </para>
/// </remarks>
/// <seealso cref="ProfitAndLoss"/>
/// <seealso cref="ProfitAndLossEntry"/>
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
    /// Retrieves a profit and loss report for a specific date range from FreeAgent.
    /// </summary>
    /// <param name="fromDate">The start date of the reporting period.</param>
    /// <param name="toDate">The end date of the reporting period.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="ProfitAndLoss"/> object with revenue, expenses, and profit/loss data for the period.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls GET /v2/profit_and_loss?from_date={fromDate}&amp;to_date={toDate} and caches the
    /// result for 30 minutes. The report provides a comprehensive view of the company's financial performance
    /// for the specified period.
    /// </remarks>
    public async Task<ProfitAndLoss> GetAsync(DateOnly fromDate, DateOnly toDate)
    {
        await this.client.InitializeAndAuthorizeAsync();

        string url = $"/v2/profit_and_loss?from_date={fromDate:yyyy-MM-dd}&to_date={toDate:yyyy-MM-dd}";
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
            throw new InvalidOperationException("Failed to retrieve profit and loss report");
        }
        
        this.cache.Set(cacheKey, profitAndLoss, TimeSpan.FromMinutes(30));
        
        return profitAndLoss;
    }
}