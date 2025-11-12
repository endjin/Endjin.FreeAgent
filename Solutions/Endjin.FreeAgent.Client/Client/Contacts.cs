// <copyright file="Contacts.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class Contacts
{
    private const string ContactsEndPoint = "v2/contacts";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    public Contacts(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

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