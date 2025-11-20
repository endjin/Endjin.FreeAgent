// <copyright file="TrialBalances.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for retrieving trial balance summaries from the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent trial balance summary reports, which list all general ledger
/// categories and their balances. The trial balance is a key accounting report used to verify that total debits
/// equal total credits and to prepare financial statements.
/// </para>
/// <para>
/// Two endpoints are available:
/// <list type="bullet">
/// <item><description>Summary endpoint - Returns the trial balance for a date range</description></item>
/// <item><description>Opening balances endpoint - Returns the opening balances</description></item>
/// </list>
/// </para>
/// <para>
/// Trial balance data is cached for 15 minutes to improve performance, as these reports are frequently
/// accessed during financial reconciliation and are computation-intensive to generate.
/// </para>
/// <para>
/// Minimum Access Level: Tax, Accounting &amp; Users
/// </para>
/// </remarks>
/// <seealso cref="TrialBalanceSummaryEntry"/>
/// <seealso cref="Category"/>
public class TrialBalances
{
    private const string TrialBalanceSummaryEndpoint = "v2/accounting/trial_balance/summary";
    private const string OpeningBalancesEndpoint = "v2/accounting/trial_balance/summary/opening_balances";

    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="TrialBalances"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing trial balance data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public TrialBalances(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(15));
    }

    /// <summary>
    /// Retrieves a trial balance summary from FreeAgent with optional date filtering.
    /// </summary>
    /// <param name="fromDate">
    /// Optional start date for filtering the trial balance (inclusive). Used with <paramref name="toDate"/>
    /// to specify a custom date range.
    /// </param>
    /// <param name="toDate">
    /// Optional end date for filtering the trial balance (inclusive). If only this is provided without
    /// <paramref name="fromDate"/>, the summary covers the period from accounting start through the specified date.
    /// If neither date is provided, returns the current trial balance.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="TrialBalanceSummaryEntry"/> objects representing each category in the trial balance.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// <para>
    /// This method calls GET /v2/accounting/trial_balance/summary and caches the result for 15 minutes.
    /// </para>
    /// <para>
    /// Date handling:
    /// <list type="bullet">
    /// <item><description>No dates specified - returns the current trial balance</description></item>
    /// <item><description>Only to_date specified - returns the summary from accounting start through that date</description></item>
    /// <item><description>Both dates specified - returns the summary for the custom date range</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Minimum Access Level: Tax, Accounting &amp; Users
    /// </para>
    /// </remarks>
    public async Task<IEnumerable<TrialBalanceSummaryEntry>> GetSummaryAsync(
        DateOnly? fromDate = null,
        DateOnly? toDate = null)
    {
        List<string> queryParams = [];

        if (fromDate.HasValue)
        {
            queryParams.Add($"from_date={fromDate.Value:yyyy-MM-dd}");
        }

        if (toDate.HasValue)
        {
            queryParams.Add($"to_date={toDate.Value:yyyy-MM-dd}");
        }

        string queryString = queryParams.Count > 0 ? $"?{string.Join("&", queryParams)}" : string.Empty;
        string cacheKey = $"{TrialBalanceSummaryEndpoint}{queryString}";

        if (this.cache.TryGetValue(cacheKey, out IEnumerable<TrialBalanceSummaryEntry>? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(
            new Uri(this.client.ApiBaseUrl, $"{TrialBalanceSummaryEndpoint}{queryString}")).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        TrialBalanceSummaryRoot? root = await response.Content.ReadFromJsonAsync<TrialBalanceSummaryRoot>(
            SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        IEnumerable<TrialBalanceSummaryEntry> entries = root?.TrialBalanceSummary ?? [];

        this.cache.Set(cacheKey, entries, this.cacheEntryOptions);

        return entries;
    }

    /// <summary>
    /// Retrieves the opening balances from the trial balance summary.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="TrialBalanceSummaryEntry"/> objects representing the opening balances for each category.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// <para>
    /// This method calls GET /v2/accounting/trial_balance/summary/opening_balances and caches the result
    /// for 15 minutes.
    /// </para>
    /// <para>
    /// Opening balances represent the starting balances for each category at the beginning of the
    /// accounting period.
    /// </para>
    /// <para>
    /// Minimum Access Level: Tax, Accounting &amp; Users
    /// </para>
    /// </remarks>
    public async Task<IEnumerable<TrialBalanceSummaryEntry>> GetOpeningBalancesAsync()
    {
        string cacheKey = OpeningBalancesEndpoint;

        if (this.cache.TryGetValue(cacheKey, out IEnumerable<TrialBalanceSummaryEntry>? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(
            new Uri(this.client.ApiBaseUrl, OpeningBalancesEndpoint)).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        TrialBalanceSummaryRoot? root = await response.Content.ReadFromJsonAsync<TrialBalanceSummaryRoot>(
            SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        IEnumerable<TrialBalanceSummaryEntry> entries = root?.TrialBalanceSummary ?? [];

        this.cache.Set(cacheKey, entries, this.cacheEntryOptions);

        return entries;
    }
}
