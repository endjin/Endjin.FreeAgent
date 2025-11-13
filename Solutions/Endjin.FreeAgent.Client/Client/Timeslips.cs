// <copyright file="Timeslips.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for accessing timeslips via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent timeslips, which represent time entries logged against
/// projects and tasks. Timeslips are used for time tracking, billing, and project management.
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
    /// Retrieves all timeslips for a specific project from FreeAgent.
    /// </summary>
    /// <param name="projectUrl">The URL of the project to retrieve timeslips for.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="Timeslip"/> objects associated with the specified project.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/timeslips?project={projectUrl}, handles pagination automatically, and caches
    /// the result for 5 minutes. All timeslips for the project are included regardless of status.
    /// </remarks>
    public async Task<IEnumerable<Timeslip>> GetByProjectUrlAsync(string projectUrl)
    {
        string urlSegment = $"{TimeslipsEndPoint}?project={projectUrl}";
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
    /// This method calls GET /v2/timeslips?from_date={fromDate}&amp;to_date={toDate}&amp;view=all&amp;user={userUri},
    /// handles pagination automatically, and caches the result for 5 minutes. All timeslips within the date range
    /// are included regardless of status.
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
}