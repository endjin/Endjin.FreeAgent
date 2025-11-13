// <copyright file="TrialBalances.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for retrieving trial balance reports from the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent trial balance reports, which list all general ledger
/// accounts and their debit and credit balances at a specific point in time. The trial balance is a key
/// accounting report used to verify that total debits equal total credits and to prepare financial statements.
/// </para>
/// <para>
/// Trial balance data is cached for 15 minutes to improve performance, as these reports are frequently
/// accessed during financial reconciliation and are computation-intensive to generate.
/// </para>
/// </remarks>
/// <seealso cref="TrialBalance"/>
/// <seealso cref="TrialBalanceEntry"/>
public class TrialBalances
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

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
    }

    /// <summary>
    /// Retrieves a trial balance report for a specific date from FreeAgent.
    /// </summary>
    /// <param name="date">The date for which to retrieve the trial balance. If null, returns the current trial balance.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="TrialBalance"/> object with all account balances.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls GET /v2/trial_balance?date={date} and caches the result for 15 minutes.
    /// The trial balance shows all accounts with their debit and credit balances, ensuring the books are balanced.
    /// </remarks>
    public async Task<TrialBalance> GetAsync(DateOnly? date = null)
    {
        await this.client.InitializeAndAuthorizeAsync();

        string url = "/v2/trial_balance";
        if (date.HasValue)
        {
            url += $"?date={date.Value:yyyy-MM-dd}";
        }

        string cacheKey = $"trial_balance_{date?.ToString("yyyy-MM-dd") ?? "current"}";
        
        if (this.cache.TryGetValue(cacheKey, out TrialBalance? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, url));
        response.EnsureSuccessStatusCode();
        
        TrialBalanceRoot? root = await response.Content.ReadFromJsonAsync<TrialBalanceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        
        TrialBalance? trialBalance = root?.TrialBalance;
        
        if (trialBalance == null)
        {
            throw new InvalidOperationException("Failed to retrieve trial balance");
        }
        
        this.cache.Set(cacheKey, trialBalance, TimeSpan.FromMinutes(15));
        
        return trialBalance;
    }
}