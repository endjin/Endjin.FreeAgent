// <copyright file="RecurringInvoices.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing recurring invoices via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent recurring invoices, which are templates for invoices
/// that are automatically generated on a regular schedule (monthly, quarterly, annually, etc.). Recurring
/// invoices can be active or inactive and can be used for subscription billing or regular services.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance. Cache entries are invalidated automatically
/// when recurring invoices are created, updated, deleted, or when their status changes.
/// </para>
/// </remarks>
/// <seealso cref="RecurringInvoice"/>
/// <seealso cref="Contact"/>
/// <seealso cref="Project"/>
public class RecurringInvoices
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

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
    }

    /// <summary>
    /// Creates a new recurring invoice in FreeAgent.
    /// </summary>
    /// <param name="invoice">The <see cref="RecurringInvoice"/> object containing the recurring invoice details to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="RecurringInvoice"/> object with server-assigned values (e.g., ID, URL).
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="invoice"/> is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/recurring_invoices to create a new recurring invoice template.
    /// The cache is invalidated to ensure subsequent queries return up-to-date data.
    /// </remarks>
    public async Task<RecurringInvoice> CreateAsync(RecurringInvoice invoice)
    {
        ArgumentNullException.ThrowIfNull(invoice);

        RecurringInvoiceRoot data = new() { RecurringInvoice = invoice };
        using JsonContent content = JsonContent.Create(data, options: SharedJsonOptions.SourceGenOptions);

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.PostAsync(new Uri(this.client.ApiBaseUrl, "/v2/recurring_invoices"), content);
        response.EnsureSuccessStatusCode();

        RecurringInvoiceRoot? root = await response.Content.ReadFromJsonAsync<RecurringInvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove("recurring_invoices_all");

        return root?.RecurringInvoice ?? throw new InvalidOperationException("Failed to create recurring invoice");
    }

    /// <summary>
    /// Retrieves recurring invoices from FreeAgent filtered by view.
    /// </summary>
    /// <param name="view">The view filter to apply (e.g., "active", "all"). Defaults to "active".</param>
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

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, $"/v2/recurring_invoices?view={view}"));
        response.EnsureSuccessStatusCode();
        
        RecurringInvoicesRoot? root = await response.Content.ReadFromJsonAsync<RecurringInvoicesRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        
        IEnumerable<RecurringInvoice> invoices = root?.RecurringInvoices ?? [];
        
        this.cache.Set(cacheKey, invoices, TimeSpan.FromMinutes(5));
        
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

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, $"/v2/recurring_invoices/{id}"));
        response.EnsureSuccessStatusCode();

        RecurringInvoiceRoot? root = await response.Content.ReadFromJsonAsync<RecurringInvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        RecurringInvoice? invoice = root?.RecurringInvoice;

        if (invoice == null)
        {
            throw new InvalidOperationException($"Recurring invoice {id} not found");
        }

        this.cache.Set(cacheKey, invoice, TimeSpan.FromMinutes(5));

        return invoice;
    }

    /// <summary>
    /// Updates an existing recurring invoice in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the recurring invoice to update.</param>
    /// <param name="invoice">The <see cref="RecurringInvoice"/> object containing the updated recurring invoice details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="RecurringInvoice"/> object as returned by the API.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="invoice"/> is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/recurring_invoices/{id} to update the recurring invoice. The cache entries
    /// for this recurring invoice and all recurring invoice queries are invalidated after a successful update.
    /// </remarks>
    public async Task<RecurringInvoice> UpdateAsync(string id, RecurringInvoice invoice)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(invoice);

        RecurringInvoiceRoot data = new() { RecurringInvoice = invoice };
        using JsonContent content = JsonContent.Create(data, options: SharedJsonOptions.SourceGenOptions);

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.PutAsync(new Uri(this.client.ApiBaseUrl, $"/v2/recurring_invoices/{id}"), content);
        response.EnsureSuccessStatusCode();

        RecurringInvoiceRoot? root = await response.Content.ReadFromJsonAsync<RecurringInvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove($"recurring_invoice_{id}");
        this.cache.Remove("recurring_invoices_active");
        this.cache.Remove("recurring_invoices_all");

        return root?.RecurringInvoice ?? throw new InvalidOperationException("Failed to update recurring invoice");
    }

    /// <summary>
    /// Deletes a recurring invoice from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the recurring invoice to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls DELETE /v2/recurring_invoices/{id} to delete the recurring invoice. The cache
    /// entries for this recurring invoice and all recurring invoice queries are invalidated after successful deletion.
    /// </remarks>
    public async Task DeleteAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.DeleteAsync(new Uri(this.client.ApiBaseUrl, $"/v2/recurring_invoices/{id}"));
        response.EnsureSuccessStatusCode();

        this.cache.Remove($"recurring_invoice_{id}");
        this.cache.Remove("recurring_invoices_active");
        this.cache.Remove("recurring_invoices_all");
    }

    /// <summary>
    /// Activates a recurring invoice in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the recurring invoice to activate.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="RecurringInvoice"/> object with its status changed to active.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/recurring_invoices/{id}/activate to activate the recurring invoice,
    /// allowing it to generate invoices on its schedule. The cache entries are invalidated.
    /// </remarks>
    public async Task<RecurringInvoice> ActivateAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.PutAsync(new Uri(this.client.ApiBaseUrl, $"/v2/recurring_invoices/{id}/activate"), null);
        response.EnsureSuccessStatusCode();

        RecurringInvoiceRoot? root = await response.Content.ReadFromJsonAsync<RecurringInvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove($"recurring_invoice_{id}");
        this.cache.Remove("recurring_invoices_active");
        this.cache.Remove("recurring_invoices_all");

        return root?.RecurringInvoice ?? throw new InvalidOperationException("Failed to activate recurring invoice");
    }

    /// <summary>
    /// Deactivates a recurring invoice in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the recurring invoice to deactivate.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="RecurringInvoice"/> object with its status changed to inactive.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/recurring_invoices/{id}/deactivate to deactivate the recurring invoice,
    /// preventing it from generating new invoices. The cache entries are invalidated.
    /// </remarks>
    public async Task<RecurringInvoice> DeactivateAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.PutAsync(new Uri(this.client.ApiBaseUrl, $"/v2/recurring_invoices/{id}/deactivate"), null);
        response.EnsureSuccessStatusCode();

        RecurringInvoiceRoot? root = await response.Content.ReadFromJsonAsync<RecurringInvoiceRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove($"recurring_invoice_{id}");
        this.cache.Remove("recurring_invoices_active");
        this.cache.Remove("recurring_invoices_all");

        return root?.RecurringInvoice ?? throw new InvalidOperationException("Failed to deactivate recurring invoice");
    }
}