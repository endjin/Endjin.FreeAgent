// <copyright file="Invoices.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing invoices via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides comprehensive access to FreeAgent invoices, which represent sales documents
/// sent to customers for payment. Invoices can be in various statuses (Draft, Scheduled To Email, Open, Zero Value,
/// Overdue, Paid, Overpaid, Refunded, Written-off, Part written-off) and can be associated with contacts and projects.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance. Cache entries are invalidated automatically
/// when invoices are updated, deleted, or when status changes occur.
/// </para>
/// </remarks>
/// <seealso cref="Invoice"/>
/// <seealso cref="InvoiceItem"/>
/// <seealso cref="Contact"/>
/// <seealso cref="Project"/>
public class Invoices
{
    private const string InvoicesEndPoint = "v2/invoices";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Invoices"/> class.
    /// </summary>
    /// <param name="freeAgentClient">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing invoice data.</param>
    public Invoices(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    /// <summary>
    /// Creates a new invoice in FreeAgent.
    /// </summary>
    /// <param name="invoice">The <see cref="Invoice"/> object containing the invoice details to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="Invoice"/> object with server-assigned values (e.g., ID, URL, reference).
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/invoices to create a new invoice. The cache is not updated as
    /// only aggregate queries are cached.
    /// </remarks>
    public async Task<Invoice> CreateAsync(Invoice invoice)
    {
        await this.freeAgentClient.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        InvoiceRoot root = new() { Invoice = invoice };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, InvoicesEndPoint),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        InvoiceRoot? result = await response.Content.ReadFromJsonAsync<InvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.Invoice ?? throw new InvalidOperationException("Failed to deserialize invoice response.");
    }

    /// <summary>
    /// Retrieves invoices from FreeAgent filtered by view.
    /// </summary>
    /// <param name="view">
    /// The view filter to apply. Supported values:
    /// <list type="bullet">
    /// <item><description>"all" - All invoices (default)</description></item>
    /// <item><description>"recent_open_or_overdue" - Recently open or overdue invoices</description></item>
    /// <item><description>"open" - Open invoices</description></item>
    /// <item><description>"overdue" - Overdue invoices</description></item>
    /// <item><description>"open_or_overdue" - Open or overdue invoices</description></item>
    /// <item><description>"draft" - Draft invoices</description></item>
    /// <item><description>"paid" - Paid invoices</description></item>
    /// <item><description>"scheduled_to_email" - Invoices scheduled to be emailed</description></item>
    /// <item><description>"thank_you_emails" - Invoices with thank you emails enabled</description></item>
    /// <item><description>"reminder_emails" - Invoices with reminder emails enabled</description></item>
    /// <item><description>"last_N_months" - Invoices from the last N months (e.g., "last_3_months")</description></item>
    /// </list>
    /// </param>
    /// <param name="updatedSince">Optional filter to retrieve only invoices updated after this timestamp (ISO 8601 format).</param>
    /// <param name="sort">Optional sort parameter. Use "created_at", "-created_at", "updated_at", or "-updated_at" (prefix with '-' for descending).</param>
    /// <param name="nestedInvoiceItems">If true, includes invoice line items in the response. Defaults to false.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="Invoice"/> objects matching the specified criteria.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/invoices with the specified query parameters, handles pagination automatically,
    /// and caches the result for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<Invoice>> GetAllAsync(
        string view = "all",
        DateTimeOffset? updatedSince = null,
        string? sort = null,
        bool nestedInvoiceItems = false)
    {
        List<string> queryParams = [$"view={view}"];

        if (updatedSince.HasValue)
        {
            queryParams.Add($"updated_since={Uri.EscapeDataString(updatedSince.Value.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))}");
        }

        if (!string.IsNullOrEmpty(sort))
        {
            queryParams.Add($"sort={sort}");
        }

        if (nestedInvoiceItems)
        {
            queryParams.Add("nested_invoice_items=true");
        }

        string queryString = string.Join("&", queryParams);
        string cacheKey = $"{InvoicesEndPoint}?{queryString}";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<Invoice>? results))
        {
            List<InvoicesRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<InvoicesRoot>(
                new Uri(this.freeAgentClient.ApiBaseUrl, $"{InvoicesEndPoint}?{queryString}"))
                .ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Invoices)];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves invoices from FreeAgent filtered by status.
    /// </summary>
    /// <param name="status">The status to filter by (e.g., "draft", "open", "overdue", "paid", "scheduled_to_email").</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="Invoice"/> objects with the specified status.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method maps the status parameter to the corresponding view and calls <see cref="GetAllAsync"/>.
    /// Status values are case-insensitive. Unrecognized status values default to "all".
    /// </remarks>
    public async Task<IEnumerable<Invoice>> GetAllByStatusAsync(string status)
    {
        string view = status.ToLowerInvariant() switch
        {
            "draft" => "draft",
            "open" => "open",
            "overdue" => "overdue",
            "paid" => "paid",
            "scheduled_to_email" or "scheduled" => "scheduled_to_email",
            _ => "all"
        };

        return await this.GetAllAsync(view).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves all invoices associated with a specific contact from FreeAgent.
    /// </summary>
    /// <param name="contactUri">The URI of the contact to filter invoices by.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="Invoice"/> objects associated with the specified contact.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/invoices?contact={contactUri}, handles pagination automatically, and caches
    /// the result for 5 minutes.
    /// </remarks>
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

    /// <summary>
    /// Retrieves all invoices associated with a specific project from FreeAgent.
    /// </summary>
    /// <param name="projectUri">The URI of the project to filter invoices by.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="Invoice"/> objects associated with the specified project.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/invoices?project={projectUri}, handles pagination automatically, and caches
    /// the result for 5 minutes.
    /// </remarks>
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

    /// <summary>
    /// Retrieves a specific invoice by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the invoice to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="Invoice"/> object with the specified ID.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no invoice with the specified ID is found.</exception>
    /// <remarks>
    /// This method calls GET /v2/invoices/{id} and caches the result for 5 minutes.
    /// </remarks>
    public async Task<Invoice> GetByIdAsync(string id)
    {
        string cacheKey = $"{InvoicesEndPoint}/{id}";

        if (!this.cache.TryGetValue(cacheKey, out Invoice? results))
        {
            await this.freeAgentClient.InitializeAndAuthorizeAsync().ConfigureAwait(false);

            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(new Uri(this.freeAgentClient.ApiBaseUrl, $"{InvoicesEndPoint}/{id}")).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            InvoiceRoot? root = await response.Content.ReadFromJsonAsync<InvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            results = root?.Invoice;
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? throw new InvalidOperationException($"Invoice with ID {id} not found.");
    }

    /// <summary>
    /// Updates an existing invoice in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the invoice to update.</param>
    /// <param name="invoice">The <see cref="Invoice"/> object containing the updated invoice details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="Invoice"/> object as returned by the API.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/invoices/{id} to update the invoice. The cache entry for this invoice
    /// is invalidated after a successful update.
    /// </remarks>
    public async Task<Invoice> UpdateAsync(string id, Invoice invoice)
    {
        await this.freeAgentClient.InitializeAndAuthorizeAsync().ConfigureAwait(false);

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

    /// <summary>
    /// Deletes an invoice from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the invoice to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls DELETE /v2/invoices/{id} to delete the invoice. The cache entry for this invoice
    /// is invalidated after successful deletion. Only invoices in draft status can be deleted.
    /// </remarks>
    public async Task DeleteAsync(string id)
    {
        await this.freeAgentClient.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.DeleteAsync(new Uri(this.freeAgentClient.ApiBaseUrl, $"{InvoicesEndPoint}/{id}")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        // Invalidate cache
        string cacheKey = $"{InvoicesEndPoint}/{id}";
        this.cache.Remove(cacheKey);
    }

    /// <summary>
    /// Sends an invoice via email through FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the invoice to send.</param>
    /// <param name="email">The <see cref="InvoiceEmail"/> object containing email details (recipients, subject, body).</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="Invoice"/> object with its status changed to sent.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/invoices/{id}/send_email to send the invoice. The invoice status is
    /// automatically changed to "sent". The cache entry for this invoice is invalidated.
    /// </remarks>
    public async Task<Invoice> SendEmailAsync(string id, InvoiceEmail email)
    {
        await this.freeAgentClient.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        InvoiceEmailRoot emailRoot = new()
        {
            Invoice = new InvoiceEmailWrapper { Email = email }
        };

        using JsonContent content = JsonContent.Create(emailRoot, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{InvoicesEndPoint}/{id}/send_email"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        InvoiceRoot? root = await response.Content.ReadFromJsonAsync<InvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        string cacheKey = $"{InvoicesEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return root?.Invoice ?? throw new InvalidOperationException("Failed to deserialize invoice response.");
    }

    /// <summary>
    /// Marks an invoice as sent in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the invoice to mark as sent.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="Invoice"/> object with its status changed to Open.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/invoices/{id}/transitions/mark_as_sent to manually mark the invoice as sent without
    /// actually emailing it. The invoice status changes to Open and the SentAt timestamp is set.
    /// The cache entry for this invoice is invalidated.
    /// </remarks>
    public async Task<Invoice> MarkAsSentAsync(string id)
    {
        await this.freeAgentClient.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{InvoicesEndPoint}/{id}/transitions/mark_as_sent"),
            JsonContent.Create(new object())).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        InvoiceRoot? root = await response.Content.ReadFromJsonAsync<InvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        string cacheKey = $"{InvoicesEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return root?.Invoice ?? throw new InvalidOperationException("Failed to deserialize invoice response.");
    }

    /// <summary>
    /// Cancels a scheduled invoice and returns it to draft status in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the invoice to cancel.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="Invoice"/> object with its status changed to Draft.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/invoices/{id}/transitions/mark_as_cancelled to cancel a scheduled invoice,
    /// returning it to Draft status for further editing or deletion. The cache entry for this invoice is invalidated.
    /// </remarks>
    public async Task<Invoice> MarkAsCancelledAsync(string id)
    {
        await this.freeAgentClient.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{InvoicesEndPoint}/{id}/transitions/mark_as_cancelled"),
            JsonContent.Create(new object())).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        InvoiceRoot? root = await response.Content.ReadFromJsonAsync<InvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        string cacheKey = $"{InvoicesEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return root?.Invoice ?? throw new InvalidOperationException("Failed to deserialize invoice response.");
    }

    /// <summary>
    /// Marks an invoice as scheduled for emailing in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the invoice to mark as scheduled.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="Invoice"/> object with its status changed to Scheduled To Email.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/invoices/{id}/transitions/mark_as_scheduled to schedule the invoice for later sending.
    /// The cache entry for this invoice is invalidated.
    /// </remarks>
    public async Task<Invoice> MarkAsScheduledAsync(string id)
    {
        await this.freeAgentClient.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{InvoicesEndPoint}/{id}/transitions/mark_as_scheduled"),
            JsonContent.Create(new object())).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        InvoiceRoot? root = await response.Content.ReadFromJsonAsync<InvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        string cacheKey = $"{InvoicesEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return root?.Invoice ?? throw new InvalidOperationException("Failed to deserialize invoice response.");
    }

    /// <summary>
    /// Marks an invoice as draft in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the invoice to mark as draft.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="Invoice"/> object with its status changed to draft.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/invoices/{id}/transitions/mark_as_draft to return the invoice to draft status, allowing
    /// further editing. The cache entry for this invoice is invalidated.
    /// </remarks>
    public async Task<Invoice> MarkAsDraftAsync(string id)
    {
        await this.freeAgentClient.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{InvoicesEndPoint}/{id}/transitions/mark_as_draft"),
            JsonContent.Create(new object())).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        InvoiceRoot? root = await response.Content.ReadFromJsonAsync<InvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        string cacheKey = $"{InvoicesEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return root?.Invoice ?? throw new InvalidOperationException("Failed to deserialize invoice response.");
    }

    /// <summary>
    /// Retrieves the PDF representation of an invoice from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the invoice to retrieve as PDF.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a byte array
    /// with the PDF content of the invoice.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the PDF content cannot be decoded.</exception>
    /// <remarks>
    /// This method calls GET /v2/invoices/{id}/pdf to retrieve the invoice as a PDF document. The API
    /// returns a JSON response with base64-encoded PDF content which is decoded to raw bytes. The result
    /// is not cached as PDF generation may vary.
    /// </remarks>
    public async Task<byte[]> GetPdfAsync(string id)
    {
        await this.freeAgentClient.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(new Uri(this.freeAgentClient.ApiBaseUrl, $"{InvoicesEndPoint}/{id}/pdf")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        InvoicePdfRoot? root = await response.Content.ReadFromJsonAsync<InvoicePdfRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        string? base64Content = root?.Pdf?.Content;
        if (string.IsNullOrEmpty(base64Content))
        {
            throw new InvalidOperationException("Failed to retrieve PDF content from response.");
        }

        return Convert.FromBase64String(base64Content);
    }

    /// <summary>
    /// Creates a duplicate of an existing invoice in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the invoice to duplicate.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// newly created <see cref="Invoice"/> object with Draft status and the same details as the original.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/invoices/{id}/duplicate to create a copy of the invoice.
    /// The new invoice will have Draft status with all the same line items and details.
    /// </remarks>
    public async Task<Invoice> DuplicateAsync(string id)
    {
        await this.freeAgentClient.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{InvoicesEndPoint}/{id}/duplicate"),
            JsonContent.Create(new object())).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        InvoiceRoot? result = await response.Content.ReadFromJsonAsync<InvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.Invoice ?? throw new InvalidOperationException("Failed to deserialize invoice response.");
    }

    /// <summary>
    /// Converts a negative draft invoice to a credit note in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the invoice to convert.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// converted invoice (now a credit note) object.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/invoices/{id}/transitions/convert_to_credit_note to convert a draft invoice
    /// with negative values to a credit note. The invoice must be in draft status and have a negative total.
    /// The cache entry for this invoice is invalidated.
    /// </remarks>
    public async Task<Invoice> ConvertToCreditNoteAsync(string id)
    {
        await this.freeAgentClient.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{InvoicesEndPoint}/{id}/transitions/convert_to_credit_note"),
            JsonContent.Create(new object())).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        InvoiceRoot? root = await response.Content.ReadFromJsonAsync<InvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        string cacheKey = $"{InvoicesEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return root?.Invoice ?? throw new InvalidOperationException("Failed to deserialize invoice response.");
    }

    /// <summary>
    /// Initiates a GoCardless Direct Debit payment for an invoice in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the invoice to initiate payment for.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls POST /v2/invoices/{id}/direct_debit to initiate a GoCardless Direct Debit payment.
    /// The invoice must have an open status and the customer must have an active GoCardless mandate.
    /// </remarks>
    public async Task InitiateDirectDebitAsync(string id)
    {
        await this.freeAgentClient.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{InvoicesEndPoint}/{id}/direct_debit"),
            JsonContent.Create(new object())).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Retrieves the activity timeline for all invoices from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="InvoiceTimelineEntry"/> objects representing the chronological activity log.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/invoices/timeline to retrieve a chronological log of all invoice-related
    /// activities including creation, updates, emails sent, payments received, and status changes.
    /// The timeline provides a complete audit trail for invoice lifecycle management.
    /// </remarks>
    public async Task<IEnumerable<InvoiceTimelineEntry>> GetTimelineAsync()
    {
        await this.freeAgentClient.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{InvoicesEndPoint}/timeline")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        InvoiceTimelineRoot? root = await response.Content.ReadFromJsonAsync<InvoiceTimelineRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return root?.TimelineEntries ?? [];
    }
}