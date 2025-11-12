// <copyright file="Tasks.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class Tasks
{
    private const string TasksEndPoint = "v2/tasks";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    public Tasks(FreeAgentClient client, IMemoryCache cache)
    {
        this.freeAgentClient = client;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    public async Task<TaskItem> CreateAsync(TaskItem taskItem)
    {
        string projectId = taskItem.Project?.Split('/').Last() ?? throw new ArgumentException("TaskItem.Project cannot be null.", nameof(taskItem));

        TaskRoot root = new() { TaskItem = taskItem };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(new Uri(this.freeAgentClient.ApiBaseUrl, $"{TasksEndPoint}?project={projectId}"), content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        TaskRoot? result = await response.Content.ReadFromJsonAsync<TaskRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.TaskItem ?? throw new InvalidOperationException("Failed to deserialize task response.");
    }

    public async Task<IEnumerable<TaskItem>> GetAllByProjectUrlAsync(Uri projectUrl)
    {
        string urlSegment = $"{TasksEndPoint}?project={projectUrl}";
        string cacheKey = urlSegment;

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<TaskItem>? results))
        {
            List<TasksRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<TasksRoot>(new Uri(this.freeAgentClient.ApiBaseUrl, urlSegment)).ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Tasks ?? Enumerable.Empty<TaskItem>())];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    public async Task<TaskItem> UpdateTaskAsync(TaskItem taskItem)
    {
        string taskId = taskItem.Url?.Segments.Last() ?? throw new ArgumentException("TaskItem.Url cannot be null.", nameof(taskItem));
        TaskRoot root = new() { TaskItem = taskItem };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(new Uri(this.freeAgentClient.ApiBaseUrl, $"{TasksEndPoint}/{taskId}"), content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        return taskItem;
    }
}
