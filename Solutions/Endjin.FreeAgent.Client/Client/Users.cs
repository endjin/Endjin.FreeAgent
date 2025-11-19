// <copyright file="Users.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Concurrent;
using System.Net.Http.Json;

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
    private readonly ConcurrentDictionary<string, byte> cacheKeys = new();

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
    /// Invalidates all user-related cache entries.
    /// </summary>
    /// <remarks>
    /// This method removes all cached entries whose keys start with the users endpoint path,
    /// ensuring that any cached user data (individual users, lists, filtered views) is cleared
    /// when users are created, updated, or deleted.
    /// </remarks>
    private void InvalidateAllUsersCaches()
    {
        foreach (string key in this.cacheKeys.Keys)
        {
            if (key.StartsWith(UsersEndPoint, StringComparison.Ordinal))
            {
                this.cache.Remove(key);
                this.cacheKeys.TryRemove(key, out _);
            }
        }
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
    /// <para>
    /// This method calls GET /v2/users, handles pagination automatically, filters for active employees,
    /// and caches the result for 5 minutes. Only non-hidden users with the Employee role are included.
    /// </para>
    /// <para>
    /// Minimum Access Level: Tax, Accounting &amp; Users (level 7).
    /// </para>
    /// </remarks>
    public async Task<IList<User>> GetAllActiveEmployeesAsync()
    {
        string cacheKey = $"{UsersEndPoint}/employees/active";

        if (!this.cache.TryGetValue(cacheKey, out IList<User>? results))
        {
            List<UsersRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<UsersRoot>(new Uri(this.freeAgentClient.ApiBaseUrl, UsersEndPoint)).ConfigureAwait(false);
            IEnumerable<User> users = response.SelectMany(x => x.Users);

            results = [.. users.Where(x => x is { Hidden: false, Role: Role.Employee }).OrderBy(x => x.LastName)];
            this.SetCache(cacheKey, results);
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
    /// <para>
    /// This method calls GET /v2/users, handles pagination automatically, filters for active directors,
    /// and caches the result for 5 minutes. Only non-hidden users with the Director role are included.
    /// </para>
    /// <para>
    /// Minimum Access Level: Tax, Accounting &amp; Users (level 7).
    /// </para>
    /// </remarks>
    public async Task<IEnumerable<User>> GetAllDirectorsAsync()
    {
        string cacheKey = $"{UsersEndPoint}/directors/active";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<User>? results))
        {
            List<UsersRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<UsersRoot>(new Uri(this.freeAgentClient.ApiBaseUrl, UsersEndPoint)).ConfigureAwait(false);
            IEnumerable<User> users = response.SelectMany(x => x.Users);

            results = [.. users.Where(x => x is { Hidden: false, Role: Role.Director })];
            this.SetCache(cacheKey, results);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves all users from FreeAgent.
    /// </summary>
    /// <param name="view">
    /// Optional view filter to apply. Valid values are:
    /// "all" (all users, default), "staff" (all staff), "active_staff" (active staff only),
    /// "advisors" (all advisors), "active_advisors" (active advisors only).
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="User"/> objects in the FreeAgent account matching the specified view.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// <para>
    /// This method calls GET /v2/users with an optional view query parameter, handles pagination automatically,
    /// and caches the result for 5 minutes. If no view is specified, all users are returned.
    /// </para>
    /// <para>
    /// Minimum Access Level: Tax, Accounting &amp; Users (level 7).
    /// </para>
    /// </remarks>
    public async Task<IEnumerable<User>> GetAllUsersAsync(string? view = null)
    {
        string queryString = string.IsNullOrEmpty(view) ? string.Empty : $"?view={view}";
        string cacheKey = $"{UsersEndPoint}{queryString}";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<User>? results))
        {
            List<UsersRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<UsersRoot>(
                new Uri(this.freeAgentClient.ApiBaseUrl, $"{UsersEndPoint}{queryString}")).ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Users)];
            this.SetCache(cacheKey, results);
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
    /// <para>
    /// This method calls GET /v2/users/{userId} and caches the result for 5 minutes.
    /// </para>
    /// <para>
    /// Minimum Access Level: Tax, Accounting &amp; Users (level 7).
    /// </para>
    /// </remarks>
    public async Task<User> GetByIdAsync(string userId)
    {
        string cacheKey = $"{UsersEndPoint}/{userId}";

        if (!this.cache.TryGetValue(cacheKey, out User? results))
        {
            List<UserRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<UserRoot>(new Uri(this.freeAgentClient.ApiBaseUrl, $"{UsersEndPoint}/{userId}")).ConfigureAwait(false);

            results = response.First().User;
            this.SetCache(cacheKey, results);
        }

        return results ?? throw new InvalidOperationException($"User with ID {userId} not found.");
    }

    /// <summary>
    /// Retrieves the currently authenticated user's profile from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="User"/> object for the authenticated user.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// <para>
    /// This method calls GET /v2/users/me and caches the result for 5 minutes.
    /// </para>
    /// <para>
    /// Minimum Access Level: Time (level 1).
    /// </para>
    /// </remarks>
    public async Task<User> GetCurrentUserAsync()
    {
        string cacheKey = $"{UsersEndPoint}/me";

        if (!this.cache.TryGetValue(cacheKey, out User? results))
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(
                new Uri(this.freeAgentClient.ApiBaseUrl, $"{UsersEndPoint}/me")).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            UserRoot? root = await response.Content.ReadFromJsonAsync<UserRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            results = root?.User;
            this.SetCache(cacheKey, results);
        }

        return results ?? throw new InvalidOperationException("Failed to retrieve current user.");
    }

    /// <summary>
    /// Creates a new user in FreeAgent.
    /// </summary>
    /// <param name="user">The <see cref="User"/> object containing the user details to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="User"/> object with server-assigned values (e.g., URL, timestamps).
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// <para>
    /// This method calls POST /v2/users to create a new user. If <see cref="User.SendInvitation"/> is set to true,
    /// the new user will receive an email with instructions to set up their password.
    /// </para>
    /// <para>
    /// Minimum Access Level: Tax, Accounting &amp; Users (level 7).
    /// </para>
    /// </remarks>
    public async Task<User> CreateAsync(User user)
    {
        UserRoot root = new() { User = user };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, UsersEndPoint),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        UserRoot? result = await response.Content.ReadFromJsonAsync<UserRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate all user-related caches since the new user may appear in various cached queries
        this.InvalidateAllUsersCaches();

        return result?.User ?? throw new InvalidOperationException("Failed to deserialize user response.");
    }

    /// <summary>
    /// Updates an existing user in FreeAgent.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to update.</param>
    /// <param name="user">The <see cref="User"/> object containing the updated user details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="User"/> object with the latest server values.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// <para>
    /// This method calls PUT /v2/users/{userId} to update the user. The cache is invalidated after
    /// a successful update.
    /// </para>
    /// <para>
    /// Minimum Access Level: Tax, Accounting &amp; Users (level 7).
    /// </para>
    /// </remarks>
    public async Task<User> UpdateAsync(string userId, User user)
    {
        UserRoot root = new() { User = user };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{UsersEndPoint}/{userId}"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        UserRoot? result = await response.Content.ReadFromJsonAsync<UserRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate all user-related caches since the updated user may be included in various cached queries
        this.InvalidateAllUsersCaches();

        return result?.User ?? throw new InvalidOperationException("Failed to deserialize user response.");
    }

    /// <summary>
    /// Updates the currently authenticated user's profile in FreeAgent.
    /// </summary>
    /// <param name="user">The <see cref="User"/> object containing the updated profile details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="User"/> object with the latest server values.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// <para>
    /// This method calls PUT /v2/users/me to update the current user's own profile. The cache is
    /// invalidated after a successful update.
    /// </para>
    /// <para>
    /// Minimum Access Level: Time (level 1).
    /// </para>
    /// </remarks>
    public async Task<User> UpdateCurrentUserAsync(User user)
    {
        UserRoot root = new() { User = user };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{UsersEndPoint}/me"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        UserRoot? result = await response.Content.ReadFromJsonAsync<UserRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate all user-related caches since the updated user may be included in various cached queries
        this.InvalidateAllUsersCaches();

        return result?.User ?? throw new InvalidOperationException("Failed to deserialize user response.");
    }

    /// <summary>
    /// Deletes a user from FreeAgent.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// <para>
    /// This method calls DELETE /v2/users/{userId} to delete the user. The cache is invalidated
    /// after successful deletion.
    /// </para>
    /// <para>
    /// Minimum Access Level: Tax, Accounting &amp; Users (level 7).
    /// </para>
    /// </remarks>
    public async Task DeleteAsync(string userId)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.DeleteAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{UsersEndPoint}/{userId}")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        // Invalidate all user-related caches since the deleted user may have been included in various cached queries
        this.InvalidateAllUsersCaches();
    }
}