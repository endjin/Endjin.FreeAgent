// <copyright file="Payroll.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class Payroll
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    public Payroll(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

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
