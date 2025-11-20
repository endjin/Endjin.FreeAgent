// <copyright file="Notes.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Concurrent;
using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing notes via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent notes, which are text-based annotations that can be
/// attached to contacts or projects. Notes help track conversations, decisions, and important information
/// related to project work or contact relationships.
/// </para>
/// <para>
/// Results are cached without expiration to improve performance. Cache entries are stored per parent URL.
/// </para>
/// <para>
/// Minimum Access Level: Contacts and Projects
/// </para>
/// </remarks>
/// <seealso cref="NoteItem"/>
/// <seealso cref="Project"/>
/// <seealso cref="Contact"/>
public class Notes
{
    private const string NotesEndPoint = "v2/notes";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();
    private readonly ConcurrentDictionary<string, byte> cacheKeys = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Notes"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing note data.</param>
    public Notes(FreeAgentClient client, IMemoryCache cache)
    {
        this.freeAgentClient = client;
        this.cache = cache;
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
    /// Invalidates all note-related cache entries.
    /// </summary>
    /// <remarks>
    /// This method removes all cached entries whose keys start with the notes endpoint path,
    /// ensuring that any cached note data is cleared when notes are created, updated, or deleted.
    /// </remarks>
    private void InvalidateAllNotesCaches()
    {
        foreach (string key in this.cacheKeys.Keys)
        {
            if (key.StartsWith(NotesEndPoint, StringComparison.Ordinal))
            {
                this.cache.Remove(key);
                this.cacheKeys.TryRemove(key, out _);
            }
        }
    }

    /// <summary>
    /// Retrieves all notes associated with a specific project from FreeAgent.
    /// </summary>
    /// <param name="projectUrl">The URL of the project to retrieve notes for.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="NoteItem"/> objects for the specified project.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/notes?project={projectUrl}, handles pagination automatically, and caches the
    /// result indefinitely. The cache is keyed by the full URL including the project parameter.
    /// </remarks>
    public async Task<IEnumerable<NoteItem>> GetAllByProjectUrlAsync(Uri projectUrl)
    {
        string urlSegment = $"{NotesEndPoint}?project={projectUrl}";
        string cacheKey = urlSegment;

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<NoteItem>? results))
        {
            List<NotesRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<NotesRoot>(new Uri(this.freeAgentClient.ApiBaseUrl, urlSegment)).ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Notes ?? Enumerable.Empty<NoteItem>())];
            this.SetCache(cacheKey, results);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves all notes associated with a specific contact from FreeAgent.
    /// </summary>
    /// <param name="contactUrl">The URL of the contact to retrieve notes for.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="NoteItem"/> objects for the specified contact.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/notes?contact={contactUrl}, handles pagination automatically, and caches the
    /// result indefinitely. The cache is keyed by the full URL including the contact parameter.
    /// </remarks>
    public async Task<IEnumerable<NoteItem>> GetAllByContactUrlAsync(Uri contactUrl)
    {
        string urlSegment = $"{NotesEndPoint}?contact={contactUrl}";
        string cacheKey = urlSegment;

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<NoteItem>? results))
        {
            List<NotesRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<NotesRoot>(new Uri(this.freeAgentClient.ApiBaseUrl, urlSegment)).ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Notes ?? Enumerable.Empty<NoteItem>())];
            this.SetCache(cacheKey, results);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves a specific note by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the note to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="NoteItem"/> object with the specified ID.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no note with the specified ID is found.</exception>
    /// <remarks>
    /// This method calls GET /v2/notes/{id} and caches the result indefinitely.
    /// </remarks>
    public async Task<NoteItem> GetByIdAsync(string id)
    {
        string cacheKey = $"{NotesEndPoint}/{id}";

        if (!this.cache.TryGetValue(cacheKey, out NoteItem? result))
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(new Uri(this.freeAgentClient.ApiBaseUrl, $"{NotesEndPoint}/{id}")).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            NoteRoot? root = await response.Content.ReadFromJsonAsync<NoteRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            result = root?.Note;
            this.SetCache(cacheKey, result);
        }

        return result ?? throw new InvalidOperationException($"Note with ID {id} not found.");
    }

    /// <summary>
    /// Creates a new note for a project in FreeAgent.
    /// </summary>
    /// <param name="projectUrl">The URL of the project to attach the note to.</param>
    /// <param name="note">The <see cref="NoteItem"/> object containing the note details to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="NoteItem"/> object with server-assigned values (e.g., ID, URL, timestamps).
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/notes?project={projectUrl} to create a new note. The cache is
    /// invalidated after a successful creation.
    /// </remarks>
    public async Task<NoteItem> CreateForProjectAsync(Uri projectUrl, NoteItem note)
    {
        NoteRoot root = new() { Note = note };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{NotesEndPoint}?project={projectUrl}"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        NoteRoot? result = await response.Content.ReadFromJsonAsync<NoteRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.InvalidateAllNotesCaches();

        return result?.Note ?? throw new InvalidOperationException("Failed to deserialize note response.");
    }

    /// <summary>
    /// Creates a new note for a contact in FreeAgent.
    /// </summary>
    /// <param name="contactUrl">The URL of the contact to attach the note to.</param>
    /// <param name="note">The <see cref="NoteItem"/> object containing the note details to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="NoteItem"/> object with server-assigned values (e.g., ID, URL, timestamps).
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/notes?contact={contactUrl} to create a new note. The cache is
    /// invalidated after a successful creation.
    /// </remarks>
    public async Task<NoteItem> CreateForContactAsync(Uri contactUrl, NoteItem note)
    {
        NoteRoot root = new() { Note = note };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{NotesEndPoint}?contact={contactUrl}"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        NoteRoot? result = await response.Content.ReadFromJsonAsync<NoteRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.InvalidateAllNotesCaches();

        return result?.Note ?? throw new InvalidOperationException("Failed to deserialize note response.");
    }

    /// <summary>
    /// Updates an existing note in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the note to update.</param>
    /// <param name="note">The <see cref="NoteItem"/> object containing the updated note details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="NoteItem"/> object with the latest server values.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/notes/{id} to update the note. The cache is invalidated after a successful update.
    /// </remarks>
    public async Task<NoteItem> UpdateAsync(string id, NoteItem note)
    {
        NoteRoot root = new() { Note = note };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{NotesEndPoint}/{id}"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        NoteRoot? result = await response.Content.ReadFromJsonAsync<NoteRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.InvalidateAllNotesCaches();

        return result?.Note ?? throw new InvalidOperationException("Failed to deserialize note response.");
    }

    /// <summary>
    /// Deletes a note from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the note to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls DELETE /v2/notes/{id} to delete the note. The cache is invalidated after successful deletion.
    /// </remarks>
    public async Task DeleteAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.DeleteAsync(new Uri(this.freeAgentClient.ApiBaseUrl, $"{NotesEndPoint}/{id}")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        this.InvalidateAllNotesCaches();
    }
}