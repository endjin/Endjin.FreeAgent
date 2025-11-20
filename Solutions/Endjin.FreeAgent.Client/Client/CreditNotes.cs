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
    /// Retrieves credit notes from FreeAgent with optional filters.
    /// </summary>
    /// <param name="view">The view filter to apply. Valid values: "all", "recent_open_or_overdue", "open", "overdue", "open_or_overdue", "draft", "refunded", or "last_N_months" (e.g., "last_3_months"). Defaults to "all".</param>
    /// <param name="nestedCreditNoteItems">Whether to include nested credit note items in the response. Defaults to false.</param>
    /// <param name="contact">Filter by contact URI.</param>
    /// <param name="project">Filter by project URI.</param>
    /// <param name="updatedSince">Filter by last update date - only return credit notes updated after this date.</param>
    /// <param name="sort">Sort order: "created_at", "-created_at", "updated_at", or "-updated_at" (prefix with - for descending). Default is "created_at".</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="CreditNote"/> objects matching the specified criteria.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/credit_notes with query parameters, handles pagination automatically,
    /// and caches the result for 5 minutes based on the query parameters combination.
    /// </remarks>
    public async Task<IEnumerable<CreditNote>> GetAllAsync(
        string view = "all",
        bool nestedCreditNoteItems = false,
        Uri? contact = null,
        Uri? project = null,
        DateTimeOffset? updatedSince = null,
        string? sort = null)
    {
        List<string> queryParams = new List<string> { $"view={view}" };

        if (nestedCreditNoteItems)
        {
            queryParams.Add("nested_credit_note_items=true");
        }

        if (contact != null)
        {
            queryParams.Add($"contact={Uri.EscapeDataString(contact.ToString())}");
        }

        if (project != null)
        {
            queryParams.Add($"project={Uri.EscapeDataString(project.ToString())}");
        }

        if (updatedSince.HasValue)
        {
            queryParams.Add($"updated_since={updatedSince.Value.UtcDateTime:yyyy-MM-ddTHH:mm:ssZ}");
        }

        if (!string.IsNullOrWhiteSpace(sort))
        {
            queryParams.Add($"sort={sort}");
        }

        string queryString = string.Join("&", queryParams);
        string cacheKey = $"{CreditNotesEndPoint}?{queryString}";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<CreditNote>? results))
        {
            List<CreditNotesRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<CreditNotesRoot>(
                new Uri(this.freeAgentClient.ApiBaseUrl, $"{CreditNotesEndPoint}?{queryString}"))
                .ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.CreditNotes)];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves all credit notes with a specific status from FreeAgent.
    /// </summary>
    /// <param name="status">The status to filter by (e.g., "draft", "open", "overdue", "refunded").</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="CreditNote"/> objects with the specified status.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method maps the status parameter to the corresponding view and calls <see cref="GetAllAsync"/>.
    /// Status values are case-insensitive. Unrecognized status values default to "all".
    /// </remarks>
    public async Task<IEnumerable<CreditNote>> GetAllByStatusAsync(string status)
    {
        string view = status.ToLowerInvariant() switch
        {
            "draft" => "draft",
            "open" => "open",
            "overdue" => "overdue",
            "refunded" => "refunded",
            _ => "all"
        };

        return await this.GetAllAsync(view).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves all credit notes associated with a specific contact from FreeAgent.
    /// </summary>
    /// <param name="contactUri">The URI of the contact to filter credit notes by.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="CreditNote"/> objects associated with the specified contact.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/credit_notes?contact={contactUri}, handles pagination automatically, and caches
    /// the result for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<CreditNote>> GetAllByContactAsync(Uri contactUri)
    {
        string cacheKey = $"{CreditNotesEndPoint}/contact/{contactUri}";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<CreditNote>? results))
        {
            List<CreditNotesRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<CreditNotesRoot>(
                new Uri(this.freeAgentClient.ApiBaseUrl, $"{CreditNotesEndPoint}?contact={Uri.EscapeDataString(contactUri.ToString())}"))
                .ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.CreditNotes)];
            this.cache.Set(cacheKey, results, this.cacheEntryOptions);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves all credit notes associated with a specific project from FreeAgent.
    /// </summary>
    /// <param name="projectUri">The URI of the project to filter credit notes by.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="CreditNote"/> objects associated with the specified project.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/credit_notes?project={projectUri}, handles pagination automatically, and caches
    /// the result for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<CreditNote>> GetAllByProjectAsync(Uri projectUri)
    {
        string cacheKey = $"{CreditNotesEndPoint}/project/{projectUri}";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<CreditNote>? results))
        {
            List<CreditNotesRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<CreditNotesRoot>(
                new Uri(this.freeAgentClient.ApiBaseUrl, $"{CreditNotesEndPoint}?project={Uri.EscapeDataString(projectUri.ToString())}"))
                .ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.CreditNotes)];
            this.cache.Set(cacheKey, results, this.cacheEntryOptions);
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
    /// Retrieves a credit note as a PDF document from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the credit note to retrieve as PDF.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// PDF document as a byte array.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the PDF content is null or invalid.</exception>
    /// <remarks>
    /// This method calls GET /v2/credit_notes/{id}/pdf to retrieve the credit note as a PDF document.
    /// The API returns a JSON response with base64-encoded PDF content, which is decoded before returning.
    /// The result is not cached as PDF generation may vary.
    /// </remarks>
    public async Task<byte[]> GetPdfAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{CreditNotesEndPoint}/{id}/pdf")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        CreditNotePdfRoot? result = await response.Content.ReadFromJsonAsync<CreditNotePdfRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        if (result?.Pdf?.Content is null)
        {
            throw new InvalidOperationException("The PDF content was not returned in the expected format.");
        }

        return Convert.FromBase64String(result.Pdf.Content);
    }

    /// <summary>
    /// Marks a credit note as sent in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the credit note to mark as sent.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="CreditNote"/> object with its status changed to sent.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/credit_notes/{id}/transitions/mark_as_sent to change the credit note
    /// status from draft to sent. The cache entry for this credit note is invalidated.
    /// </remarks>
    public async Task<CreditNote> MarkAsSentAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{CreditNotesEndPoint}/{id}/transitions/mark_as_sent"),
            JsonContent.Create(new { })).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        CreditNoteRoot? root = await response.Content.ReadFromJsonAsync<CreditNoteRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        string cacheKey = $"{CreditNotesEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return root?.CreditNote ?? throw new InvalidOperationException("Failed to deserialize credit note response.");
    }

    /// <summary>
    /// Marks a credit note as draft in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the credit note to mark as draft.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="CreditNote"/> object with its status changed to draft.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/credit_notes/{id}/transitions/mark_as_draft to change the credit note
    /// status back to draft from sent. The cache entry for this credit note is invalidated.
    /// </remarks>
    public async Task<CreditNote> MarkAsDraftAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{CreditNotesEndPoint}/{id}/transitions/mark_as_draft"),
            JsonContent.Create(new { })).ConfigureAwait(false);

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
    /// This method calls POST /v2/credit_notes/{id}/send_email to send the credit note. The credit note
    /// status is automatically changed to "sent". The cache entry for this credit note is invalidated.
    /// </remarks>
    public async Task<CreditNote> SendEmailAsync(string id, InvoiceEmail email)
    {
        CreditNoteEmailRoot emailRoot = new()
        {
            CreditNote = new CreditNoteEmailWrapper { Email = email }
        };
        using JsonContent content = JsonContent.Create(emailRoot, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
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