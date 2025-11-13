// <copyright file="Users.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for accessing user information via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent users, including employees, directors, and other
/// roles. Users represent individuals who have access to the FreeAgent account and can be assigned
/// various permissions and roles.
/// </para>
/// <para>
/// Results are cached for 5 minutes to balance performance with data freshness, as user information
/// may change during active usage (e.g., adding new employees, updating roles).
/// </para>
/// </remarks>
/// <seealso cref="User"/>
/// <seealso cref="Role"/>
public class Users
{
    private const string UsersEndPoint = "v2/users";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Users"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing user data.</param>
    public Users(FreeAgentClient client, IMemoryCache cache)
    {
        this.freeAgentClient = client;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    /// <summary>
    /// Retrieves all active employees from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a list of
    /// <see cref="User"/> objects with role <see cref="Role.Employee"/> and Hidden set to false,
    /// ordered by last name.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/users, handles pagination automatically, filters for active employees,
    /// and caches the result for 5 minutes. Only non-hidden users with the Employee role are included.
    /// </remarks>
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

    /// <summary>
    /// Retrieves all active directors from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="User"/> objects with role <see cref="Role.Director"/> and Hidden set to false.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/users, handles pagination automatically, filters for active directors,
    /// and caches the result for 5 minutes. Only non-hidden users with the Director role are included.
    /// </remarks>
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

    /// <summary>
    /// Retrieves all users from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// all <see cref="User"/> objects in the FreeAgent account.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/users, handles pagination automatically, and caches the result for 5 minutes.
    /// All users are included regardless of role or hidden status.
    /// </remarks>
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

    /// <summary>
    /// Retrieves a specific user by their ID from FreeAgent.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="User"/> object with the specified ID.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no user with the specified ID is found.</exception>
    /// <remarks>
    /// This method calls GET /v2/users/{userId} and caches the result for 5 minutes.
    /// </remarks>
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