// <copyright file="BankTransactionExplanations.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class BankTransactionExplanations
{
    private const string BankTransactionExplanationsEndPoint = "v2/bank_transaction_explanations";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    public BankTransactionExplanations(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    public async Task<BankTransactionExplanation> CreateAsync(BankTransactionExplanation explanation)
    {
        BankTransactionExplanationRoot root = new() { BankTransactionExplanation = explanation };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, BankTransactionExplanationsEndPoint),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        BankTransactionExplanationRoot? result = await response.Content.ReadFromJsonAsync<BankTransactionExplanationRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.BankTransactionExplanation ?? throw new InvalidOperationException("Failed to deserialize bank transaction explanation response.");
    }

    public async Task<IEnumerable<BankTransactionExplanation>> GetAllAsync(Uri? bankTransactionUri = null)
    {
        string queryString = "";
        if (bankTransactionUri != null)
        {
            queryString = $"?bank_transaction={Uri.EscapeDataString(bankTransactionUri.ToString())}";
        }

        string cacheKey = $"{BankTransactionExplanationsEndPoint}{queryString}";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<BankTransactionExplanation>? results))
        {
            List<BankTransactionExplanationsRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<BankTransactionExplanationsRoot>(
                new Uri(this.freeAgentClient.ApiBaseUrl, $"{BankTransactionExplanationsEndPoint}{queryString}"))
                .ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.BankTransactionExplanations)];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    public async Task<BankTransactionExplanation> GetByIdAsync(string id)
    {
        string cacheKey = $"{BankTransactionExplanationsEndPoint}/{id}";

        if (!this.cache.TryGetValue(cacheKey, out BankTransactionExplanation? results))
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(
                new Uri(freeAgentClient.ApiBaseUrl, $"{BankTransactionExplanationsEndPoint}/{id}")).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            BankTransactionExplanationRoot? root = await response.Content.ReadFromJsonAsync<BankTransactionExplanationRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            results = root?.BankTransactionExplanation;
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? throw new InvalidOperationException($"Bank transaction explanation with ID {id} not found.");
    }

    public async Task<BankTransactionExplanation> UpdateAsync(string id, BankTransactionExplanation explanation)
    {
        BankTransactionExplanationRoot root = new() { BankTransactionExplanation = explanation };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{BankTransactionExplanationsEndPoint}/{id}"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        BankTransactionExplanationRoot? result = await response.Content.ReadFromJsonAsync<BankTransactionExplanationRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache for this explanation
        string cacheKey = $"{BankTransactionExplanationsEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return result?.BankTransactionExplanation ?? throw new InvalidOperationException("Failed to deserialize bank transaction explanation response.");
    }

    public async Task DeleteAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.DeleteAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{BankTransactionExplanationsEndPoint}/{id}")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        // Invalidate cache
        string cacheKey = $"{BankTransactionExplanationsEndPoint}/{id}";
        this.cache.Remove(cacheKey);
    }
}