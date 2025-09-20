// <copyright file="Projects.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain;

using NodaTime;
using NodaTime.Calendars;

using System.Net.Http.Json;

namespace Endjin.FreeAgent.Client;

public class Projects
{
    private const string ProjectEndPoint = "v2/projects";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    public Projects(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    public async Task<Project> CreateAsync(Project project)
    {
        ProjectRoot root = new() { Project = project };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(new Uri(freeAgentClient.ApiBaseUrl, ProjectEndPoint), content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        ProjectRoot? result = await response.Content.ReadFromJsonAsync<ProjectRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.Project ?? throw new InvalidOperationException("Failed to deserialize project response.");
    }

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
