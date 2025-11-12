// <copyright file="Notes.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class Notes
{
    private const string NotesEndPoint = "v2/notes";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    public Notes(FreeAgentClient client, IMemoryCache cache)
    {
        this.freeAgentClient = client;
        this.cache = cache;
    }

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
