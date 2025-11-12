// <copyright file="RecurringInvoices.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class RecurringInvoices
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    public RecurringInvoices(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<RecurringInvoice> CreateAsync(RecurringInvoice invoice)
    {
        ArgumentNullException.ThrowIfNull(invoice);

        RecurringInvoiceRoot data = new() { RecurringInvoice = invoice };
        using JsonContent content = JsonContent.Create(data, options: SharedJsonOptions.SourceGenOptions);

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.PostAsync("/v2/recurring_invoices", content);
        response.EnsureSuccessStatusCode();

        RecurringInvoiceRoot? root = await response.Content.ReadFromJsonAsync<RecurringInvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove("recurring_invoices_all");

        return root?.RecurringInvoice ?? throw new InvalidOperationException("Failed to create recurring invoice");
    }

    public async Task<IEnumerable<RecurringInvoice>> GetAllAsync(string view = "active")
    {
        string cacheKey = $"recurring_invoices_{view}";
        
        if (this.cache.TryGetValue(cacheKey, out IEnumerable<RecurringInvoice>? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, $"/v2/recurring_invoices?view={view}"));
        response.EnsureSuccessStatusCode();
        
        RecurringInvoicesRoot? root = await response.Content.ReadFromJsonAsync<RecurringInvoicesRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        
        IEnumerable<RecurringInvoice> invoices = root?.RecurringInvoices ?? [];
        
        this.cache.Set(cacheKey, invoices, TimeSpan.FromMinutes(5));
        
        return invoices;
    }

    public async Task<RecurringInvoice> GetByIdAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        
        string cacheKey = $"recurring_invoice_{id}";
        
        if (this.cache.TryGetValue(cacheKey, out RecurringInvoice? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, $"/v2/recurring_invoices/{id}"));
        response.EnsureSuccessStatusCode();
        
        RecurringInvoiceRoot? root = await response.Content.ReadFromJsonAsync<RecurringInvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        
        RecurringInvoice? invoice = root?.RecurringInvoice;
        
        if (invoice == null)
        {
            throw new InvalidOperationException($"Recurring invoice {id} not found");
        }
        
        this.cache.Set(cacheKey, invoice, TimeSpan.FromMinutes(5));
        
        return invoice;
    }

    public async Task<RecurringInvoice> UpdateAsync(string id, RecurringInvoice invoice)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(invoice);

        RecurringInvoiceRoot data = new() { RecurringInvoice = invoice };
        using JsonContent content = JsonContent.Create(data, options: SharedJsonOptions.SourceGenOptions);

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.PutAsync($"/v2/recurring_invoices/{id}", content);
        response.EnsureSuccessStatusCode();

        RecurringInvoiceRoot? root = await response.Content.ReadFromJsonAsync<RecurringInvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove($"recurring_invoice_{id}");
        this.cache.Remove("recurring_invoices_active");
        this.cache.Remove("recurring_invoices_all");

        return root?.RecurringInvoice ?? throw new InvalidOperationException("Failed to update recurring invoice");
    }

    public async Task DeleteAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.DeleteAsync(new Uri(this.client.ApiBaseUrl, $"/v2/recurring_invoices/{id}"));
        response.EnsureSuccessStatusCode();

        this.cache.Remove($"recurring_invoice_{id}");
        this.cache.Remove("recurring_invoices_active");
        this.cache.Remove("recurring_invoices_all");
    }

    public async Task<RecurringInvoice> ActivateAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.PutAsync($"/v2/recurring_invoices/{id}/activate", null);
        response.EnsureSuccessStatusCode();

        RecurringInvoiceRoot? root = await response.Content.ReadFromJsonAsync<RecurringInvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove($"recurring_invoice_{id}");
        this.cache.Remove("recurring_invoices_active");
        this.cache.Remove("recurring_invoices_all");

        return root?.RecurringInvoice ?? throw new InvalidOperationException("Failed to activate recurring invoice");
    }

    public async Task<RecurringInvoice> DeactivateAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.PutAsync($"/v2/recurring_invoices/{id}/deactivate", null);
        response.EnsureSuccessStatusCode();

        RecurringInvoiceRoot? root = await response.Content.ReadFromJsonAsync<RecurringInvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove($"recurring_invoice_{id}");
        this.cache.Remove("recurring_invoices_active");
        this.cache.Remove("recurring_invoices_all");

        return root?.RecurringInvoice ?? throw new InvalidOperationException("Failed to deactivate recurring invoice");
    }
}
