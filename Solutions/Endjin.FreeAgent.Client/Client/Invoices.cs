// <copyright file="Invoices.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class Invoices
{
    private const string InvoicesEndPoint = "v2/invoices";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    public Invoices(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    public async Task<Invoice> CreateAsync(Invoice invoice)
    {
        InvoiceRoot root = new() { Invoice = invoice };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, InvoicesEndPoint),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        InvoiceRoot? result = await response.Content.ReadFromJsonAsync<InvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.Invoice ?? throw new InvalidOperationException("Failed to deserialize invoice response.");
    }

    public async Task<IEnumerable<Invoice>> GetAllAsync(string view = "all")
    {
        string cacheKey = $"{InvoicesEndPoint}/{view}";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<Invoice>? results))
        {
            List<InvoicesRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<InvoicesRoot>(
                new Uri(this.freeAgentClient.ApiBaseUrl, $"{InvoicesEndPoint}?view={view}"))
                .ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Invoices)];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    public async Task<IEnumerable<Invoice>> GetAllByStatusAsync(string status)
    {
        string view = status.ToLowerInvariant() switch
        {
            "draft" => "draft",
            "sent" => "sent",
            "open" => "open",
            "overdue" => "overdue",
            "paid" => "paid",
            "scheduled" => "scheduled",
            _ => "all"
        };

        return await this.GetAllAsync(view).ConfigureAwait(false);
    }

    public async Task<IEnumerable<Invoice>> GetAllByContactAsync(Uri contactUri)
    {
        string cacheKey = $"{InvoicesEndPoint}/contact/{contactUri}";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<Invoice>? results))
        {
            List<InvoicesRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<InvoicesRoot>(
                new Uri(this.freeAgentClient.ApiBaseUrl, $"{InvoicesEndPoint}?contact={Uri.EscapeDataString(contactUri.ToString())}"))
                .ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Invoices)];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    public async Task<IEnumerable<Invoice>> GetAllByProjectAsync(Uri projectUri)
    {
        string cacheKey = $"{InvoicesEndPoint}/project/{projectUri}";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<Invoice>? results))
        {
            List<InvoicesRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<InvoicesRoot>(
                new Uri(this.freeAgentClient.ApiBaseUrl, $"{InvoicesEndPoint}?project={Uri.EscapeDataString(projectUri.ToString())}"))
                .ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Invoices)];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    public async Task<Invoice> GetByIdAsync(string id)
    {
        string cacheKey = $"{InvoicesEndPoint}/{id}";

        if (!this.cache.TryGetValue(cacheKey, out Invoice? results))
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(new Uri(freeAgentClient.ApiBaseUrl, $"{InvoicesEndPoint}/{id}")).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            InvoiceRoot? root = await response.Content.ReadFromJsonAsync<InvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            results = root?.Invoice;
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? throw new InvalidOperationException($"Invoice with ID {id} not found.");
    }

    public async Task<Invoice> UpdateAsync(string id, Invoice invoice)
    {
        InvoiceRoot root = new() { Invoice = invoice };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{InvoicesEndPoint}/{id}"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        InvoiceRoot? result = await response.Content.ReadFromJsonAsync<InvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache for this invoice
        string cacheKey = $"{InvoicesEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return result?.Invoice ?? throw new InvalidOperationException("Failed to deserialize invoice response.");
    }

    public async Task DeleteAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.DeleteAsync(new Uri(this.freeAgentClient.ApiBaseUrl, $"{InvoicesEndPoint}/{id}")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        // Invalidate cache
        string cacheKey = $"{InvoicesEndPoint}/{id}";
        this.cache.Remove(cacheKey);
    }

    public async Task<Invoice> SendEmailAsync(string id, InvoiceEmail email)
    {
        InvoiceEmailRoot emailRoot = new()
        {
            Invoice = new InvoiceEmailWrapper { Email = email }
        };
        
        using JsonContent content = JsonContent.Create(emailRoot, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{InvoicesEndPoint}/{id}/send_email"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        InvoiceRoot? root = await response.Content.ReadFromJsonAsync<InvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        string cacheKey = $"{InvoicesEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return root?.Invoice ?? throw new InvalidOperationException("Failed to deserialize invoice response.");
    }

    public async Task<Invoice> MarkAsSentAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{InvoicesEndPoint}/{id}/mark_as_sent"),
            JsonContent.Create(new { })).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        InvoiceRoot? root = await response.Content.ReadFromJsonAsync<InvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        string cacheKey = $"{InvoicesEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return root?.Invoice ?? throw new InvalidOperationException("Failed to deserialize invoice response.");
    }

    public async Task<Invoice> MarkAsPaidAsync(string id, DateOnly paidOn, Uri paidIntoBankAccount)
    {
        InvoicePaymentRoot paymentRoot = new()
        {
            Invoice = new InvoicePayment
            {
                PaidOn = paidOn.ToString("yyyy-MM-dd"),
                PaidIntoBankAccount = paidIntoBankAccount.ToString()
            }
        };

        using JsonContent content = JsonContent.Create(paymentRoot, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{InvoicesEndPoint}/{id}/mark_as_paid"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        InvoiceRoot? root = await response.Content.ReadFromJsonAsync<InvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        string cacheKey = $"{InvoicesEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return root?.Invoice ?? throw new InvalidOperationException("Failed to deserialize invoice response.");
    }

    public async Task<Invoice> MarkAsCancelledAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{InvoicesEndPoint}/{id}/mark_as_cancelled"),
            JsonContent.Create(new { })).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        InvoiceRoot? root = await response.Content.ReadFromJsonAsync<InvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        string cacheKey = $"{InvoicesEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return root?.Invoice ?? throw new InvalidOperationException("Failed to deserialize invoice response.");
    }

    public async Task<Invoice> MarkAsScheduledAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{InvoicesEndPoint}/{id}/mark_as_scheduled"),
            JsonContent.Create(new { })).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        InvoiceRoot? root = await response.Content.ReadFromJsonAsync<InvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        string cacheKey = $"{InvoicesEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return root?.Invoice ?? throw new InvalidOperationException("Failed to deserialize invoice response.");
    }

    public async Task<Invoice> MarkAsDraftAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{InvoicesEndPoint}/{id}/mark_as_draft"),
            JsonContent.Create(new { })).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        InvoiceRoot? root = await response.Content.ReadFromJsonAsync<InvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        string cacheKey = $"{InvoicesEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return root?.Invoice ?? throw new InvalidOperationException("Failed to deserialize invoice response.");
    }

    public async Task<byte[]> GetPdfAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(new Uri(this.freeAgentClient.ApiBaseUrl, $"{InvoicesEndPoint}/{id}/pdf")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
    }
}
