// <copyright file="Payroll.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing payroll payments via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides comprehensive access to FreeAgent payroll payments, which represent salary
/// and wage payments made to employees. Payroll payments track gross pay, deductions, net pay, employer
/// contributions, and tax/NI calculations. This service supports creating, retrieving, updating, and
/// deleting payroll payment records.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance. Cache entries are invalidated automatically
/// when payments are created, updated, or deleted.
/// </para>
/// </remarks>
/// <seealso cref="PayrollPayment"/>
/// <seealso cref="User"/>
/// <seealso cref="Payslip"/>
public class Payroll
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="Payroll"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing payroll payment data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public Payroll(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Creates a new payroll payment in FreeAgent.
    /// </summary>
    /// <param name="payment">The <see cref="PayrollPayment"/> object containing the payment details to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="PayrollPayment"/> object with server-assigned values.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="payment"/> is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/payroll_payments and invalidates the cache for all payroll payments.
    /// </remarks>
    public async Task<PayrollPayment> CreateAsync(PayrollPayment payment)
    {
        ArgumentNullException.ThrowIfNull(payment);
        await this.client.InitializeAndAuthorizeAsync();

        PayrollPaymentRoot data = new() { PayrollPayment = payment };
        using JsonContent content = JsonContent.Create(data, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.client.HttpClient.PostAsync(new Uri(this.client.ApiBaseUrl, "/v2/payroll_payments"), content);
        response.EnsureSuccessStatusCode();

        PayrollPaymentRoot? root = await response.Content.ReadFromJsonAsync<PayrollPaymentRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove("payroll_payments_all");

        return root?.PayrollPayment ?? throw new InvalidOperationException("Failed to create payroll payment");
    }

    /// <summary>
    /// Retrieves payroll payments from FreeAgent, optionally filtered by user and date range.
    /// </summary>
    /// <param name="userId">Optional user ID to filter payments. If null, returns payments for all users.</param>
    /// <param name="fromDate">Optional start date to filter payments. If null, no lower date bound is applied.</param>
    /// <param name="toDate">Optional end date to filter payments. If null, no upper date bound is applied.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="PayrollPayment"/> objects matching the filter criteria.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/payroll_payments?user={userId}&amp;from_date={fromDate}&amp;to_date={toDate}
    /// and caches the result for 5 minutes. The cache key includes all filter parameters.
    /// </remarks>
    public async Task<IEnumerable<PayrollPayment>> GetAllAsync(string? userId = null, DateOnly? fromDate = null, DateOnly? toDate = null)
    {
        await this.client.InitializeAndAuthorizeAsync();

        List<string> queryParams = [];
        if (!string.IsNullOrWhiteSpace(userId))
        {
            queryParams.Add($"user={userId}");
        }

        if (fromDate.HasValue)
        {
            queryParams.Add($"from_date={fromDate.Value:yyyy-MM-dd}");
        }

        if (toDate.HasValue)
        {
            queryParams.Add($"to_date={toDate.Value:yyyy-MM-dd}");
        }

        string url = "/v2/payroll_payments";
        if (queryParams.Count > 0)
        {
            url += "?" + string.Join("&", queryParams);
        }

        string cacheKey = $"payroll_payments_{userId ?? "all"}_{fromDate?.ToString("yyyyMMdd")}_{toDate?.ToString("yyyyMMdd")}";
        
        if (this.cache.TryGetValue(cacheKey, out IEnumerable<PayrollPayment>? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, url));
        response.EnsureSuccessStatusCode();
        
        PayrollPaymentsRoot? root = await response.Content.ReadFromJsonAsync<PayrollPaymentsRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        
        IEnumerable<PayrollPayment> payments = root?.PayrollPayments ?? [];
        
        this.cache.Set(cacheKey, payments, TimeSpan.FromMinutes(5));
        
        return payments;
    }

    /// <summary>
    /// Retrieves a specific payroll payment by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the payroll payment.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="PayrollPayment"/> object with all payment details.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the payment is not found or cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls GET /v2/payroll_payments/{id} and caches the result for 5 minutes.
    /// </remarks>
    public async Task<PayrollPayment> GetByIdAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        await this.client.InitializeAndAuthorizeAsync();
        
        string cacheKey = $"payroll_payment_{id}";
        
        if (this.cache.TryGetValue(cacheKey, out PayrollPayment? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, $"/v2/payroll_payments/{id}"));
        response.EnsureSuccessStatusCode();
        
        PayrollPaymentRoot? root = await response.Content.ReadFromJsonAsync<PayrollPaymentRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        
        PayrollPayment? payment = root?.PayrollPayment;
        
        if (payment == null)
        {
            throw new InvalidOperationException($"Payroll payment {id} not found");
        }
        
        this.cache.Set(cacheKey, payment, TimeSpan.FromMinutes(5));
        
        return payment;
    }

    /// <summary>
    /// Updates an existing payroll payment in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the payroll payment to update.</param>
    /// <param name="payment">The <see cref="PayrollPayment"/> object containing the updated payment details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="PayrollPayment"/> object.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="payment"/> is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/payroll_payments/{id} and invalidates the cache entries for the payment.
    /// </remarks>
    public async Task<PayrollPayment> UpdateAsync(string id, PayrollPayment payment)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(payment);
        await this.client.InitializeAndAuthorizeAsync();

        PayrollPaymentRoot data = new() { PayrollPayment = payment };
        using JsonContent content = JsonContent.Create(data, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.client.HttpClient.PutAsync(new Uri(this.client.ApiBaseUrl, $"/v2/payroll_payments/{id}"), content);
        response.EnsureSuccessStatusCode();

        PayrollPaymentRoot? root = await response.Content.ReadFromJsonAsync<PayrollPaymentRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove($"payroll_payment_{id}");
        this.cache.Remove("payroll_payments_all");

        return root?.PayrollPayment ?? throw new InvalidOperationException("Failed to update payroll payment");
    }

    /// <summary>
    /// Deletes a payroll payment from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the payroll payment to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls DELETE /v2/payroll_payments/{id} and invalidates the cache entries for the payment.
    /// The deletion is permanent and cannot be undone.
    /// </remarks>
    public async Task DeleteAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.DeleteAsync(new Uri(this.client.ApiBaseUrl, $"/v2/payroll_payments/{id}"));
        response.EnsureSuccessStatusCode();

        this.cache.Remove($"payroll_payment_{id}");
        this.cache.Remove("payroll_payments_all");
    }
}