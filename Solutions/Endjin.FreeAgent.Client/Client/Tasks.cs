// <copyright file="Tasks.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing tasks via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent tasks, which represent billable or non-billable work items
/// within projects. Tasks define the type of work and billing rates, and are used when logging timeslips.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance when retrieving tasks for projects.
/// </para>
/// </remarks>
/// <seealso cref="TaskItem"/>
/// <seealso cref="Project"/>
/// <seealso cref="Timeslip"/>
public class Tasks
{
    private const string TasksEndPoint = "v2/tasks";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Tasks"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing task data.</param>
    public Tasks(FreeAgentClient client, IMemoryCache cache)
    {
        this.freeAgentClient = client;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    /// <summary>
    /// Creates a new task in FreeAgent.
    /// </summary>
    /// <param name="taskItem">The <see cref="TaskItem"/> object containing the task details to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="TaskItem"/> object with server-assigned values (e.g., ID, URL).
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when the TaskItem.Project property is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/tasks?project={projectId} to create a new task. The project ID is extracted
    /// from the TaskItem.Project property. The cache is not updated as only aggregate queries are cached.
    /// </remarks>
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

    /// <summary>
    /// Retrieves all tasks for a specific project from FreeAgent.
    /// </summary>
    /// <param name="projectUrl">The URI of the project to retrieve tasks for.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="TaskItem"/> objects associated with the specified project.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/tasks?project={projectUrl}, handles pagination automatically, and caches
    /// the result for 5 minutes. All tasks for the project are included regardless of status.
    /// </remarks>
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

    /// <summary>
    /// Updates an existing task in FreeAgent.
    /// </summary>
    /// <param name="taskItem">The <see cref="TaskItem"/> object containing the updated task details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="TaskItem"/> object.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when the TaskItem.Url property is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls PUT /v2/tasks/{taskId} to update the task. The task ID is extracted from the
    /// TaskItem.Url property. The cache is not invalidated as task updates are typically infrequent.
    /// </remarks>
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