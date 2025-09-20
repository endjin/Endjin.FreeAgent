// <copyright file="Bills.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class Bills
{
    private const string BillsEndPoint = "v2/bills";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    public Bills(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    public async Task<Bill> CreateAsync(Bill bill)
    {
        BillRoot root = new() { Bill = bill };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, BillsEndPoint),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        BillRoot? result = await response.Content.ReadFromJsonAsync<BillRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.Bill ?? throw new InvalidOperationException("Failed to deserialize bill response.");
    }

    public async Task<IEnumerable<Bill>> GetAllAsync(string view = "all")
    {
        string cacheKey = $"{BillsEndPoint}/{view}";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<Bill>? results))
        {
            List<BillsRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<BillsRoot>(
                new Uri(this.freeAgentClient.ApiBaseUrl, $"{BillsEndPoint}?view={view}"))
                .ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Bills)];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    public async Task<Bill> GetByIdAsync(string id)
    {
        string cacheKey = $"{BillsEndPoint}/{id}";

        if (!this.cache.TryGetValue(cacheKey, out Bill? results))
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(
                new Uri(freeAgentClient.ApiBaseUrl, $"{BillsEndPoint}/{id}")).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            BillRoot? root = await response.Content.ReadFromJsonAsync<BillRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            results = root?.Bill;
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? throw new InvalidOperationException($"Bill with ID {id} not found.");
    }

    public async Task<Bill> UpdateAsync(string id, Bill bill)
    {
        BillRoot root = new() { Bill = bill };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{BillsEndPoint}/{id}"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        BillRoot? result = await response.Content.ReadFromJsonAsync<BillRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache for this bill
        string cacheKey = $"{BillsEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return result?.Bill ?? throw new InvalidOperationException("Failed to deserialize bill response.");
    }

    public async Task DeleteAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.DeleteAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{BillsEndPoint}/{id}")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        // Invalidate cache
        string cacheKey = $"{BillsEndPoint}/{id}";
        this.cache.Remove(cacheKey);
    }

    public async Task<Bill> MarkAsPaidAsync(string id, DateOnly paidOn, Uri bankAccountUri)
    {
        BillPaymentRoot paymentRoot = new()
        {
            Bill = new BillPayment
            {
                PaidOn = paidOn.ToString("yyyy-MM-dd"),
                BankAccount = bankAccountUri.ToString()
            }
        };

        using JsonContent content = JsonContent.Create(paymentRoot, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{BillsEndPoint}/{id}/mark_as_paid"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        BillRoot? root = await response.Content.ReadFromJsonAsync<BillRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        string cacheKey = $"{BillsEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return root?.Bill ?? throw new InvalidOperationException("Failed to deserialize bill response.");
    }
}