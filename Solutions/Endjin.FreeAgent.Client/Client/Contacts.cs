// <copyright file="Contacts.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

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
    /// This method retrieves all contacts (using <see cref="GetAllAsync"/>) and filters for those with
    /// ActiveProjectsCount greater than zero. Results are cached for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<Contact>> GetAllWithActiveProjectsAsync()
    {
        string cacheKey = $"{ContactsEndPoint}/active/projects";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<Contact>? results))
        {
            IEnumerable<Contact> response = await this.GetAllAsync().ConfigureAwait(false);

            results = [.. response.Where(x => x.ActiveProjectsCount > 0)];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves all contacts from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// all <see cref="Contact"/> objects in the FreeAgent account.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/contacts?filter=all, handles pagination automatically, and caches the
    /// result for 5 minutes. All contacts are included regardless of status.
    /// </remarks>
    public async Task<IEnumerable<Contact>> GetAllAsync()
    {
        string cacheKey = ContactsEndPoint;

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<Contact>? results))
        {
            List<ContactsRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<ContactsRoot>(new Uri(this.freeAgentClient.ApiBaseUrl, ContactsEndPoint + "?filter=all"))
                .ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Contacts)];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
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
            this.cache.Set(cacheKey, results, cacheEntryOptions);
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
    /// This method retrieves all contacts (using <see cref="GetAllAsync"/>) and performs a case-insensitive
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
                this.cache.Set(cacheKey, results, cacheEntryOptions);
            }
        }

        return results ?? throw new InvalidOperationException($"Contact with organisation name '{organisationName}' not found.");
    }
}