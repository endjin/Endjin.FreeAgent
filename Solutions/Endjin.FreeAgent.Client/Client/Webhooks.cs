// <copyright file="Webhooks.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;
/// <summary>
/// Provides methods for managing webhooks via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent webhooks, which allow applications to receive
/// real-time notifications when events occur in FreeAgent (e.g., invoice created, contact updated).
/// Webhooks enable integration and automation by pushing event notifications to specified URLs.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance. Cache entries are invalidated automatically
/// when webhooks are created, updated, or deleted.
/// </para>
/// </remarks>
/// <seealso cref="Webhook"/>
public class Webhooks
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="Webhooks"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing webhook data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public Webhooks(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Creates a new webhook in FreeAgent.
    /// </summary>
    /// <param name="webhook">The <see cref="Webhook"/> object containing the webhook details to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="Webhook"/> object with server-assigned values (e.g., ID, URL).
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="webhook"/> is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/webhooks to create a new webhook subscription. The cache is invalidated
    /// to ensure subsequent queries return up-to-date data.
    /// </remarks>
    public async Task<Webhook> CreateAsync(Webhook webhook)
    {
        ArgumentNullException.ThrowIfNull(webhook);
        await this.client.InitializeAndAuthorizeAsync();

        WebhookRoot data = new() { Webhook = webhook };
        using JsonContent content = JsonContent.Create(data, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.client.HttpClient.PostAsync(
            new Uri(this.client.ApiBaseUrl, "/v2/webhooks"),
            content);
        response.EnsureSuccessStatusCode();

        WebhookRoot? root = await response.Content.ReadFromJsonAsync<WebhookRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove("webhooks_all");

        return root?.Webhook ?? throw new InvalidOperationException("Failed to create webhook");
    }

    /// <summary>
    /// Retrieves all webhooks from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// all <see cref="Webhook"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/webhooks and caches the result for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<Webhook>> GetAllAsync()
    {
        string cacheKey = "webhooks_all";

        if (this.cache.TryGetValue(cacheKey, out IEnumerable<Webhook>? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(
            new Uri(this.client.ApiBaseUrl, "/v2/webhooks"));
        response.EnsureSuccessStatusCode();

        WebhooksRoot? root = await response.Content.ReadFromJsonAsync<WebhooksRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        IEnumerable<Webhook> webhooks = root?.Webhooks ?? [];

        this.cache.Set(cacheKey, webhooks, TimeSpan.FromMinutes(5));

        return webhooks;
    }

    /// <summary>
    /// Retrieves a specific webhook by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the webhook to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="Webhook"/> object with the specified ID.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no webhook with the specified ID is found.</exception>
    /// <remarks>
    /// This method calls GET /v2/webhooks/{id} and caches the result for 5 minutes.
    /// </remarks>
    public async Task<Webhook> GetByIdAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        string cacheKey = $"webhook_{id}";

        if (this.cache.TryGetValue(cacheKey, out Webhook? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, $"/v2/webhooks/{id}"));
        response.EnsureSuccessStatusCode();

        WebhookRoot? root = await response.Content.ReadFromJsonAsync<WebhookRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        Webhook? webhook = root?.Webhook;

        if (webhook == null)
        {
            throw new InvalidOperationException($"Webhook {id} not found");
        }

        this.cache.Set(cacheKey, webhook, TimeSpan.FromMinutes(5));

        return webhook;
    }

    /// <summary>
    /// Updates an existing webhook in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the webhook to update.</param>
    /// <param name="webhook">The <see cref="Webhook"/> object containing the updated webhook details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="Webhook"/> object as returned by the API.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="webhook"/> is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/webhooks/{id} to update the webhook. The cache entries for this
    /// webhook and all webhook queries are invalidated after a successful update.
    /// </remarks>
    public async Task<Webhook> UpdateAsync(string id, Webhook webhook)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(webhook);

        await this.client.InitializeAndAuthorizeAsync();

        WebhookRoot data = new() { Webhook = webhook };
        using JsonContent content = JsonContent.Create(data, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.client.HttpClient.PutAsync(new Uri(this.client.ApiBaseUrl, $"/v2/webhooks/{id}"), content);
        response.EnsureSuccessStatusCode();

        WebhookRoot? root = await response.Content.ReadFromJsonAsync<WebhookRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove($"webhook_{id}");
        this.cache.Remove("webhooks_all");

        return root?.Webhook ?? throw new InvalidOperationException("Failed to update webhook");
    }

    /// <summary>
    /// Deletes a webhook from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the webhook to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls DELETE /v2/webhooks/{id} to delete the webhook subscription. The cache entries
    /// for this webhook and all webhook queries are invalidated after successful deletion.
    /// </remarks>
    public async Task DeleteAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.DeleteAsync(new Uri(this.client.ApiBaseUrl, $"/v2/webhooks/{id}"));
        response.EnsureSuccessStatusCode();

        this.cache.Remove($"webhook_{id}");
        this.cache.Remove("webhooks_all");
    }
}