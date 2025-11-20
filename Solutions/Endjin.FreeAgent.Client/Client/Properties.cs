// <copyright file="Properties.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Concurrent;
using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing properties via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent properties, which represent rental or investment
/// properties for UK unincorporated landlords. Properties can be associated with income and expenses
/// for property management purposes.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance while maintaining reasonable data freshness.
/// </para>
/// <para>
/// Note: This endpoint is only available for companies of type <c>UkUnincorporatedLandlord</c>.
/// Other company types cannot have or create properties.
/// </para>
/// </remarks>
/// <seealso cref="Property"/>
public class Properties
{
    private const string PropertiesEndPoint = "v2/properties";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();
    private readonly ConcurrentDictionary<string, byte> cacheKeys = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Properties"/> class.
    /// </summary>
    /// <param name="freeAgentClient">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing property data.</param>
    public Properties(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    /// <summary>
    /// Adds an item to the cache and tracks the cache key for later invalidation.
    /// </summary>
    /// <typeparam name="T">The type of the value to cache.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The value to cache.</param>
    private void SetCache<T>(string key, T value)
    {
        this.cache.Set(key, value, this.cacheEntryOptions);
        this.cacheKeys.TryAdd(key, 0);
    }

    /// <summary>
    /// Invalidates all property-related cache entries.
    /// </summary>
    /// <remarks>
    /// This method removes all cached entries whose keys start with the properties endpoint path,
    /// ensuring that any cached property data (individual properties, lists) is cleared
    /// when properties are created, updated, or deleted.
    /// </remarks>
    private void InvalidateAllPropertiesCaches()
    {
        foreach (string key in this.cacheKeys.Keys)
        {
            if (key.StartsWith(PropertiesEndPoint, StringComparison.Ordinal))
            {
                this.cache.Remove(key);
                this.cacheKeys.TryRemove(key, out _);
            }
        }
    }

    /// <summary>
    /// Creates a new property in FreeAgent.
    /// </summary>
    /// <param name="property">The <see cref="Property"/> object containing the property details to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="Property"/> object with server-assigned values (e.g., ID, URL).
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/properties to create a new property. The cache is invalidated
    /// after a successful creation.
    /// </remarks>
    public async Task<Property> CreateAsync(Property property)
    {
        PropertyRoot root = new() { Property = property };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, PropertiesEndPoint),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        PropertyRoot? result = await response.Content.ReadFromJsonAsync<PropertyRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate all property-related caches since the new property may be included in various cached queries
        this.InvalidateAllPropertiesCaches();

        return result?.Property ?? throw new InvalidOperationException("Failed to deserialize property response.");
    }

    /// <summary>
    /// Retrieves all properties from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="Property"/> objects in the FreeAgent account.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/properties, handles pagination automatically, and caches the
    /// result for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<Property>> GetAllAsync()
    {
        string cacheKey = PropertiesEndPoint;

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<Property>? results))
        {
            List<PropertiesRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<PropertiesRoot>(
                new Uri(this.freeAgentClient.ApiBaseUrl, PropertiesEndPoint))
                .ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Properties)];
            this.SetCache(cacheKey, results);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves a specific property by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the property to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="Property"/> object with the specified ID.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no property with the specified ID is found.</exception>
    /// <remarks>
    /// This method calls GET /v2/properties/{id} and caches the result for 5 minutes.
    /// </remarks>
    public async Task<Property> GetByIdAsync(string id)
    {
        string cacheKey = $"{PropertiesEndPoint}/{id}";

        if (!this.cache.TryGetValue(cacheKey, out Property? results))
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(new Uri(this.freeAgentClient.ApiBaseUrl, $"{PropertiesEndPoint}/{id}")).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            PropertyRoot? root = await response.Content.ReadFromJsonAsync<PropertyRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            results = root?.Property;
            this.SetCache(cacheKey, results);
        }

        return results ?? throw new InvalidOperationException($"Property with ID {id} not found.");
    }

    /// <summary>
    /// Updates an existing property in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the property to update.</param>
    /// <param name="property">The <see cref="Property"/> object containing the updated property details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="Property"/> object with the latest server values.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/properties/{id} to update the property. The cache is
    /// invalidated after a successful update.
    /// </remarks>
    public async Task<Property> UpdateAsync(string id, Property property)
    {
        PropertyRoot root = new() { Property = property };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{PropertiesEndPoint}/{id}"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        PropertyRoot? result = await response.Content.ReadFromJsonAsync<PropertyRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate all property-related caches since the updated property may be included in various cached queries
        this.InvalidateAllPropertiesCaches();

        return result?.Property ?? throw new InvalidOperationException("Failed to deserialize property response.");
    }

    /// <summary>
    /// Deletes a property from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the property to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls DELETE /v2/properties/{id} to delete the property. The cache is
    /// invalidated after successful deletion.
    /// </remarks>
    public async Task DeleteAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.DeleteAsync(new Uri(this.freeAgentClient.ApiBaseUrl, $"{PropertiesEndPoint}/{id}")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        // Invalidate all property-related caches since the deleted property may have been included in various cached queries
        this.InvalidateAllPropertiesCaches();
    }
}
