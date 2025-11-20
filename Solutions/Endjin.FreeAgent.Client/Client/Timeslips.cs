// <copyright file="Timeslips.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing timeslips via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent timeslips, which represent time entries logged against
/// projects and tasks. Timeslips are used for time tracking, billing, and project management.
/// </para>
/// <para>
/// Timeslips support timer functionality for real-time tracking, where users can start and stop timers
/// that automatically calculate elapsed time.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance when retrieving timeslips for projects or users.
/// </para>
/// </remarks>
/// <seealso cref="Timeslip"/>
/// <seealso cref="Project"/>
/// <seealso cref="TaskItem"/>
/// <seealso cref="User"/>
public class Timeslips
{
    private const string TimeslipsEndPoint = "v2/timeslips";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Timeslips"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing timeslip data.</param>
    public Timeslips(FreeAgentClient client, IMemoryCache cache)
    {
        this.freeAgentClient = client;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    /// <summary>
    /// Creates a new timeslip in FreeAgent.
    /// </summary>
    /// <param name="timeslip">The <see cref="Timeslip"/> object containing the timeslip details to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="Timeslip"/> object with server-assigned values (e.g., URL, created_at).
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// <para>
    /// This method calls POST /v2/timeslips to create a new timeslip. The timeslip requires
    /// task, user, project, and dated_on properties to be set.
    /// </para>
    /// <para>
    /// Minimum Access Level: Time
    /// </para>
    /// </remarks>
    public async Task<Timeslip> CreateAsync(Timeslip timeslip)
    {
        TimeslipRoot root = new() { Timeslip = timeslip };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, TimeslipsEndPoint), content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        TimeslipRoot? result = await response.Content.ReadFromJsonAsync<TimeslipRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.Timeslip ?? throw new InvalidOperationException("Failed to deserialize timeslip response.");
    }

    /// <summary>
    /// Creates multiple timeslips in FreeAgent in a single batch operation.
    /// </summary>
    /// <param name="timeslips">The collection of <see cref="Timeslip"/> objects to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// created <see cref="Timeslip"/> objects with server-assigned values.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// <para>
    /// This method calls POST /v2/timeslips with an array of timeslips for batch creation.
    /// Each timeslip requires task, user, project, and dated_on properties to be set.
    /// </para>
    /// <para>
    /// Minimum Access Level: Time
    /// </para>
    /// </remarks>
    public async Task<IEnumerable<Timeslip>> CreateBatchAsync(IEnumerable<Timeslip> timeslips)
    {
        TimeslipsRoot root = new() { Timeslips = [.. timeslips] };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, TimeslipsEndPoint), content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        TimeslipsRoot? result = await response.Content.ReadFromJsonAsync<TimeslipsRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.Timeslips ?? throw new InvalidOperationException("Failed to deserialize timeslips response.");
    }

    /// <summary>
    /// Retrieves a specific timeslip by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the timeslip to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="Timeslip"/> object with the specified ID.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no timeslip with the specified ID is found.</exception>
    /// <remarks>
    /// <para>
    /// This method calls GET /v2/timeslips/{id} and caches the result for 5 minutes.
    /// </para>
    /// <para>
    /// Minimum Access Level: Time
    /// </para>
    /// </remarks>
    public async Task<Timeslip> GetByIdAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        string cacheKey = $"{TimeslipsEndPoint}/{id}";

        if (this.cache.TryGetValue(cacheKey, out Timeslip? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{TimeslipsEndPoint}/{id}")).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        TimeslipRoot? root = await response.Content.ReadFromJsonAsync<TimeslipRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        Timeslip? item = root?.Timeslip;

        if (item == null)
        {
            throw new InvalidOperationException($"Timeslip {id} not found");
        }

        this.cache.Set(cacheKey, item, this.cacheEntryOptions);

        return item;
    }

    /// <summary>
    /// Retrieves all timeslips from FreeAgent with optional filtering.
    /// </summary>
    /// <param name="fromDate">Optional start date filter (format: yyyy-MM-dd).</param>
    /// <param name="toDate">Optional end date filter (format: yyyy-MM-dd).</param>
    /// <param name="updatedSince">Optional filter to retrieve only timeslips updated after this timestamp.</param>
    /// <param name="view">
    /// Optional view filter. Valid values are:
    /// "all" (default - all timeslips), "unbilled" (unbilled timeslips only), "running" (timeslips with running timers).
    /// </param>
    /// <param name="nested">Optional flag to return associated resources as nested objects instead of URL references.</param>
    /// <param name="user">Optional filter to retrieve only timeslips for a specific user.</param>
    /// <param name="task">Optional filter to retrieve only timeslips for a specific task.</param>
    /// <param name="project">Optional filter to retrieve only timeslips for a specific project.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="Timeslip"/> objects matching the specified criteria.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// <para>
    /// This method calls GET /v2/timeslips with the specified query parameters, handles pagination automatically,
    /// and caches the result for 5 minutes using a cache key that includes all parameters.
    /// </para>
    /// <para>
    /// Minimum Access Level: Time
    /// </para>
    /// </remarks>
    public async Task<IEnumerable<Timeslip>> GetAllAsync(
        string? fromDate = null,
        string? toDate = null,
        DateTimeOffset? updatedSince = null,
        string? view = null,
        bool? nested = null,
        Uri? user = null,
        Uri? task = null,
        Uri? project = null)
    {
        List<string> queryParams = [];

        if (!string.IsNullOrEmpty(fromDate))
        {
            queryParams.Add($"from_date={fromDate}");
        }

        if (!string.IsNullOrEmpty(toDate))
        {
            queryParams.Add($"to_date={toDate}");
        }

        if (updatedSince.HasValue)
        {
            queryParams.Add($"updated_since={updatedSince.Value:O}");
        }

        if (!string.IsNullOrEmpty(view))
        {
            queryParams.Add($"view={view}");
        }

        if (nested.HasValue)
        {
            queryParams.Add($"nested={nested.Value.ToString().ToLowerInvariant()}");
        }

        if (user != null)
        {
            queryParams.Add($"user={Uri.EscapeDataString(user.ToString())}");
        }

        if (task != null)
        {
            queryParams.Add($"task={Uri.EscapeDataString(task.ToString())}");
        }

        if (project != null)
        {
            queryParams.Add($"project={Uri.EscapeDataString(project.ToString())}");
        }

        string queryString = queryParams.Count > 0 ? $"?{string.Join("&", queryParams)}" : string.Empty;
        string cacheKey = $"{TimeslipsEndPoint}{queryString}";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<Timeslip>? results))
        {
            List<TimeslipsRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<TimeslipsRoot>(
                new Uri(this.freeAgentClient.ApiBaseUrl, $"{TimeslipsEndPoint}{queryString}"))
                .ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Timeslips)];
            this.cache.Set(cacheKey, results, this.cacheEntryOptions);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves all timeslips for a specific project from FreeAgent.
    /// </summary>
    /// <param name="projectUrl">The URL of the project to retrieve timeslips for.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="Timeslip"/> objects associated with the specified project.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// <para>
    /// This method calls GET /v2/timeslips?project={projectUrl}, handles pagination automatically, and caches
    /// the result for 5 minutes. All timeslips for the project are included regardless of status.
    /// </para>
    /// <para>
    /// Minimum Access Level: Time
    /// </para>
    /// </remarks>
    public async Task<IEnumerable<Timeslip>> GetByProjectUrlAsync(string projectUrl)
    {
        return await this.GetAllAsync(project: new Uri(projectUrl)).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves all timeslips for a specific user within a date range from FreeAgent.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve timeslips for.</param>
    /// <param name="fromDate">The start date of the range (format: yyyy-MM-dd).</param>
    /// <param name="toDate">The end date of the range (format: yyyy-MM-dd).</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="Timeslip"/> objects logged by the specified user within the date range.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// <para>
    /// This method calls GET /v2/timeslips?from_date={fromDate}&amp;to_date={toDate}&amp;view=all&amp;user={userUri},
    /// handles pagination automatically, and caches the result for 5 minutes. All timeslips within the date range
    /// are included regardless of status.
    /// </para>
    /// <para>
    /// Minimum Access Level: Time
    /// </para>
    /// </remarks>
    public async Task<IEnumerable<Timeslip>> GetAllByUserIdAndDateRangeAsync(string userId, string fromDate, string toDate)
    {
        string urlSegment = $"{TimeslipsEndPoint}?from_date={fromDate}&to_date={toDate}&view=all&user=https://api.freeagent.com/v2/users/{userId}";
        string cacheKey = urlSegment;

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<Timeslip>? results))
        {
            List<TimeslipsRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<TimeslipsRoot>(new Uri(this.freeAgentClient.ApiBaseUrl, urlSegment)).ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Timeslips)];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    /// <summary>
    /// Updates an existing timeslip in FreeAgent.
    /// </summary>
    /// <param name="timeslip">The <see cref="Timeslip"/> object containing the updated timeslip details.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when the Timeslip.Url property is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// <para>
    /// This method calls PUT /v2/timeslips/{timeslipId} to update the timeslip. The timeslip ID is extracted from the
    /// Timeslip.Url property. Only the fields that need to be updated should be set on the timeslip object.
    /// </para>
    /// <para>
    /// Minimum Access Level: Time
    /// </para>
    /// </remarks>
    public async Task UpdateAsync(Timeslip timeslip)
    {
        string timeslipId = timeslip.Url?.Segments.Last() ?? throw new ArgumentException("Timeslip.Url cannot be null.", nameof(timeslip));
        TimeslipRoot root = new() { Timeslip = timeslip };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{TimeslipsEndPoint}/{timeslipId}"), content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Deletes a timeslip from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the timeslip to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// <para>
    /// This method calls DELETE /v2/timeslips/{id} to delete a timeslip.
    /// </para>
    /// <para>
    /// Minimum Access Level: Time
    /// </para>
    /// </remarks>
    public async Task DeleteAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.DeleteAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{TimeslipsEndPoint}/{id}")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Starts a timer on a timeslip for real-time time tracking.
    /// </summary>
    /// <param name="id">The unique identifier of the timeslip to start the timer on.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="Timeslip"/> object with the timer information.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// <para>
    /// This method calls POST /v2/timeslips/{id}/timer to start a timer on the timeslip.
    /// The returned timeslip will contain a Timer property with the timer state.
    /// </para>
    /// <para>
    /// Minimum Access Level: Time
    /// </para>
    /// </remarks>
    public async Task<Timeslip> StartTimerAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{TimeslipsEndPoint}/{id}/timer"), null).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        TimeslipRoot? result = await response.Content.ReadFromJsonAsync<TimeslipRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.Timeslip ?? throw new InvalidOperationException("Failed to deserialize timeslip response.");
    }

    /// <summary>
    /// Stops a running timer on a timeslip.
    /// </summary>
    /// <param name="id">The unique identifier of the timeslip to stop the timer on.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="Timeslip"/> object with the accumulated hours.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// <para>
    /// This method calls DELETE /v2/timeslips/{id}/timer to stop the timer on the timeslip.
    /// The elapsed time is added to the timeslip's hours property.
    /// </para>
    /// <para>
    /// Minimum Access Level: Time
    /// </para>
    /// </remarks>
    public async Task<Timeslip> StopTimerAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.DeleteAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{TimeslipsEndPoint}/{id}/timer")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        TimeslipRoot? result = await response.Content.ReadFromJsonAsync<TimeslipRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.Timeslip ?? throw new InvalidOperationException("Failed to deserialize timeslip response.");
    }
}
