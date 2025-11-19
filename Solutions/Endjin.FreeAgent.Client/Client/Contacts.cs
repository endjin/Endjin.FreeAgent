// <copyright file="Contacts.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Concurrent;
using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing contacts via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent contacts, which represent customers, suppliers,
/// and other business relationships. Contacts are used to associate invoices, bills, projects, and
/// other transactions with specific organizations or individuals.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance while maintaining reasonable data freshness,
/// as contact information may change during active usage.
/// </para>
/// </remarks>
/// <seealso cref="Contact"/>
public class Contacts
{
    private const string ContactsEndPoint = "v2/contacts";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();
    private readonly ConcurrentDictionary<string, byte> cacheKeys = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Contacts"/> class.
    /// </summary>
    /// <param name="freeAgentClient">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing contact data.</param>
    public Contacts(FreeAgentClient freeAgentClient, IMemoryCache cache)
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
    /// Invalidates all contact-related cache entries.
    /// </summary>
    /// <remarks>
    /// This method removes all cached entries whose keys start with the contacts endpoint path,
    /// ensuring that any cached contact data (individual contacts, lists, filtered views) is cleared
    /// when contacts are created, updated, or deleted.
    /// </remarks>
    private void InvalidateAllContactsCaches()
    {
        foreach (string key in this.cacheKeys.Keys)
        {
            if (key.StartsWith(ContactsEndPoint, StringComparison.Ordinal))
            {
                this.cache.Remove(key);
                this.cacheKeys.TryRemove(key, out _);
            }
        }
    }

    /// <summary>
    /// Creates a new contact in FreeAgent.
    /// </summary>
    /// <param name="contact">The <see cref="Contact"/> object containing the contact details to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="Contact"/> object with server-assigned values (e.g., ID, URL).
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/contacts to create a new contact. The cache is not updated as
    /// only aggregate queries are cached.
    /// </remarks>
    public async Task<Contact> CreateAsync(Contact contact)
    {
        ContactRoot root = new() { Contact = contact };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, ContactsEndPoint),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        ContactRoot? result = await response.Content.ReadFromJsonAsync<ContactRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.Contact ?? throw new InvalidOperationException("Failed to deserialize contact response.");
    }

    /// <summary>
    /// Retrieves all contacts that have active projects associated with them.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="Contact"/> objects that have at least one active project.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method uses the 'active_projects' view (GET /v2/contacts?view=active_projects) which returns
    /// contacts with active projects. Results are cached for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<Contact>> GetAllWithActiveProjectsAsync()
    {
        string cacheKey = $"{ContactsEndPoint}/active/projects";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<Contact>? results))
        {
            results = await this.GetAllAsync("active_projects").ConfigureAwait(false);
            this.SetCache(cacheKey, results);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves all active contacts from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// active <see cref="Contact"/> objects in the FreeAgent account.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/contacts?view=active, handles pagination automatically, and caches the
    /// result for 5 minutes. Only active contacts are included by default. To retrieve all contacts including
    /// hidden ones, use GetAllAsync("all").
    /// </remarks>
    public async Task<IEnumerable<Contact>> GetAllAsync()
    {
        return await this.GetAllAsync("active").ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves contacts from FreeAgent with filtering and sorting options.
    /// </summary>
    /// <param name="view">
    /// The view filter to apply. Valid values are:
    /// "all" (all contacts including hidden), "active" (active contacts only, default),
    /// "clients" (all clients), "suppliers" (active suppliers only),
    /// "active_projects" (contacts with active projects),
    /// "completed_projects" (contacts with completed invoices),
    /// "open_clients" (clients with open invoices), "open_suppliers" (suppliers with open bills),
    /// "hidden" (hidden contacts only).
    /// </param>
    /// <param name="sort">
    /// Optional sort parameter. Valid values are:
    /// "name" (by concatenated name, default), "created_at" (by creation date),
    /// "updated_at" (by modification date).
    /// Prefix with '-' for descending order (e.g., "-updated_at").
    /// </param>
    /// <param name="updatedSince">
    /// Optional filter to retrieve only contacts updated after this timestamp.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="Contact"/> objects matching the specified criteria.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/contacts with the specified query parameters, handles pagination automatically,
    /// and caches the result for 5 minutes using a cache key that includes all parameters.
    /// </remarks>
    public async Task<IEnumerable<Contact>> GetAllAsync(
        string view = "active",
        string? sort = null,
        DateTimeOffset? updatedSince = null)
    {
        var queryParams = new List<string> { $"view={view}" };

        if (!string.IsNullOrEmpty(sort))
        {
            queryParams.Add($"sort={sort}");
        }

        if (updatedSince.HasValue)
        {
            queryParams.Add($"updated_since={Uri.EscapeDataString(updatedSince.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))}");
        }

        string queryString = string.Join("&", queryParams);
        string cacheKey = $"{ContactsEndPoint}?{queryString}";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<Contact>? results))
        {
            List<ContactsRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<ContactsRoot>(
                new Uri(this.freeAgentClient.ApiBaseUrl, $"{ContactsEndPoint}?{queryString}"))
                .ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Contacts)];
            this.SetCache(cacheKey, results);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves a specific contact by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the contact to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="Contact"/> object with the specified ID.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no contact with the specified ID is found.</exception>
    /// <remarks>
    /// This method calls GET /v2/contacts/{id} and caches the result for 5 minutes.
    /// </remarks>
    public async Task<Contact> GetByIdAsync(string id)
    {
        string cacheKey = $"{ContactsEndPoint}/{id}";

        if (!this.cache.TryGetValue(cacheKey, out Contact? results))
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(new Uri(freeAgentClient.ApiBaseUrl, $"{ContactsEndPoint}/{id}")).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            ContactRoot? root = await response.Content.ReadFromJsonAsync<ContactRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            results = root?.Contact;
            this.SetCache(cacheKey, results);
        }

        return results ?? throw new InvalidOperationException($"Contact with ID {id} not found.");
    }

    /// <summary>
    /// Retrieves a specific contact by its organization name from FreeAgent.
    /// </summary>
    /// <param name="organisationName">The organization name of the contact to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="Contact"/> object with the specified organization name.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no contact with the specified organization name is found.</exception>
    /// <remarks>
    /// This method retrieves all contacts (using <c>GetAllAsync</c>) and performs a case-insensitive
    /// search by organization name. Results are cached for 5 minutes once found.
    /// </remarks>
    public async Task<Contact> GetByOrganisationNameAsync(string organisationName)
    {
        string cacheKey = $"{ContactsEndPoint}/{organisationName}";

        if (!this.cache.TryGetValue(cacheKey, out Contact? results))
        {
            IEnumerable<Contact> contacts = await this.GetAllAsync().ConfigureAwait(false);
            results = contacts.FirstOrDefault(x => string.Equals(x.OrganisationName, organisationName, StringComparison.InvariantCultureIgnoreCase));

            if (results != null)
            {
                this.SetCache(cacheKey, results);
            }
        }

        return results ?? throw new InvalidOperationException($"Contact with organisation name '{organisationName}' not found.");
    }

    /// <summary>
    /// Updates an existing contact in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the contact to update.</param>
    /// <param name="contact">The <see cref="Contact"/> object containing the updated contact details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="Contact"/> object with the latest server values.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/contacts/{id} to update the contact. The cache entry for this contact
    /// is invalidated after a successful update.
    /// </remarks>
    public async Task<Contact> UpdateAsync(string id, Contact contact)
    {
        ContactRoot root = new() { Contact = contact };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{ContactsEndPoint}/{id}"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        ContactRoot? result = await response.Content.ReadFromJsonAsync<ContactRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate all contact-related caches since the updated contact may be included in various cached queries
        this.InvalidateAllContactsCaches();

        return result?.Contact ?? throw new InvalidOperationException("Failed to deserialize contact response.");
    }

    /// <summary>
    /// Deletes a contact from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the contact to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls DELETE /v2/contacts/{id} to delete the contact. The cache entry for this contact
    /// is invalidated after successful deletion. Note that contacts with associated projects, invoices, or
    /// bills cannot be deleted and should instead be marked as hidden.
    /// </remarks>
    public async Task DeleteAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.DeleteAsync(new Uri(this.freeAgentClient.ApiBaseUrl, $"{ContactsEndPoint}/{id}")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        // Invalidate all contact-related caches since the deleted contact may have been included in various cached queries
        this.InvalidateAllContactsCaches();
    }
}