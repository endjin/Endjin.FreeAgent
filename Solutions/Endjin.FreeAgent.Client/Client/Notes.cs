// <copyright file="Notes.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing notes via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent notes, which are text-based annotations that can be
/// attached to various resources in FreeAgent, most commonly projects. Notes help track conversations,
/// decisions, and important information related to project work.
/// </para>
/// <para>
/// Results are cached without expiration to improve performance. Cache entries are stored per project URL.
/// </para>
/// </remarks>
/// <seealso cref="NoteItem"/>
/// <seealso cref="Project"/>
public class Notes
{
    private const string NotesEndPoint = "v2/notes";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

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
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }
}