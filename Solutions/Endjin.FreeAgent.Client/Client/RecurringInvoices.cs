// <copyright file="RecurringInvoices.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides read-only methods for retrieving recurring invoices via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent recurring invoices, which are templates for invoices
/// that are automatically generated on a regular schedule (monthly, quarterly, annually, etc.). Recurring
/// invoices can be active, draft, or inactive and can be used for subscription billing or regular services.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance. The FreeAgent Recurring Invoices API
/// is read-only and does not support creating, updating, or deleting recurring invoices.
/// </para>
/// </remarks>
/// <seealso cref="RecurringInvoice"/>
/// <seealso cref="Contact"/>
/// <seealso cref="Project"/>
public class RecurringInvoices
{
    private const string RecurringInvoicesEndPoint = "v2/recurring_invoices";

    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="RecurringInvoices"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing recurring invoice data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public RecurringInvoices(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    /// <summary>
    /// Retrieves recurring invoices from FreeAgent filtered by view.
    /// </summary>
    /// <param name="view">The view filter to apply. Valid values are "active", "draft", or "inactive". Defaults to "active".</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="RecurringInvoice"/> objects matching the specified view.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/recurring_invoices?view={view} and caches the result for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<RecurringInvoice>> GetAllAsync(string view = "active")
    {
        string cacheKey = $"recurring_invoices_{view}";

        if (this.cache.TryGetValue(cacheKey, out IEnumerable<RecurringInvoice>? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, $"{RecurringInvoicesEndPoint}?view={view}")).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        RecurringInvoicesRoot? root = await response.Content.ReadFromJsonAsync<RecurringInvoicesRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        IEnumerable<RecurringInvoice> invoices = root?.RecurringInvoices ?? [];

        this.cache.Set(cacheKey, invoices, this.cacheEntryOptions);

        return invoices;
    }

    /// <summary>
    /// Retrieves recurring invoices for a specific contact from FreeAgent.
    /// </summary>
    /// <param name="contactUri">The URI of the contact to filter recurring invoices by.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="RecurringInvoice"/> objects for the specified contact.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="contactUri"/> is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/recurring_invoices?contact={contactUri} and caches the result for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<RecurringInvoice>> GetAllByContactAsync(Uri contactUri)
    {
        ArgumentNullException.ThrowIfNull(contactUri);

        string cacheKey = $"recurring_invoices_contact_{contactUri}";

        if (this.cache.TryGetValue(cacheKey, out IEnumerable<RecurringInvoice>? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, $"{RecurringInvoicesEndPoint}?contact={Uri.EscapeDataString(contactUri.ToString())}")).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        RecurringInvoicesRoot? root = await response.Content.ReadFromJsonAsync<RecurringInvoicesRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        IEnumerable<RecurringInvoice> invoices = root?.RecurringInvoices ?? [];

        this.cache.Set(cacheKey, invoices, this.cacheEntryOptions);

        return invoices;
    }

    /// <summary>
    /// Retrieves a specific recurring invoice by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the recurring invoice to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="RecurringInvoice"/> object with the specified ID.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no recurring invoice with the specified ID is found.</exception>
    /// <remarks>
    /// This method calls GET /v2/recurring_invoices/{id} and caches the result for 5 minutes.
    /// </remarks>
    public async Task<RecurringInvoice> GetByIdAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        string cacheKey = $"recurring_invoice_{id}";

        if (this.cache.TryGetValue(cacheKey, out RecurringInvoice? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, $"{RecurringInvoicesEndPoint}/{id}")).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        RecurringInvoiceRoot? root = await response.Content.ReadFromJsonAsync<RecurringInvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        RecurringInvoice? invoice = root?.RecurringInvoice;

        if (invoice == null)
        {
            throw new InvalidOperationException($"Recurring invoice {id} not found");
        }

        this.cache.Set(cacheKey, invoice, this.cacheEntryOptions);

        return invoice;
    }

}