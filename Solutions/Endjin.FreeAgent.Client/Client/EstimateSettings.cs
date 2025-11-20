// <copyright file="EstimateSettings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing estimate settings and templates in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// This client provides access to estimate configuration endpoints, including default additional text
/// that appears on all estimates. These settings help standardize estimate presentation across your organization.
/// </para>
/// <para>
/// All methods support optional response caching with a configurable cache duration.
/// </para>
/// <para>
/// API Endpoint: /v2/estimates
/// </para>
/// <para>
/// Minimum Access Level: Invoices
/// </para>
/// </remarks>
/// <seealso cref="EstimateDefaultAdditionalText"/>
/// <seealso cref="Estimate"/>
public class EstimateSettings
{
    private const string DefaultAdditionalTextEndpoint = "/v2/estimates/default_additional_text";

    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="EstimateSettings"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing estimate settings data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public EstimateSettings(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Gets the default additional text that appears on all estimates.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the
    /// <see cref="EstimateDefaultAdditionalText"/> with the current default text template.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method retrieves the default text that is automatically added to new estimates.
    /// This text typically includes terms and conditions, validity period, or other standard estimate footer information.
    /// </remarks>
    public async Task<EstimateDefaultAdditionalText> GetDefaultAdditionalTextAsync()
    {
        string cacheKey = $"estimate_default_text";

        if (this.cache.TryGetValue(cacheKey, out EstimateDefaultAdditionalText? cachedResult))
        {
            return cachedResult!;
        }

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, DefaultAdditionalTextEndpoint)).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        EstimateDefaultAdditionalTextRoot? root = await response.Content.ReadFromJsonAsync<EstimateDefaultAdditionalTextRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        EstimateDefaultAdditionalText result = root?.Estimate ?? new EstimateDefaultAdditionalText();

        this.cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));

        return result;
    }

    /// <summary>
    /// Updates the default additional text that appears on all estimates.
    /// </summary>
    /// <param name="text">The new default text to set for estimates.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the
    /// updated <see cref="EstimateDefaultAdditionalText"/>.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method updates the default text template that is automatically added to new estimates.
    /// Existing estimates are not affected by this change.
    /// </remarks>
    public async Task<EstimateDefaultAdditionalText> UpdateDefaultAdditionalTextAsync(string text)
    {
        EstimateDefaultAdditionalText textObject = new() { Text = text };
        EstimateDefaultAdditionalTextRoot wrapper = new() { Estimate = textObject };

        using JsonContent content = JsonContent.Create(wrapper, options: SharedJsonOptions.SourceGenOptions);

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.PutAsync(new Uri(this.client.ApiBaseUrl, DefaultAdditionalTextEndpoint), content).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        EstimateDefaultAdditionalTextRoot? root = await response.Content.ReadFromJsonAsync<EstimateDefaultAdditionalTextRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        EstimateDefaultAdditionalText result = root?.Estimate ?? new EstimateDefaultAdditionalText();

        // Invalidate cache
        string cacheKey = $"estimate_default_text";
        this.cache.Remove(cacheKey);

        return result;
    }

    /// <summary>
    /// Deletes the default additional text for estimates.
    /// </summary>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method removes the default additional text template. After deletion, new estimates
    /// will not have any default additional text unless it is set again.
    /// </remarks>
    public async Task DeleteDefaultAdditionalTextAsync()
    {
        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.DeleteAsync(new Uri(this.client.ApiBaseUrl, DefaultAdditionalTextEndpoint)).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        // Invalidate cache
        string cacheKey = $"estimate_default_text";
        this.cache.Remove(cacheKey);
    }
}
