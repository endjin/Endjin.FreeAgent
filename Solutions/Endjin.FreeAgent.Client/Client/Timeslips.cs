// <copyright file="Timeslips.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class Timeslips
{
    private const string TimeslipsEndPoint = "v2/timeslips";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    public Timeslips(FreeAgentClient client, IMemoryCache cache)
    {
        this.freeAgentClient = client;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    //GET https://api.freeagent.com/v2/timeslips?project=https://api.freeagent.com/v2/projects/2

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
