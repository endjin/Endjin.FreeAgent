// <copyright file="InvoiceSettings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

using Microsoft.Extensions.Caching.Memory;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing invoice settings and templates in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// This client provides access to invoice configuration endpoints, including default additional text
/// that appears on all invoices. These settings help standardize invoice presentation across your organization.
/// </para>
/// <para>
/// All methods support optional response caching with a configurable cache duration.
/// </para>
/// <para>
/// API Endpoint: /v2/invoices
/// </para>
/// <para>
/// Minimum Access Level: Invoices
/// </para>
/// </remarks>
/// <seealso cref="InvoiceDefaultAdditionalText"/>
/// <seealso cref="Invoice"/>
public class InvoiceSettings
{
    private const string DefaultAdditionalTextEndpoint = "v2/invoices/default_additional_text";

    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="InvoiceSettings"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing invoice settings data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public InvoiceSettings(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Gets the default additional text that appears on all invoices.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the
    /// <see cref="InvoiceDefaultAdditionalText"/> with the current default text template.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method retrieves the default text that is automatically added to new invoices.
    /// This text typically includes payment terms, bank details, or other standard invoice footer information.
    /// </remarks>
    public async Task<InvoiceDefaultAdditionalText> GetDefaultAdditionalTextAsync()
    {
        string cacheKey = $"invoice_default_text";

        if (this.cache.TryGetValue(cacheKey, out InvoiceDefaultAdditionalText? cachedResult))
        {
            return cachedResult!;
        }

        await this.client.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, DefaultAdditionalTextEndpoint)).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        InvoiceDefaultAdditionalTextRoot? root = await response.Content.ReadFromJsonAsync<InvoiceDefaultAdditionalTextRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        InvoiceDefaultAdditionalText result = new() { Text = root?.DefaultAdditionalText };

        this.cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));

        return result;
    }

    /// <summary>
    /// Updates the default additional text that appears on all invoices.
    /// </summary>
    /// <param name="text">The new default text to set for invoices.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the
    /// updated <see cref="InvoiceDefaultAdditionalText"/>.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method updates the default text template that is automatically added to new invoices.
    /// Existing invoices are not affected by this change.
    /// </remarks>
    public async Task<InvoiceDefaultAdditionalText> UpdateDefaultAdditionalTextAsync(string text)
    {
        var wrapper = new InvoiceDefaultAdditionalTextRoot { DefaultAdditionalText = text };

        using JsonContent content = JsonContent.Create(wrapper, options: SharedJsonOptions.SourceGenOptions);

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.PutAsync(new Uri(this.client.ApiBaseUrl, DefaultAdditionalTextEndpoint), content).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        InvoiceDefaultAdditionalTextRoot? root = await response.Content.ReadFromJsonAsync<InvoiceDefaultAdditionalTextRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        InvoiceDefaultAdditionalText result = new() { Text = root?.DefaultAdditionalText };

        // Invalidate cache
        string cacheKey = $"invoice_default_text";
        this.cache.Remove(cacheKey);

        return result;
    }

    /// <summary>
    /// Deletes the default additional text for invoices.
    /// </summary>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method removes the default additional text template. After deletion, new invoices
    /// will not have any default additional text unless it is set again.
    /// </remarks>
    public async Task DeleteDefaultAdditionalTextAsync()
    {
        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.DeleteAsync(new Uri(this.client.ApiBaseUrl, DefaultAdditionalTextEndpoint)).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        // Invalidate cache
        string cacheKey = $"invoice_default_text";
        this.cache.Remove(cacheKey);
    }
}
