// <copyright file="CapitalAssetTypes.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing capital asset types via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides full CRUD access to FreeAgent capital asset types, which define
/// the categories of fixed assets. FreeAgent provides system default types (Computer Equipment,
/// Fixtures and Fittings, Motor Vehicles, Other Capital Asset), and users can create custom types.
/// </para>
/// <para>
/// System default types cannot be updated or deleted. User-created types can only be updated or
/// deleted if they do not contain any capital asset items.
/// </para>
/// <para>
/// Results from GetAllAsync are cached for 24 hours as capital asset types rarely change.
/// </para>
/// </remarks>
/// <seealso cref="CapitalAssetType"/>
/// <seealso cref="CapitalAsset"/>
public class CapitalAssetTypes
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="CapitalAssetTypes"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing capital asset type data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public CapitalAssetTypes(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Retrieves all capital asset types from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// all <see cref="CapitalAssetType"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/capital_asset_types and caches the result for 24 hours, as capital
    /// asset types rarely change during typical usage.
    /// </remarks>
    public async Task<IEnumerable<CapitalAssetType>> GetAllAsync()
    {
        await this.client.InitializeAndAuthorizeAsync();

        string cacheKey = "capital_asset_types_all";

        if (this.cache.TryGetValue(cacheKey, out IEnumerable<CapitalAssetType>? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, "/v2/capital_asset_types"));
        response.EnsureSuccessStatusCode();

        CapitalAssetTypesRoot? root = await response.Content.ReadFromJsonAsync<CapitalAssetTypesRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        IEnumerable<CapitalAssetType> types = root?.CapitalAssetTypes ?? [];

        this.cache.Set(cacheKey, types, TimeSpan.FromHours(24));

        return types;
    }

    /// <summary>
    /// Retrieves a specific capital asset type by its ID.
    /// </summary>
    /// <param name="id">The ID of the capital asset type to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="CapitalAssetType"/> object.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/capital_asset_types/:id.
    /// </remarks>
    public async Task<CapitalAssetType> GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Capital asset type ID cannot be null or whitespace.", nameof(id));
        }

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(
            new Uri(this.client.ApiBaseUrl, $"/v2/capital_asset_types/{id}"));
        response.EnsureSuccessStatusCode();

        CapitalAssetTypeRoot? root = await response.Content.ReadFromJsonAsync<CapitalAssetTypeRoot>(
            SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return root?.CapitalAssetType ?? throw new InvalidOperationException("Response did not contain a capital asset type.");
    }

    /// <summary>
    /// Creates a new capital asset type.
    /// </summary>
    /// <param name="name">The name for the new capital asset type.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the created
    /// <see cref="CapitalAssetType"/> object.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls POST /v2/capital_asset_types. The created type will be user-created
    /// (system_default = false) and can be updated or deleted if it doesn't contain any capital assets.
    /// </remarks>
    public async Task<CapitalAssetType> CreateAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Capital asset type name cannot be null or whitespace.", nameof(name));
        }

        await this.client.InitializeAndAuthorizeAsync();

        CapitalAssetTypeRoot payload = new()
        {
            CapitalAssetType = new CapitalAssetType { Name = name },
        };

        HttpResponseMessage response = await this.client.HttpClient.PostAsJsonAsync(
            new Uri(this.client.ApiBaseUrl, "/v2/capital_asset_types"),
            payload,
            SharedJsonOptions.SourceGenOptions);
        response.EnsureSuccessStatusCode();

        CapitalAssetTypeRoot? root = await response.Content.ReadFromJsonAsync<CapitalAssetTypeRoot>(
            SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache since we've added a new type
        this.cache.Remove("capital_asset_types_all");

        return root?.CapitalAssetType ?? throw new InvalidOperationException("Response did not contain a capital asset type.");
    }

    /// <summary>
    /// Updates an existing capital asset type.
    /// </summary>
    /// <param name="id">The ID of the capital asset type to update.</param>
    /// <param name="name">The new name for the capital asset type.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the updated
    /// <see cref="CapitalAssetType"/> object.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> or <paramref name="name"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls PUT /v2/capital_asset_types/:id. Only user-created capital asset types
    /// (system_default = false) that do not contain any capital asset items can be updated.
    /// Attempting to update a system default type or a type with assets will result in an error.
    /// </remarks>
    public async Task<CapitalAssetType> UpdateAsync(string id, string name)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Capital asset type ID cannot be null or whitespace.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Capital asset type name cannot be null or whitespace.", nameof(name));
        }

        await this.client.InitializeAndAuthorizeAsync();

        CapitalAssetTypeRoot payload = new()
        {
            CapitalAssetType = new CapitalAssetType { Name = name },
        };

        HttpResponseMessage response = await this.client.HttpClient.PutAsJsonAsync(
            new Uri(this.client.ApiBaseUrl, $"/v2/capital_asset_types/{id}"),
            payload,
            SharedJsonOptions.SourceGenOptions);
        response.EnsureSuccessStatusCode();

        CapitalAssetTypeRoot? root = await response.Content.ReadFromJsonAsync<CapitalAssetTypeRoot>(
            SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache since we've updated a type
        this.cache.Remove("capital_asset_types_all");

        return root?.CapitalAssetType ?? throw new InvalidOperationException("Response did not contain a capital asset type.");
    }

    /// <summary>
    /// Deletes a capital asset type.
    /// </summary>
    /// <param name="id">The ID of the capital asset type to delete.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls DELETE /v2/capital_asset_types/:id. Only user-created capital asset types
    /// (system_default = false) that do not contain any capital asset items can be deleted.
    /// Attempting to delete a system default type or a type with assets will result in an error.
    /// </remarks>
    public async Task DeleteAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Capital asset type ID cannot be null or whitespace.", nameof(id));
        }

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.DeleteAsync(
            new Uri(this.client.ApiBaseUrl, $"/v2/capital_asset_types/{id}"));
        response.EnsureSuccessStatusCode();

        // Invalidate cache since we've deleted a type
        this.cache.Remove("capital_asset_types_all");
    }
}