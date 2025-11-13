// <copyright file="CreditNotes.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing credit notes via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides comprehensive access to FreeAgent credit notes, which are documents
/// issued to customers to reduce the amount owed on invoices. Credit notes can be issued for returns,
/// discounts, or corrections, and can be in various statuses (draft, sent, open, refunded).
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance. Cache entries are invalidated automatically
/// when credit notes are updated, deleted, or when status changes occur.
/// </para>
/// </remarks>
/// <seealso cref="CreditNote"/>
/// <seealso cref="CreditNoteItem"/>
/// <seealso cref="Contact"/>
/// <seealso cref="Invoice"/>
public class CreditNotes
{
    private const string CreditNotesEndPoint = "v2/credit_notes";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CreditNotes"/> class.
    /// </summary>
    /// <param name="freeAgentClient">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing credit note data.</param>
    public CreditNotes(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    /// <summary>
    /// Creates a new credit note in FreeAgent.
    /// </summary>
    /// <param name="creditNote">The <see cref="CreditNote"/> object containing the credit note details to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="CreditNote"/> object with server-assigned values (e.g., ID, URL, reference).
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/credit_notes to create a new credit note. Credit notes reduce the
    /// amount owed on invoices and can be issued for returns, discounts, or corrections.
    /// </remarks>
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

    /// <summary>
    /// Retrieves credit notes from FreeAgent filtered by view.
    /// </summary>
    /// <param name="view">The view filter to apply (e.g., "all", "draft", "sent", "open", "refunded"). Defaults to "all".</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="CreditNote"/> objects matching the specified view.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/credit_notes?view={view}, handles pagination automatically, and caches the
    /// result for 5 minutes.
    /// </remarks>
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

    /// <summary>
    /// Retrieves a specific credit note by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the credit note to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="CreditNote"/> object with the specified ID.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no credit note with the specified ID is found.</exception>
    /// <remarks>
    /// This method calls GET /v2/credit_notes/{id} and caches the result for 5 minutes.
    /// </remarks>
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

    /// <summary>
    /// Updates an existing credit note in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the credit note to update.</param>
    /// <param name="creditNote">The <see cref="CreditNote"/> object containing the updated credit note details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="CreditNote"/> object as returned by the API.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/credit_notes/{id} to update the credit note. The cache entry for this
    /// credit note is invalidated after a successful update.
    /// </remarks>
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

    /// <summary>
    /// Deletes a credit note from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the credit note to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls DELETE /v2/credit_notes/{id} to delete the credit note. The cache entry for this
    /// credit note is invalidated after successful deletion. Only credit notes in draft status can be deleted.
    /// </remarks>
    public async Task DeleteAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.DeleteAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{CreditNotesEndPoint}/{id}")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        // Invalidate cache
        string cacheKey = $"{CreditNotesEndPoint}/{id}";
        this.cache.Remove(cacheKey);
    }

    /// <summary>
    /// Marks a credit note as refunded in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the credit note to mark as refunded.</param>
    /// <param name="refundedOn">The date the credit note was refunded.</param>
    /// <param name="bankAccountUri">The URI of the bank account from which the refund was issued.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="CreditNote"/> object with its status changed to refunded.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/credit_notes/{id}/mark_as_refunded to record refund of the credit note.
    /// This creates a corresponding bank transaction. The cache entry for this credit note is invalidated.
    /// </remarks>
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

    /// <summary>
    /// Sends a credit note via email through FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the credit note to send.</param>
    /// <param name="email">The <see cref="InvoiceEmail"/> object containing email details (recipients, subject, body).</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="CreditNote"/> object with its status changed to sent.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/credit_notes/{id}/send_email to send the credit note. The credit note
    /// status is automatically changed to "sent". The cache entry for this credit note is invalidated.
    /// </remarks>
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