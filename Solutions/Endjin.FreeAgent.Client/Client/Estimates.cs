// <copyright file="Estimates.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Text.RegularExpressions;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public partial class Estimates
{
    private const string EstimatesEndPoint = "v2/estimates";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    public Estimates(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    public async Task<IEnumerable<Estimate>> GetAllAsync()
    {
        List<EstimatesRoot> results = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<EstimatesRoot>(new Uri(freeAgentClient.ApiBaseUrl, EstimatesEndPoint)).ConfigureAwait(false);

        return results.SelectMany(x => x.Estimates ?? Enumerable.Empty<Estimate>());
    }

    /// <summary>
    /// Gets all estimates by the supplied status
    /// </summary>
    /// <param name="status">any of: all, recent, draft, non_draft, sent, approved, rejected, invoiced</param>
    /// <returns></returns>
    public async Task<IEnumerable<Estimate>> GetAllByStatusAsync(string status)
    {
        string urlSegment = $"{EstimatesEndPoint}?view={status}";
        string cacheKey = urlSegment;

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<Estimate>? results))
        {
            List<EstimatesRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<EstimatesRoot>(new Uri(freeAgentClient.ApiBaseUrl, urlSegment)).ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Estimates ?? Enumerable.Empty<Estimate>())];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    public async Task<Estimate> GetByIdAsync(string id)
    {
        string urlSegment = $"{EstimatesEndPoint}/{id}";
        string cacheKey = urlSegment;

        if (!this.cache.TryGetValue(cacheKey, out Estimate? results))
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(new Uri(freeAgentClient.ApiBaseUrl, urlSegment));

            response.EnsureSuccessStatusCode();

            EstimateRoot content = await response.Content.ReadAsAsync<EstimateRoot>().ConfigureAwait(false);

            results = content.Estimate;
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? throw new InvalidOperationException($"Estimate with ID {id} not found.");
    }

    public async Task<Dictionary<DateTime, List<(Contact contact, Project project, decimal price)>>> GetProjectedMonthlyRevenue()
    {
        // Local function to extract date from description string
        static DateTime? ExtractDateFromDescription(string? description)
        {
            if (string.IsNullOrEmpty(description))
            {
                return null;
            }

            Match match = MonthYearRegex().Match(description);
            return match.Success ? DateTime.Parse(match.Value) : null;
        }

        string cacheKey = "GetProjectedMonthlyRevenue";

        if (!this.cache.TryGetValue(cacheKey, out Dictionary<DateTime, List<(Contact contact, Project project, decimal price)>>? results))
        {
            Task<IEnumerable<Contact>> contactsTask = this.freeAgentClient.Contacts?.GetAllWithActiveProjectsAsync() ?? Task.FromResult(Enumerable.Empty<Contact>());
            Task<IEnumerable<Project>> projectsTask = this.freeAgentClient.Projects?.GetAllActiveAsync() ?? Task.FromResult(Enumerable.Empty<Project>());
            Task<IEnumerable<Estimate>> approvedEstimatesTask = this.freeAgentClient.Estimates?.GetAllByStatusAsync("approved") ?? Task.FromResult(Enumerable.Empty<Estimate>());
            Task<IEnumerable<Estimate>> draftEstimatesTask = this.freeAgentClient.Estimates?.GetAllByStatusAsync("draft") ?? Task.FromResult(Enumerable.Empty<Estimate>());

            await Task.WhenAll(contactsTask, projectsTask, approvedEstimatesTask, draftEstimatesTask).ConfigureAwait(false);

            List<Contact> contacts = [.. await contactsTask.ConfigureAwait(false)];
            List<Project> projects = [.. await projectsTask.ConfigureAwait(false)];
            List<Estimate> estimates = [.. await approvedEstimatesTask.ConfigureAwait(false)];

            estimates.AddRange([.. await draftEstimatesTask.ConfigureAwait(false)]);

            Dictionary<Project, List<(DateTime? Date, decimal Price)>> estimateLineItems = [];

            foreach (Estimate estimate in estimates)
            {
                try
                {
                    List<(DateTime? Date, decimal Price)> entries = [];
                    Estimate currentEstimate = await (this.freeAgentClient.Estimates?.GetByIdAsync(estimate.Url?.Segments?.Last() ?? string.Empty) ?? Task.FromResult<Estimate>(new Estimate())).ConfigureAwait(false);
                    Contact? contact = contacts.FirstOrDefault(x => x.Url == currentEstimate.Contact);
                    Project? project = projects.FirstOrDefault(x => x.Url == currentEstimate.Project);

                    // Update project with contact and estimate status using immutable pattern
                    if (project != null)
                    {
                        project = project with
                        {
                            ContactEntry = contact,
                            IsEstimate = (estimate.Status == "Draft")
                        };
                    }

                    foreach (EstimateItem item in currentEstimate.EstimateItems.Where(x => x.Price > 0))
                    {
                        DateTime? date = ExtractDateFromDescription(item.Description);
                        decimal price = (item.Price ?? 0m) * (item.Quantity ?? 0m);

                        if (date != null)
                        {
                            entries.Add((date, price));
                        }
                    }

                    if (project != null)
                    {
                        estimateLineItems.Add(project, entries);
                    }
                }
                catch (Exception ex)
                {
                    // Log the error but continue processing other estimates
                    // In production, consider using ILogger instead
                    System.Diagnostics.Debug.WriteLine($"Error processing estimate {estimate.Reference ?? estimate.Url?.ToString() ?? "unknown"}: {ex.Message}");
                }
            }

            Dictionary<DateTime, List<(Contact contact, Project project, decimal price)>> revenueByMonth = [];

            List<DateTime> dateRange = [.. (from key in estimateLineItems.Keys
                             from lineItem in estimateLineItems[key]
                             where lineItem.Date.HasValue
                             group lineItem by lineItem.Date!.Value into itemGroup
                             select itemGroup.Key)];

            foreach (DateTime month in dateRange)
            {
                foreach (Project key in estimateLineItems.Keys)
                {
                    if (estimateLineItems[key].Any(x => x.Date == month))
                    {
                        foreach ((DateTime? Date, decimal Price) in estimateLineItems[key].Where(x => x.Date == month))
                        {
                            List<(Contact contact, Project project, decimal price)> entries = [];

                            if (Date.HasValue && revenueByMonth.TryGetValue(Date.Value, out List<(Contact contact, Project project, decimal price)>? value))
                            {
                                entries = value;
                                entries.Add((key.ContactEntry!, key, Price));
                            }
                            else if (Date.HasValue)
                            {
                                entries.Add((key.ContactEntry!, key, Price));
                                revenueByMonth.Add(Date.Value, entries);
                            }
                        }
                    }
                }
            }

            results = revenueByMonth;
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    [GeneratedRegex(@"(Jan(uary)?|Feb(ruary)?|Mar(ch)?|Apr(il)?|May|Jun(e)?|Jul(y)?|Aug(ust)?|Sep(tember)?|Oct(ober)?|Nov(ember)?|Dec(ember)?)\s+\d{4}")]
    private static partial Regex MonthYearRegex();
}