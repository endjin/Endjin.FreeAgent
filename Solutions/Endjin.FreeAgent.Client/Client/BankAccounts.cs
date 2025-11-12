// <copyright file="BankAccounts.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class BankAccounts
{
    private const string BankAccountsEndPoint = "v2/bank_accounts";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    public BankAccounts(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    public async Task<BankAccount> CreateAsync(BankAccount bankAccount)
    {
        BankAccountRoot root = new() { BankAccount = bankAccount };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, BankAccountsEndPoint),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        BankAccountRoot? result = await response.Content.ReadFromJsonAsync<BankAccountRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.BankAccount ?? throw new InvalidOperationException("Failed to deserialize bank account response.");
    }

    public async Task<IEnumerable<BankAccount>> GetAllAsync(string view = "all")
    {
        string cacheKey = $"{BankAccountsEndPoint}/{view}";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<BankAccount>? results))
        {
            List<BankAccountsRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<BankAccountsRoot>(
                new Uri(this.freeAgentClient.ApiBaseUrl, $"{BankAccountsEndPoint}?view={view}"))
                .ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.BankAccounts)];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    public async Task<BankAccount> GetByIdAsync(string id)
    {
        string cacheKey = $"{BankAccountsEndPoint}/{id}";

        if (!this.cache.TryGetValue(cacheKey, out BankAccount? results))
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(
                new Uri(freeAgentClient.ApiBaseUrl, $"{BankAccountsEndPoint}/{id}")).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            BankAccountRoot? root = await response.Content.ReadFromJsonAsync<BankAccountRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            results = root?.BankAccount;
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? throw new InvalidOperationException($"Bank account with ID {id} not found.");
    }

    public async Task<BankAccount> UpdateAsync(string id, BankAccount bankAccount)
    {
        BankAccountRoot root = new() { BankAccount = bankAccount };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{BankAccountsEndPoint}/{id}"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        BankAccountRoot? result = await response.Content.ReadFromJsonAsync<BankAccountRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache for this bank account
        string cacheKey = $"{BankAccountsEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return result?.BankAccount ?? throw new InvalidOperationException("Failed to deserialize bank account response.");
    }

    public async Task DeleteAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.DeleteAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{BankAccountsEndPoint}/{id}")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        // Invalidate cache
        string cacheKey = $"{BankAccountsEndPoint}/{id}";
        this.cache.Remove(cacheKey);
    }
}