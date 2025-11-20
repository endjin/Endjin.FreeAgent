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
        string projectId = taskItem.Project?.Segments.Last() ?? throw new ArgumentException("TaskItem.Project cannot be null.", nameof(taskItem));

        TaskRoot root = new() { TaskItem = taskItem };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(new Uri(this.freeAgentClient.ApiBaseUrl, $"{TasksEndPoint}?project={projectId}"), content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        TaskRoot? result = await response.Content.ReadFromJsonAsync<TaskRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.TaskItem ?? throw new InvalidOperationException("Failed to deserialize task response.");
    }

    /// <summary>
    /// Retrieves a specific task by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the task to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="TaskItem"/> object with the specified ID.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no task with the specified ID is found.</exception>
    /// <remarks>
    /// This method calls GET /v2/tasks/{id} and caches the result for 5 minutes.
    /// </remarks>
    public async Task<TaskItem> GetByIdAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        string cacheKey = $"{TasksEndPoint}/{id}";

        if (this.cache.TryGetValue(cacheKey, out TaskItem? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(new Uri(this.freeAgentClient.ApiBaseUrl, $"{TasksEndPoint}/{id}")).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        TaskRoot? root = await response.Content.ReadFromJsonAsync<TaskRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        TaskItem? item = root?.TaskItem;

        if (item == null)
        {
            throw new InvalidOperationException($"Task {id} not found");
        }

        this.cache.Set(cacheKey, item, this.cacheEntryOptions);

        return item;
    }

    /// <summary>
    /// Retrieves all tasks from FreeAgent with optional filtering and sorting.
    /// </summary>
    /// <param name="view">
    /// The view filter to apply. Valid values are:
    /// "all" (all tasks), "active" (active tasks only), "completed" (completed tasks only),
    /// "hidden" (hidden tasks only).
    /// </param>
    /// <param name="sort">
    /// Optional sort parameter. Valid values are:
    /// "name", "project", "billing_rate", "created_at", "updated_at".
    /// </param>
    /// <param name="updatedSince">
    /// Optional filter to retrieve only tasks updated after this timestamp.
    /// </param>
    /// <param name="project">
    /// Optional filter to retrieve only tasks for a specific project.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="TaskItem"/> objects matching the specified criteria.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/tasks with the specified query parameters, handles pagination automatically,
    /// and caches the result for 5 minutes using a cache key that includes all parameters.
    /// </remarks>
    public async Task<IEnumerable<TaskItem>> GetAllAsync(
        string? view = null,
        string? sort = null,
        DateTimeOffset? updatedSince = null,
        Uri? project = null)
    {
        List<string> queryParams = [];

        if (!string.IsNullOrEmpty(view))
        {
            queryParams.Add($"view={view}");
        }

        if (!string.IsNullOrEmpty(sort))
        {
            queryParams.Add($"sort={sort}");
        }

        if (updatedSince.HasValue)
        {
            queryParams.Add($"updated_since={updatedSince.Value.ToString("yyyy-MM-dd")}");
        }

        if (project != null)
        {
            queryParams.Add($"project={Uri.EscapeDataString(project.ToString())}");
        }

        string queryString = queryParams.Count > 0 ? $"?{string.Join("&", queryParams)}" : string.Empty;
        string cacheKey = $"{TasksEndPoint}{queryString}";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<TaskItem>? results))
        {
            List<TasksRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<TasksRoot>(
                new Uri(this.freeAgentClient.ApiBaseUrl, $"{TasksEndPoint}{queryString}"))
                .ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Tasks ?? Enumerable.Empty<TaskItem>())];
            this.cache.Set(cacheKey, results, this.cacheEntryOptions);
        }

        return results ?? [];
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
    /// updated <see cref="TaskItem"/> object with server-assigned values (e.g., updated_at).
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when the TaskItem.Url property is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
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

        TaskRoot? result = await response.Content.ReadFromJsonAsync<TaskRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.TaskItem ?? throw new InvalidOperationException("Failed to deserialize task response.");
    }

    /// <summary>
    /// Deletes a task from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the task to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls DELETE /v2/tasks/{id} to delete a task. The task must be deletable
    /// (check <see cref="TaskItem.IsDeletable"/>) before calling this method.
    /// </remarks>
    public async Task DeleteAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.DeleteAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{TasksEndPoint}/{id}")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
    }
}