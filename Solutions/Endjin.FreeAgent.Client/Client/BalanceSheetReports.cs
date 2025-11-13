// <copyright file="BalanceSheetReports.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for retrieving balance sheet reports from the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent balance sheet reports, which show the financial position
/// of the company at a specific point in time. The balance sheet displays assets, liabilities, and equity,
/// providing a snapshot of what the company owns and owes.
/// </para>
/// <para>
/// Balance sheet data is cached for 30 minutes to improve performance, as financial reports are relatively
/// stable for historical dates and computation-intensive to generate.
/// </para>
/// </remarks>
/// <seealso cref="BalanceSheet"/>
/// <seealso cref="BalanceSheetEntry"/>
public class BalanceSheetReports
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="BalanceSheetReports"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing balance sheet data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public BalanceSheetReports(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Retrieves a balance sheet report for a specific date from FreeAgent.
    /// </summary>
    /// <param name="date">The date for which to retrieve the balance sheet. If null, returns the current balance sheet.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="BalanceSheet"/> object with assets, liabilities, and equity data.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls GET /v2/balance_sheet?date={date} and caches the result for 30 minutes.
    /// The balance sheet provides a complete view of the company's financial position at the specified date.
    /// </remarks>
    public async Task<BalanceSheet> GetAsync(DateOnly? date = null)
    {
        await this.client.InitializeAndAuthorizeAsync();

        string url = "/v2/balance_sheet";
        if (date.HasValue)
        {
            url += $"?date={date.Value:yyyy-MM-dd}";
        }

        string cacheKey = $"balance_sheet_{date?.ToString("yyyy-MM-dd") ?? "current"}";
        
        if (this.cache.TryGetValue(cacheKey, out BalanceSheet? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, url));
        response.EnsureSuccessStatusCode();
        
        BalanceSheetRoot? root = await response.Content.ReadFromJsonAsync<BalanceSheetRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        BalanceSheet? balanceSheet = (root?.BalanceSheet) ?? throw new InvalidOperationException("Failed to retrieve balance sheet");

        this.cache.Set(cacheKey, balanceSheet, TimeSpan.FromMinutes(30));
        
        return balanceSheet;
    }
}