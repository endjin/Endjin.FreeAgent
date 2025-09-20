// <copyright file="Users.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class Users
{
    private const string UsersEndPoint = "v2/users";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    public Users(FreeAgentClient client, IMemoryCache cache)
    {
        this.freeAgentClient = client;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    public async Task<IList<User>> GetAllActiveEmployeesAsync()
    {
        string cacheKey = $"{UsersEndPoint}/employees/active";

        if (!this.cache.TryGetValue(cacheKey, out IList<User>? results))
        {
            List<UsersRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<UsersRoot>(new Uri(this.freeAgentClient.ApiBaseUrl, UsersEndPoint)).ConfigureAwait(false);
            IEnumerable<User> users = response.SelectMany(x => x.Users);

            results = [.. users.Where(x => x is { Hidden: false, Role: Role.Employee }).OrderBy(x => x.LastName)];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    public async Task<IEnumerable<User>> GetAllDirectorsAsync()
    {
        string cacheKey = $"{UsersEndPoint}/directors/active";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<User>? results))
        {
            List<UsersRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<UsersRoot>(new Uri(this.freeAgentClient.ApiBaseUrl, UsersEndPoint)).ConfigureAwait(false);
            IEnumerable<User> users = response.SelectMany(x => x.Users);

            results = [.. users.Where(x => x is { Hidden: false, Role: Role.Director })];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        string cacheKey = UsersEndPoint;

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<User>? results))
        {
            List<UsersRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<UsersRoot>(new Uri(this.freeAgentClient.ApiBaseUrl, UsersEndPoint)).ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Users)];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    public async Task<User> GetByIdAsync(string userId)
    {
        string cacheKey = $"{UsersEndPoint}/{userId}";

        if (!this.cache.TryGetValue(cacheKey, out User? results))
        {
            List<UserRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<UserRoot>(new Uri(this.freeAgentClient.ApiBaseUrl, $"{UsersEndPoint}/{userId}")).ConfigureAwait(false);

            results = response.First().User;
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? throw new InvalidOperationException($"User with ID {userId} not found.");
    }
}
