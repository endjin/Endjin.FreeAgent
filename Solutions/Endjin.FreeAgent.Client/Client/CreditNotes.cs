// <copyright file="CreditNotes.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class CreditNotes
{
    private const string CreditNotesEndPoint = "v2/credit_notes";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    public CreditNotes(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    public async Task<CreditNote> CreateAsync(CreditNote creditNote)
    {
        CreditNoteRoot root = new() { CreditNote = creditNote };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, CreditNotesEndPoint),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        CreditNoteRoot? result = await response.Content.ReadFromJsonAsync<CreditNoteRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.CreditNote ?? throw new InvalidOperationException("Failed to deserialize credit note response.");
    }

    public async Task<IEnumerable<CreditNote>> GetAllAsync(string view = "all")
    {
        string cacheKey = $"{CreditNotesEndPoint}/{view}";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<CreditNote>? results))
        {
            List<CreditNotesRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<CreditNotesRoot>(
                new Uri(this.freeAgentClient.ApiBaseUrl, $"{CreditNotesEndPoint}?view={view}"))
                .ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.CreditNotes)];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    public async Task<CreditNote> GetByIdAsync(string id)
    {
        string cacheKey = $"{CreditNotesEndPoint}/{id}";

        if (!this.cache.TryGetValue(cacheKey, out CreditNote? results))
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(
                new Uri(freeAgentClient.ApiBaseUrl, $"{CreditNotesEndPoint}/{id}")).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            CreditNoteRoot? root = await response.Content.ReadFromJsonAsync<CreditNoteRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            results = root?.CreditNote;
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? throw new InvalidOperationException($"Credit note with ID {id} not found.");
    }

    public async Task<CreditNote> UpdateAsync(string id, CreditNote creditNote)
    {
        CreditNoteRoot root = new() { CreditNote = creditNote };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{CreditNotesEndPoint}/{id}"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        CreditNoteRoot? result = await response.Content.ReadFromJsonAsync<CreditNoteRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache for this credit note
        string cacheKey = $"{CreditNotesEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return result?.CreditNote ?? throw new InvalidOperationException("Failed to deserialize credit note response.");
    }

    public async Task DeleteAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.DeleteAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{CreditNotesEndPoint}/{id}")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        // Invalidate cache
        string cacheKey = $"{CreditNotesEndPoint}/{id}";
        this.cache.Remove(cacheKey);
    }

    public async Task<CreditNote> MarkAsRefundedAsync(string id, DateOnly refundedOn, Uri bankAccountUri)
    {
        CreditNoteRefundRoot refundRoot = new()
        {
            CreditNote = new CreditNoteRefund
            {
                RefundedOn = refundedOn.ToString("yyyy-MM-dd"),
                BankAccount = bankAccountUri.ToString()
            }
        };

        using JsonContent content = JsonContent.Create(refundRoot, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{CreditNotesEndPoint}/{id}/mark_as_refunded"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        CreditNoteRoot? root = await response.Content.ReadFromJsonAsync<CreditNoteRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        string cacheKey = $"{CreditNotesEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return root?.CreditNote ?? throw new InvalidOperationException("Failed to deserialize credit note response.");
    }

    public async Task<CreditNote> SendEmailAsync(string id, InvoiceEmail email)
    {
        CreditNoteEmailRoot emailRoot = new()
        {
            CreditNote = new CreditNoteEmailWrapper { Email = email }
        };
        using JsonContent content = JsonContent.Create(emailRoot, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{CreditNotesEndPoint}/{id}/send_email"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        CreditNoteRoot? root = await response.Content.ReadFromJsonAsync<CreditNoteRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        string cacheKey = $"{CreditNotesEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return root?.CreditNote ?? throw new InvalidOperationException("Failed to deserialize credit note response.");
    }
}