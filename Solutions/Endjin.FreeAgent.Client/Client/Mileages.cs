// <copyright file="Mileages.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing mileage claims via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent mileage tracking, which allows users to record
/// business miles traveled and claim mileage expenses. Mileage can be filtered by user and date range
/// for reporting and expense tracking purposes.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance. Cache entries are invalidated automatically
/// when mileage records are created, updated, or deleted.
/// </para>
/// </remarks>
/// <seealso cref="Mileage"/>
/// <seealso cref="User"/>
public class Mileages
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="Mileages"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing mileage data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public Mileages(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Creates a new mileage claim in FreeAgent.
    /// </summary>
    /// <param name="mileage">The <see cref="Mileage"/> object containing the mileage claim details to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="Mileage"/> object with server-assigned values (e.g., ID, URL).
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="mileage"/> is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/mileages to create a new mileage claim. The cache is invalidated
    /// to ensure subsequent queries return up-to-date data.
    /// </remarks>
    public async Task<Mileage> CreateAsync(Mileage mileage)
    {
        ArgumentNullException.ThrowIfNull(mileage);
        await this.client.InitializeAndAuthorizeAsync();

        MileageRoot data = new() { Mileage = mileage };
        using JsonContent content = JsonContent.Create(data, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.client.HttpClient.PostAsync(new Uri(this.client.ApiBaseUrl, "/v2/mileages"), content);
        response.EnsureSuccessStatusCode();

        MileageRoot? root = await response.Content.ReadFromJsonAsync<MileageRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove("mileages_all");

        return root?.Mileage ?? throw new InvalidOperationException("Failed to create mileage");
    }

    /// <summary>
    /// Retrieves mileage claims from FreeAgent with optional filters.
    /// </summary>
    /// <param name="userId">Optional user ID to filter mileage claims by specific user.</param>
    /// <param name="fromDate">Optional start date to filter mileage claims from this date onwards.</param>
    /// <param name="toDate">Optional end date to filter mileage claims up to this date.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="Mileage"/> objects matching the specified filters.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/mileages with optional query parameters (user, from_date, to_date) and caches
    /// the result for 5 minutes. Different filter combinations create separate cache entries.
    /// </remarks>
    public async Task<IEnumerable<Mileage>> GetAllAsync(string? userId = null, DateOnly? fromDate = null, DateOnly? toDate = null)
    {
        await this.client.InitializeAndAuthorizeAsync();

        List<string> queryParams = [];
        if (!string.IsNullOrWhiteSpace(userId))
        {
            queryParams.Add($"user={userId}");
        }

        if (fromDate.HasValue)
        {
            queryParams.Add($"from_date={fromDate.Value:yyyy-MM-dd}");
        }

        if (toDate.HasValue)
        {
            queryParams.Add($"to_date={toDate.Value:yyyy-MM-dd}");
        }

        string url = "/v2/mileages";
        if (queryParams.Count > 0)
        {
            url += "?" + string.Join("&", queryParams);
        }

        string cacheKey = $"mileages_{userId ?? "all"}_{fromDate?.ToString("yyyyMMdd")}_{toDate?.ToString("yyyyMMdd")}";

        if (this.cache.TryGetValue(cacheKey, out IEnumerable<Mileage>? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(
            new Uri(this.client.ApiBaseUrl, url));
        response.EnsureSuccessStatusCode();

        MileagesRoot? root = await response.Content.ReadFromJsonAsync<MileagesRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        IEnumerable<Mileage> mileages = root?.Mileages ?? [];

        this.cache.Set(cacheKey, mileages, TimeSpan.FromMinutes(5));

        return mileages;
    }

    /// <summary>
    /// Retrieves a specific mileage claim by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the mileage claim to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="Mileage"/> object with the specified ID.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no mileage claim with the specified ID is found.</exception>
    /// <remarks>
    /// This method calls GET /v2/mileages/{id} and caches the result for 5 minutes.
    /// </remarks>
    public async Task<Mileage> GetByIdAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        await this.client.InitializeAndAuthorizeAsync();

        string cacheKey = $"mileage_{id}";

        if (this.cache.TryGetValue(cacheKey, out Mileage? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, $"/v2/mileages/{id}"));
        response.EnsureSuccessStatusCode();

        MileageRoot? root = await response.Content.ReadFromJsonAsync<MileageRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        Mileage? mileage = root?.Mileage;

        if (mileage == null)
        {
            throw new InvalidOperationException($"Mileage {id} not found");
        }

        this.cache.Set(cacheKey, mileage, TimeSpan.FromMinutes(5));

        return mileage;
    }

    /// <summary>
    /// Updates an existing mileage claim in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the mileage claim to update.</param>
    /// <param name="mileage">The <see cref="Mileage"/> object containing the updated mileage claim details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="Mileage"/> object as returned by the API.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="mileage"/> is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/mileages/{id} to update the mileage claim. The cache entries for this
    /// mileage claim and all mileage queries are invalidated after a successful update.
    /// </remarks>
    public async Task<Mileage> UpdateAsync(string id, Mileage mileage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(mileage);
        await this.client.InitializeAndAuthorizeAsync();

        MileageRoot data = new() { Mileage = mileage };
        using JsonContent content = JsonContent.Create(data, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.client.HttpClient.PutAsync(new Uri(this.client.ApiBaseUrl, $"/v2/mileages/{id}"), content);
        response.EnsureSuccessStatusCode();

        MileageRoot? root = await response.Content.ReadFromJsonAsync<MileageRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove($"mileage_{id}");
        this.cache.Remove("mileages_all");

        return root?.Mileage ?? throw new InvalidOperationException("Failed to update mileage");
    }

    /// <summary>
    /// Deletes a mileage claim from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the mileage claim to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls DELETE /v2/mileages/{id} to delete the mileage claim. The cache entries for this
    /// mileage claim and all mileage queries are invalidated after successful deletion.
    /// </remarks>
    public async Task DeleteAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.DeleteAsync(new Uri(this.client.ApiBaseUrl, $"/v2/mileages/{id}"));
        response.EnsureSuccessStatusCode();

        this.cache.Remove($"mileage_{id}");
        this.cache.Remove("mileages_all");
    }
}