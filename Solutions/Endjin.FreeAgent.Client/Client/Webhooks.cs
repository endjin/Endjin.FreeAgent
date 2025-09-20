// <copyright file="Webhooks.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
namespace Endjin.FreeAgent.Client;

using System.Net.Http;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Caching.Memory;

public class Webhooks
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    public Webhooks(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

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
