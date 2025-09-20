// <copyright file="BankTransactions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class BankTransactions
{
    private const string BankTransactionsEndPoint = "v2/bank_transactions";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    public BankTransactions(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    public async Task<BankTransaction> CreateAsync(BankTransaction bankTransaction)
    {
        BankTransactionRoot root = new() { BankTransaction = bankTransaction };

        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, BankTransactionsEndPoint),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        BankTransactionRoot? results = await response.Content.ReadFromJsonAsync<BankTransactionRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return results?.BankTransaction ?? throw new InvalidOperationException("Failed to deserialize bank transaction response.");
    }

    public async Task<IEnumerable<BankTransaction>> GetAllAsync(Uri? bankAccountUri = null, string view = "all")
    {
        string queryString = $"?view={view}";
        if (bankAccountUri != null)
        {
            queryString += $"&bank_account={Uri.EscapeDataString(bankAccountUri.ToString())}";
        }

        string cacheKey = $"{BankTransactionsEndPoint}{queryString}";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<BankTransaction>? results))
        {
            List<BankTransactionsRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<BankTransactionsRoot>(
                new Uri(this.freeAgentClient.ApiBaseUrl, $"{BankTransactionsEndPoint}{queryString}"))
                .ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.BankTransactions)];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    public async Task<IEnumerable<BankTransaction>> GetUnexplainedAsync(Uri? bankAccountUri = null) => await this.GetAllAsync(bankAccountUri, "unexplained").ConfigureAwait(false);

    public async Task<IEnumerable<BankTransaction>> GetExplainedAsync(Uri? bankAccountUri = null) => await this.GetAllAsync(bankAccountUri, "explained").ConfigureAwait(false);

    public async Task<BankTransaction> GetByIdAsync(string id)
    {
        string cacheKey = $"{BankTransactionsEndPoint}/{id}";

        if (!this.cache.TryGetValue(cacheKey, out BankTransaction? results))
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(
                new Uri(freeAgentClient.ApiBaseUrl, $"{BankTransactionsEndPoint}/{id}")).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            BankTransactionRoot? content = await response.Content.ReadFromJsonAsync<BankTransactionRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            results = content?.BankTransaction;
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? throw new InvalidOperationException($"Bank transaction with ID {id} not found.");
    }

    public async Task<BankTransaction> UpdateAsync(string id, BankTransaction bankTransaction)
    {
        BankTransactionRoot root = new() { BankTransaction = bankTransaction };

        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{BankTransactionsEndPoint}/{id}"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        BankTransactionRoot? results = await response.Content.ReadFromJsonAsync<BankTransactionRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache for this bank transaction
        string cacheKey = $"{BankTransactionsEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return results?.BankTransaction ?? throw new InvalidOperationException("Failed to deserialize bank transaction response.");
    }

    public async Task DeleteAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.DeleteAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{BankTransactionsEndPoint}/{id}")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        // Invalidate cache
        string cacheKey = $"{BankTransactionsEndPoint}/{id}";
        this.cache.Remove(cacheKey);
    }

    public async Task<IEnumerable<BankTransaction>> UploadStatementAsync(Uri bankAccountUri, string statementData, string fileType)
    {
        StatementUploadRoot statementRoot = new()
        {
            Statement = new StatementUpload
            {
                BankAccount = bankAccountUri.ToString(),
                Statement = statementData,
                FileType = fileType
            }
        };

        using JsonContent content = JsonContent.Create(statementRoot, options: SharedJsonOptions.SourceGenOptions);
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{BankTransactionsEndPoint}/statement"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        BankTransactionsRoot? results = await response.Content.ReadFromJsonAsync<BankTransactionsRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return results?.BankTransactions ?? [];
    }
}