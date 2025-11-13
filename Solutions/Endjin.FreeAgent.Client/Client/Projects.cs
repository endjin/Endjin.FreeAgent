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
        ProjectRoot root = new() { Project = project };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(new Uri(freeAgentClient.ApiBaseUrl, ProjectEndPoint), content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        ProjectRoot? result = await response.Content.ReadFromJsonAsync<ProjectRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.Project ?? throw new InvalidOperationException("Failed to deserialize project response.");
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
        string urlSegment = $"{ProjectEndPoint}?view=active";
        string cacheKey = urlSegment;

        if (!this.cache.TryGetValue(cacheKey, out List<Project>? results))
        {
            Task<IEnumerable<Contact>> contactsTask = this.freeAgentClient.Contacts?.GetAllWithActiveProjectsAsync() ?? Task.FromResult(Enumerable.Empty<Contact>());
            Task<List<ProjectsRoot>> projectsTask = this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<ProjectsRoot>(new Uri(freeAgentClient.ApiBaseUrl, urlSegment));

            await Task.WhenAll(contactsTask, projectsTask);

            List<Contact> contacts = [.. (contactsTask.Result)];
            results = [.. (projectsTask.Result).SelectMany(x => x.Projects)];

            // Update projects with their contact entries using immutable pattern
            results = [.. results.Select(project => project with { ContactEntry = contacts.FirstOrDefault(x => x.Url == project.Contact) })];

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
        string cacheKey = ProjectEndPoint;

        if (!this.cache.TryGetValue(cacheKey, out List<Project>? results))
        {
            List<ProjectsRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<ProjectsRoot>(new Uri(freeAgentClient.ApiBaseUrl, ProjectEndPoint)).ConfigureAwait(false);

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
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(new Uri(freeAgentClient.ApiBaseUrl, $"{ProjectEndPoint}/{id}"));

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
                List<Timeslip> timeslips = project.Url != null ? [.. (await (freeAgentClient.Timeslips?.GetByProjectUrlAsync(project.Url.ToString()) ?? Task.FromResult(Enumerable.Empty<Timeslip>())).ConfigureAwait(false)).Where(x => x.DatedOn.HasValue && range.Contains(LocalDate.FromDateTime(x.DatedOn.Value.DateTime)))] : [];

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
                    DateTimeOffset date = timeslip.DatedOn ?? DateTimeOffset.MinValue;
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