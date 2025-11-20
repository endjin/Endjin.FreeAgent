// <copyright file="Payroll.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for accessing UK RTI (Real Time Information) payroll data via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides read-only access to FreeAgent payroll data for UK companies. It retrieves
/// payroll periods, HMRC payments, and employee payslips for RTI reporting purposes.
/// </para>
/// <para>
/// The payroll API uses UK tax years (April to March). When specifying a year parameter, use the
/// tax year end (e.g., 2026 for the tax year April 2025 - March 2026).
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance. Cache entries are invalidated when
/// payment status is updated.
/// </para>
/// <para>
/// Minimum Access Level: Tax and Limited Accounting. Only available for UK companies.
/// </para>
/// </remarks>
/// <seealso cref="PayrollPeriod"/>
/// <seealso cref="PayrollPayment"/>
/// <seealso cref="Payslip"/>
public class Payroll
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="Payroll"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing payroll data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public Payroll(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Retrieves all payroll periods and payments for a tax year.
    /// </summary>
    /// <param name="year">The tax year end (e.g., 2026 for April 2025 - March 2026).</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a
    /// <see cref="PayrollYearRoot"/> with periods and payments for the tax year.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls GET /v2/payroll/:year and caches the result for 5 minutes.
    /// </remarks>
    public async Task<PayrollYearRoot> GetPayrollYearAsync(int year)
    {
        await this.client.InitializeAndAuthorizeAsync();

        string cacheKey = $"payroll_year_{year}";

        if (this.cache.TryGetValue(cacheKey, out PayrollYearRoot? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(
            new Uri(this.client.ApiBaseUrl, $"/v2/payroll/{year}"));
        response.EnsureSuccessStatusCode();

        PayrollYearRoot? root = await response.Content.ReadFromJsonAsync<PayrollYearRoot>(
            SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        if (root == null)
        {
            throw new InvalidOperationException($"Failed to retrieve payroll data for year {year}");
        }

        this.cache.Set(cacheKey, root, TimeSpan.FromMinutes(5));

        return root;
    }

    /// <summary>
    /// Retrieves a specific payroll period including all payslips for that period.
    /// </summary>
    /// <param name="year">The tax year end (e.g., 2026 for April 2025 - March 2026).</param>
    /// <param name="period">The period number (0-11, where 0 is April and 11 is March).</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="PayrollPeriod"/> with embedded payslips.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="period"/> is not between 0 and 11.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls GET /v2/payroll/:year/:period and caches the result for 5 minutes.
    /// </remarks>
    public async Task<PayrollPeriod> GetPayrollPeriodAsync(int year, int period)
    {
        if (period < 0 || period > 11)
        {
            throw new ArgumentOutOfRangeException(nameof(period), "Period must be between 0 and 11.");
        }

        await this.client.InitializeAndAuthorizeAsync();

        string cacheKey = $"payroll_period_{year}_{period}";

        if (this.cache.TryGetValue(cacheKey, out PayrollPeriod? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(
            new Uri(this.client.ApiBaseUrl, $"/v2/payroll/{year}/{period}"));
        response.EnsureSuccessStatusCode();

        PayrollPeriodRoot? root = await response.Content.ReadFromJsonAsync<PayrollPeriodRoot>(
            SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        if (root?.Period == null)
        {
            throw new InvalidOperationException($"Failed to retrieve payroll period {period} for year {year}");
        }

        this.cache.Set(cacheKey, root.Period, TimeSpan.FromMinutes(5));

        return root.Period;
    }

    /// <summary>
    /// Marks a payroll payment to HMRC as paid.
    /// </summary>
    /// <param name="year">The tax year end (e.g., 2026 for April 2025 - March 2026).</param>
    /// <param name="paymentDate">The payment due date in YYYY-MM-DD format.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="PayrollYearRoot"/> with the payment status changed to "marked_as_paid".
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="paymentDate"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/payroll/:year/payments/:payment_date/mark_as_paid and invalidates
    /// the cache for the payroll year.
    /// </remarks>
    public async Task<PayrollYearRoot> MarkPaymentAsPaidAsync(int year, string paymentDate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(paymentDate);
        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.PutAsync(
            new Uri(this.client.ApiBaseUrl, $"/v2/payroll/{year}/payments/{paymentDate}/mark_as_paid"),
            null);
        response.EnsureSuccessStatusCode();

        PayrollYearRoot? root = await response.Content.ReadFromJsonAsync<PayrollYearRoot>(
            SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        if (root == null)
        {
            throw new InvalidOperationException($"Failed to mark payment as paid for {paymentDate}");
        }

        // Invalidate cache
        this.cache.Remove($"payroll_year_{year}");

        return root;
    }

    /// <summary>
    /// Marks a payroll payment to HMRC as unpaid.
    /// </summary>
    /// <param name="year">The tax year end (e.g., 2026 for April 2025 - March 2026).</param>
    /// <param name="paymentDate">The payment due date in YYYY-MM-DD format.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="PayrollYearRoot"/> with the payment status changed to "unpaid".
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="paymentDate"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls GET /v2/payroll/:year/payments/:payment_date/mark_as_unpaid and invalidates
    /// the cache for the payroll year.
    /// </remarks>
    public async Task<PayrollYearRoot> MarkPaymentAsUnpaidAsync(int year, string paymentDate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(paymentDate);
        await this.client.InitializeAndAuthorizeAsync();

        // Note: The FreeAgent API documentation specifies GET for this endpoint
        HttpResponseMessage response = await this.client.HttpClient.GetAsync(
            new Uri(this.client.ApiBaseUrl, $"/v2/payroll/{year}/payments/{paymentDate}/mark_as_unpaid"));
        response.EnsureSuccessStatusCode();

        PayrollYearRoot? root = await response.Content.ReadFromJsonAsync<PayrollYearRoot>(
            SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        if (root == null)
        {
            throw new InvalidOperationException($"Failed to mark payment as unpaid for {paymentDate}");
        }

        // Invalidate cache
        this.cache.Remove($"payroll_year_{year}");

        return root;
    }
}
