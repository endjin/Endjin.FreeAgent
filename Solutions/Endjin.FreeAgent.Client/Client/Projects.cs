// <copyright file="Projects.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain;

using NodaTime;
using NodaTime.Calendars;

using System.Net.Http.Json;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing projects via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent projects, which represent client work, internal initiatives,
/// or other trackable activities. Projects are associated with contacts, can contain tasks and timeslips, and
/// are used to organize billable and non-billable work.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance while maintaining reasonable data freshness,
/// as project information may change during active usage.
/// </para>
/// </remarks>
/// <seealso cref="Project"/>
/// <seealso cref="Contact"/>
/// <seealso cref="TaskItem"/>
/// <seealso cref="Timeslip"/>
public class Projects
{
    private const string ProjectEndPoint = "v2/projects";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Projects"/> class.
    /// </summary>
    /// <param name="freeAgentClient">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing project data.</param>
    public Projects(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    /// <summary>
    /// Creates a new project in FreeAgent.
    /// </summary>
    /// <param name="project">The <see cref="Project"/> object containing the project details to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="Project"/> object with server-assigned values (e.g., ID, URL).
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/projects to create a new project. The cache is not updated as
    /// only aggregate queries are cached.
    /// </remarks>
    public async Task<Project> CreateAsync(Project project)
    {
        await this.freeAgentClient.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        ProjectRoot root = new() { Project = project };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(new Uri(freeAgentClient.ApiBaseUrl, ProjectEndPoint), content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        ProjectRoot? result = await response.Content.ReadFromJsonAsync<ProjectRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.Project ?? throw new InvalidOperationException("Failed to deserialize project response.");
    }

    /// <summary>
    /// Updates an existing project in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the project to update.</param>
    /// <param name="project">The <see cref="Project"/> object containing the updated project details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="Project"/> object with server-assigned values.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/projects/{id} to update an existing project.
    /// </remarks>
    public async Task<Project> UpdateAsync(string id, Project project)
    {
        await this.freeAgentClient.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        ProjectRoot root = new() { Project = project };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(freeAgentClient.ApiBaseUrl, $"{ProjectEndPoint}/{id}"), content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        ProjectRoot? result = await response.Content.ReadFromJsonAsync<ProjectRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.Project ?? throw new InvalidOperationException("Failed to deserialize project response.");
    }

    /// <summary>
    /// Deletes a project from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the project to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls DELETE /v2/projects/{id} to delete a project. The project must be deletable
    /// (check <see cref="Project.IsDeletable"/>) before calling this method.
    /// </remarks>
    public async Task DeleteAsync(string id)
    {
        await this.freeAgentClient.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.DeleteAsync(
            new Uri(freeAgentClient.ApiBaseUrl, $"{ProjectEndPoint}/{id}")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Retrieves all projects associated with a specific contact from FreeAgent.
    /// </summary>
    /// <param name="contactUri">The URI of the contact whose projects to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="Project"/> objects associated with the specified contact.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/projects?contact={contactUri} and handles pagination automatically.
    /// Results are cached for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<Project>> GetByContactAsync(Uri contactUri)
    {
        string urlSegment = $"{ProjectEndPoint}?contact={Uri.EscapeDataString(contactUri.ToString())}";
        string cacheKey = urlSegment;

        if (!this.cache.TryGetValue(cacheKey, out List<Project>? results))
        {
            List<ProjectsRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<ProjectsRoot>(new Uri(freeAgentClient.ApiBaseUrl, urlSegment)).ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Projects)];

            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves all active projects from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// active <see cref="Project"/> objects with their associated <see cref="Contact"/> entries populated.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/projects?view=active, handles pagination automatically, and enriches each
    /// project with its associated contact information. Both projects and contacts are retrieved concurrently
    /// for optimal performance. Results are cached for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<Project>> GetAllActiveAsync()
    {
        return await GetAllActiveAsync(sort: null, nested: null).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves all active projects from FreeAgent with optional sorting and nested contact expansion.
    /// </summary>
    /// <param name="sort">
    /// Optional sorting parameter. Valid values are "name" (default), "contact_name", "contact_display_name",
    /// "created_at", or "updated_at". Prefix with "-" for descending order (e.g., "-updated_at").
    /// </param>
    /// <param name="nested">
    /// When true, returns full contact objects inline instead of just references.
    /// When false, returns only contact URIs and ContactEntry will be populated via separate query.
    /// When null, defaults to false and ContactEntry will be populated for backward compatibility.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// active <see cref="Project"/> objects with their associated <see cref="Contact"/> entries populated.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/projects?view=active with optional parameters, handles pagination automatically,
    /// and enriches each project with contact information. When nested is false or null, contacts are retrieved
    /// separately for backward compatibility. Results are cached for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<Project>> GetAllActiveAsync(string? sort, bool? nested)
    {
        List<string> queryParams = ["view=active"];
        if (!string.IsNullOrEmpty(sort))
        {
            queryParams.Add($"sort={Uri.EscapeDataString(sort)}");
        }
        if (nested.HasValue)
        {
            queryParams.Add($"nested={nested.Value.ToString().ToLowerInvariant()}");
        }

        string urlSegment = $"{ProjectEndPoint}?{string.Join("&", queryParams)}";
        string cacheKey = urlSegment;

        if (!this.cache.TryGetValue(cacheKey, out List<Project>? results))
        {
            // If nested is not true, fetch contacts separately for backward compatibility
            if (!nested.GetValueOrDefault(false))
            {
                Task<IEnumerable<Contact>> contactsTask = this.freeAgentClient.Contacts?.GetAllWithActiveProjectsAsync() ?? Task.FromResult(Enumerable.Empty<Contact>());
                Task<List<ProjectsRoot>> projectsTask = this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<ProjectsRoot>(new Uri(freeAgentClient.ApiBaseUrl, urlSegment));

                await Task.WhenAll(contactsTask, projectsTask).ConfigureAwait(false);

                List<Contact> contacts = [.. (contactsTask.Result)];
                results = [.. (projectsTask.Result).SelectMany(x => x.Projects)];

                // Update projects with their contact entries using immutable pattern
                results = [.. results.Select(project => project with { ContactEntry = contacts.FirstOrDefault(x => x.Url == project.Contact) })];
            }
            else
            {
                // When nested is true, the API returns contacts inline
                List<ProjectsRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<ProjectsRoot>(new Uri(freeAgentClient.ApiBaseUrl, urlSegment)).ConfigureAwait(false);
                results = [.. response.SelectMany(x => x.Projects)];
            }

            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves all completed projects from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// completed <see cref="Project"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/projects?view=completed and handles pagination automatically.
    /// Results are cached for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<Project>> GetAllCompletedAsync()
    {
        return await GetAllCompletedAsync(sort: null, nested: null).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves all completed projects from FreeAgent with optional sorting and nested contact expansion.
    /// </summary>
    /// <param name="sort">
    /// Optional sorting parameter. Valid values are "name" (default), "contact_name", "contact_display_name",
    /// "created_at", or "updated_at". Prefix with "-" for descending order (e.g., "-updated_at").
    /// </param>
    /// <param name="nested">
    /// When true, returns full contact objects inline instead of just references.
    /// When false or null, returns only contact URIs.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// completed <see cref="Project"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/projects?view=completed with optional parameters and handles
    /// pagination automatically. Results are cached for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<Project>> GetAllCompletedAsync(string? sort, bool? nested)
    {
        List<string> queryParams = ["view=completed"];
        if (!string.IsNullOrEmpty(sort))
        {
            queryParams.Add($"sort={Uri.EscapeDataString(sort)}");
        }
        if (nested.HasValue)
        {
            queryParams.Add($"nested={nested.Value.ToString().ToLowerInvariant()}");
        }

        string urlSegment = $"{ProjectEndPoint}?{string.Join("&", queryParams)}";
        string cacheKey = urlSegment;

        if (!this.cache.TryGetValue(cacheKey, out List<Project>? results))
        {
            List<ProjectsRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<ProjectsRoot>(new Uri(freeAgentClient.ApiBaseUrl, urlSegment)).ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Projects)];

            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves all cancelled projects from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// cancelled <see cref="Project"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/projects?view=cancelled and handles pagination automatically.
    /// Results are cached for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<Project>> GetAllCancelledAsync()
    {
        return await GetAllCancelledAsync(sort: null, nested: null).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves all cancelled projects from FreeAgent with optional sorting and nested contact expansion.
    /// </summary>
    /// <param name="sort">
    /// Optional sorting parameter. Valid values are "name" (default), "contact_name", "contact_display_name",
    /// "created_at", or "updated_at". Prefix with "-" for descending order (e.g., "-updated_at").
    /// </param>
    /// <param name="nested">
    /// When true, returns full contact objects inline instead of just references.
    /// When false or null, returns only contact URIs.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// cancelled <see cref="Project"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/projects?view=cancelled with optional parameters and handles
    /// pagination automatically. Results are cached for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<Project>> GetAllCancelledAsync(string? sort, bool? nested)
    {
        List<string> queryParams = ["view=cancelled"];
        if (!string.IsNullOrEmpty(sort))
        {
            queryParams.Add($"sort={Uri.EscapeDataString(sort)}");
        }
        if (nested.HasValue)
        {
            queryParams.Add($"nested={nested.Value.ToString().ToLowerInvariant()}");
        }

        string urlSegment = $"{ProjectEndPoint}?{string.Join("&", queryParams)}";
        string cacheKey = urlSegment;

        if (!this.cache.TryGetValue(cacheKey, out List<Project>? results))
        {
            List<ProjectsRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<ProjectsRoot>(new Uri(freeAgentClient.ApiBaseUrl, urlSegment)).ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Projects)];

            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves all hidden projects from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// hidden <see cref="Project"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/projects?view=hidden and handles pagination automatically.
    /// Results are cached for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<Project>> GetAllHiddenAsync()
    {
        return await GetAllHiddenAsync(sort: null, nested: null).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves all hidden projects from FreeAgent with optional sorting and nested contact expansion.
    /// </summary>
    /// <param name="sort">
    /// Optional sorting parameter. Valid values are "name" (default), "contact_name", "contact_display_name",
    /// "created_at", or "updated_at". Prefix with "-" for descending order (e.g., "-updated_at").
    /// </param>
    /// <param name="nested">
    /// When true, returns full contact objects inline instead of just references.
    /// When false or null, returns only contact URIs.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// hidden <see cref="Project"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/projects?view=hidden with optional parameters and handles
    /// pagination automatically. Results are cached for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<Project>> GetAllHiddenAsync(string? sort, bool? nested)
    {
        List<string> queryParams = ["view=hidden"];
        if (!string.IsNullOrEmpty(sort))
        {
            queryParams.Add($"sort={Uri.EscapeDataString(sort)}");
        }
        if (nested.HasValue)
        {
            queryParams.Add($"nested={nested.Value.ToString().ToLowerInvariant()}");
        }

        string urlSegment = $"{ProjectEndPoint}?{string.Join("&", queryParams)}";
        string cacheKey = urlSegment;

        if (!this.cache.TryGetValue(cacheKey, out List<Project>? results))
        {
            List<ProjectsRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<ProjectsRoot>(new Uri(freeAgentClient.ApiBaseUrl, urlSegment)).ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Projects)];

            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves all projects from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// all <see cref="Project"/> objects in the FreeAgent account.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/projects, handles pagination automatically, and caches the result for
    /// 5 minutes. All projects are included regardless of status.
    /// </remarks>
    public async Task<IEnumerable<Project>> GetAllAsync()
    {
        return await GetAllAsync(sort: null, nested: null).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves all projects from FreeAgent with optional sorting and nested contact expansion.
    /// </summary>
    /// <param name="sort">
    /// Optional sorting parameter. Valid values are "name" (default), "contact_name", "contact_display_name",
    /// "created_at", or "updated_at". Prefix with "-" for descending order (e.g., "-updated_at").
    /// </param>
    /// <param name="nested">
    /// When true, returns full contact objects inline instead of just references.
    /// When false or null, returns only contact URIs.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// all <see cref="Project"/> objects in the FreeAgent account.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/projects with optional query parameters, handles pagination automatically,
    /// and caches the result for 5 minutes. All projects are included regardless of status.
    /// </remarks>
    public async Task<IEnumerable<Project>> GetAllAsync(string? sort, bool? nested)
    {
        List<string> queryParams = [];
        if (!string.IsNullOrEmpty(sort))
        {
            queryParams.Add($"sort={Uri.EscapeDataString(sort)}");
        }
        if (nested.HasValue)
        {
            queryParams.Add($"nested={nested.Value.ToString().ToLowerInvariant()}");
        }

        string queryString = queryParams.Count > 0 ? $"?{string.Join("&", queryParams)}" : string.Empty;
        string urlSegment = $"{ProjectEndPoint}{queryString}";
        string cacheKey = urlSegment;

        if (!this.cache.TryGetValue(cacheKey, out List<Project>? results))
        {
            List<ProjectsRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<ProjectsRoot>(new Uri(freeAgentClient.ApiBaseUrl, urlSegment)).ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Projects)];

            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves a specific project by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the project to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="Project"/> object with the specified ID.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no project with the specified ID is found.</exception>
    /// <remarks>
    /// This method calls GET /v2/projects/{id} and caches the result for 5 minutes.
    /// </remarks>
    public async Task<Project> GetByIdAsync(string id)
    {
        await this.freeAgentClient.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        string cacheKey = $"{ProjectEndPoint}/{id}";

        if (!this.cache.TryGetValue(cacheKey, out Project? results))
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(new Uri(freeAgentClient.ApiBaseUrl, $"{ProjectEndPoint}/{id}")).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            ProjectRoot? root = await response.Content.ReadFromJsonAsync<ProjectRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            results = root?.Project;
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? throw new InvalidOperationException($"Project with ID {id} not found.");
    }

    /// <summary>
    /// Retrieves a specific project by its name from FreeAgent.
    /// </summary>
    /// <param name="name">The name of the project to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="Project"/> object with the specified name.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no project with the specified name is found.</exception>
    /// <remarks>
    /// This method retrieves all projects (using <see cref="GetAllAsync"/>) and performs a case-insensitive
    /// search by project name. Results are cached for 5 minutes once found.
    /// </remarks>
    public async Task<Project> GetByNameAsync(string name)
    {
        string cacheKey = $"{ProjectEndPoint}/{name}";

        if (!this.cache.TryGetValue(cacheKey, out Project? results))
        {
            IEnumerable<Project> projects = await this.GetAllAsync().ConfigureAwait(false);

            results = projects.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase)) ?? throw new InvalidOperationException($"Project with name '{name}' not found.");
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? throw new InvalidOperationException($"Project with name '{name}' not found.");
    }

    /// <summary>
    /// Retrieves detailed timeslip information for all active projects within a specified date range.
    /// </summary>
    /// <param name="range">The date range to filter timeslips by.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a dictionary where
    /// each key is a <see cref="Project"/> and the value is a list of tuples containing detailed timeslip
    /// information including customer, project, user, date, effort, rates, and costs.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// <para>
    /// This method retrieves active projects and their associated timeslips within the specified date range.
    /// For each project, it fetches tasks and timeslips, then calculates detailed metrics including weekly
    /// effort tracking, billing rates, and costs.
    /// </para>
    /// <para>
    /// The method enriches projects with contact information, tasks with their details, and timeslips with
    /// user and task information. Week numbers are calculated using ISO week rules. Results are cached for
    /// 5 minutes with a cache key based on the date range.
    /// </para>
    /// <para>
    /// Only projects with non-zero costs in the date range are included in the result.
    /// </para>
    /// </remarks>
    public async Task<Dictionary<Project, List<(string Customer, string Project, string User, DateTimeOffset Date, int Year, int WeekNumber, decimal Effort, decimal? DayRate, decimal? HourlyRate, string Level, decimal? Cost, string Comment)>>> GetActiveProjectsDetailsByDateRangeAsync(DateInterval range)
    {
        string cacheKey = $"{ProjectEndPoint}/active/date/{range.Start.ToFreeAgentDateString()}/{range.End.ToFreeAgentDateString()}";

        if (!this.cache.TryGetValue(cacheKey, out Dictionary<Project, List<(string Customer, string Project, string User, DateTimeOffset Date, int Year, int WeekNumber, decimal Effort, decimal? DayRate, decimal? HourlyRate, string Level, decimal? Cost, string Comment)>>? results))
        {
            results = [];

            Task<IEnumerable<Contact>> contactsTask = freeAgentClient.Contacts?.GetAllWithActiveProjectsAsync() ?? Task.FromResult(Enumerable.Empty<Contact>());
            Task<IEnumerable<Project>> projectsTask = freeAgentClient.Projects?.GetAllActiveAsync() ?? Task.FromResult(Enumerable.Empty<Project>());
            Task<IEnumerable<User>> usersTask = freeAgentClient.Users?.GetAllUsersAsync() ?? Task.FromResult(Enumerable.Empty<User>());

            await Task.WhenAll(contactsTask, projectsTask, usersTask);

            List<Contact> contacts = [.. (contactsTask.Result)];
            List<Project> projects = [.. (projectsTask.Result)];
            List<User> users = [.. (usersTask.Result)];

            for (int i = 0; i < projects.Count; i++)
            {
                Project project = projects[i];

                List<TaskItem> tasks = project.Url != null ? [.. (await (freeAgentClient.Tasks?.GetAllByProjectUrlAsync(project.Url) ?? Task.FromResult(Enumerable.Empty<TaskItem>())).ConfigureAwait(false))] : [];
                List<Timeslip> timeslips = project.Url != null ? [.. (await (freeAgentClient.Timeslips?.GetByProjectUrlAsync(project.Url.ToString()) ?? Task.FromResult(Enumerable.Empty<Timeslip>())).ConfigureAwait(false)).Where(x => x.DatedOn.HasValue && range.Contains(LocalDate.FromDateTime(x.DatedOn.Value.ToDateTime(TimeOnly.MinValue))))] : [];

                // Update timeslips with their related entries using immutable pattern
                timeslips = [.. timeslips.Select(timeslip =>
                    timeslip with
                    {
                        TaskEntry = tasks.FirstOrDefault(x => x.Url == timeslip.Task),
                        UserEntry = users.FirstOrDefault(x => x.Url == timeslip.User)
                    }
                )];

                // Update project with contact and timeslips using immutable pattern
                projects[i] = project with
                {
                    ContactEntry = contacts.FirstOrDefault(x => x.Url == project.Contact),
                    TimeslipEntries = [.. timeslips]
                };

                List<(string Customer, string Project, string User, DateTimeOffset Date, int Year, int WeekNumber, decimal Effort, decimal? DayRate, decimal? HourlyRate, string Level, decimal? Cost, string Comment)> entries = [];

                foreach (Timeslip timeslip in timeslips)
                {
                    string user = timeslip.UserEntry?.FullName ?? "Unknown";
                    DateTimeOffset date = timeslip.DatedOn.HasValue ? new DateTimeOffset(timeslip.DatedOn.Value.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero) : DateTimeOffset.MinValue;
                    decimal effort = timeslip.Hours ?? 0m;
                    decimal? dayRate = timeslip.TaskEntry?.BillingRate;
                    decimal? hourlyRate = dayRate / 8;
                    string level = timeslip.TaskEntry?.Name ?? string.Empty;
                    decimal? cost = hourlyRate * effort;
                    string comment = timeslip.Comment ?? string.Empty;
                    LocalDate localDate = LocalDate.FromDateTime(date.DateTime);

                    IWeekYearRule rule = WeekYearRules.Iso;
                    int weekNumber = rule.GetWeekOfWeekYear(localDate);

                    entries.Add((
                        Customer: project.ContactEntry?.OrganisationName ?? string.Empty,
                        Project: project.Name ?? string.Empty,
                        User: user,
                        Date: date,
                        Year: date.Year,
                        WeekNumber: weekNumber,
                        Effort: effort,
                        DayRate: dayRate,
                        HourlyRate: hourlyRate,
                        Level: level,
                        Cost: cost,
                        Comment: comment));
                }

                if (entries.Sum(x => x.Cost) != 0)
                {
                    results.Add(project, entries);
                }
            }

            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }
}