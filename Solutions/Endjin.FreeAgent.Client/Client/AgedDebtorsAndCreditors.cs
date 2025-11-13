// <copyright file="AgedDebtorsAndCreditors.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for retrieving aged debtors and creditors reports from the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent aged debtors (receivables) and aged creditors (payables)
/// reports. These reports categorize outstanding invoices and bills by how long they have been unpaid,
/// typically grouped into current, 30 days, 60 days, 90 days, and over 90 days past due. These reports
/// are essential for managing cash flow and identifying collection or payment issues.
/// </para>
/// <para>
/// Report data is cached for 30 minutes to improve performance. The reports provide crucial information
/// for accounts receivable and accounts payable management.
/// </para>
/// </remarks>
/// <seealso cref="SalesAgedDebtors"/>
/// <seealso cref="AgedDebtorEntry"/>
/// <seealso cref="PurchaseAgedCreditors"/>
/// <seealso cref="AgedCreditorEntry"/>
public class AgedDebtorsAndCreditors
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="AgedDebtorsAndCreditors"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing aged debtors and creditors data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public AgedDebtorsAndCreditors(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Retrieves an aged debtors (receivables) report for a specific date from FreeAgent.
    /// </summary>
    /// <param name="date">The date for which to retrieve the aged debtors report. If null, returns the current report.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="SalesAgedDebtors"/> object with outstanding receivables categorized by age.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls GET /v2/sales_aged_debtors?date={date} and caches the result for 30 minutes.
    /// The report shows which customers owe money and how overdue their invoices are, helping with
    /// credit control and cash flow management.
    /// </remarks>
    public async Task<SalesAgedDebtors> GetSalesAgedDebtorsAsync(DateOnly? date = null)
    {
        await this.client.InitializeAndAuthorizeAsync();

        string url = "/v2/sales_aged_debtors";
        if (date.HasValue)
        {
            url += $"?date={date.Value:yyyy-MM-dd}";
        }

        string cacheKey = $"sales_aged_debtors_{date?.ToString("yyyy-MM-dd") ?? "current"}";
        
        if (this.cache.TryGetValue(cacheKey, out SalesAgedDebtors? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, url));
        response.EnsureSuccessStatusCode();
        
        SalesAgedDebtorsRoot? root = await response.Content.ReadFromJsonAsync<SalesAgedDebtorsRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        
        SalesAgedDebtors? debtors = (root?.SalesAgedDebtors) ?? throw new InvalidOperationException("Failed to retrieve sales aged debtors");
        this.cache.Set(cacheKey, debtors, TimeSpan.FromMinutes(30));
        
        return debtors;
    }

    /// <summary>
    /// Retrieves an aged creditors (payables) report for a specific date from FreeAgent.
    /// </summary>
    /// <param name="date">The date for which to retrieve the aged creditors report. If null, returns the current report.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="PurchaseAgedCreditors"/> object with outstanding payables categorized by age.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls GET /v2/purchase_aged_creditors?date={date} and caches the result for 30 minutes.
    /// The report shows which suppliers are owed money and how overdue the bills are, helping with
    /// payment prioritization and supplier relationship management.
    /// </remarks>
    public async Task<PurchaseAgedCreditors> GetPurchaseAgedCreditorsAsync(DateOnly? date = null)
    {
        await this.client.InitializeAndAuthorizeAsync();

        string url = "/v2/purchase_aged_creditors";
        if (date.HasValue)
        {
            url += $"?date={date.Value:yyyy-MM-dd}";
        }

        string cacheKey = $"purchase_aged_creditors_{date?.ToString("yyyy-MM-dd") ?? "current"}";
        
        if (this.cache.TryGetValue(cacheKey, out PurchaseAgedCreditors? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, url));
        response.EnsureSuccessStatusCode();
        
        PurchaseAgedCreditorsRoot? root = await response.Content.ReadFromJsonAsync<PurchaseAgedCreditorsRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        PurchaseAgedCreditors? creditors = (root?.PurchaseAgedCreditors) ?? throw new InvalidOperationException("Failed to retrieve purchase aged creditors");

        this.cache.Set(cacheKey, creditors, TimeSpan.FromMinutes(30));
        
        return creditors;
    }
}