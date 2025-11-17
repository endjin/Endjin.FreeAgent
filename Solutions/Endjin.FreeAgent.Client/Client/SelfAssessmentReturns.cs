// <copyright file="SelfAssessmentReturns.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing self assessment tax returns via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent self assessment tax returns, which are annual submissions
/// to HMRC for sole traders, partnerships, and individuals with income not taxed at source. Self assessment
/// returns (SA100) must be filed by 31 January following the end of the tax year. This service allows
/// retrieval of return data and marking returns as filed with HMRC.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance. Cache entries are invalidated automatically
/// when returns are marked as filed or when return status changes. Returns can be filtered by user.
/// </para>
/// </remarks>
/// <seealso cref="SelfAssessmentReturn"/>
/// <seealso cref="User"/>
public class SelfAssessmentReturns
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelfAssessmentReturns"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing self assessment return data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public SelfAssessmentReturns(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Retrieves self assessment returns from FreeAgent, optionally filtered by user.
    /// </summary>
    /// <param name="userId">Optional user ID to filter returns. If null, returns all self assessment returns.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="SelfAssessmentReturn"/> objects for the specified user or all users.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/self_assessment_returns?user={userId} and caches the result for 5 minutes.
    /// Returns include both filed and unfiled self assessment periods.
    /// </remarks>
    public async Task<IEnumerable<SelfAssessmentReturn>> GetAllAsync(string? userId = null)
    {
        string url = "/v2/self_assessment_returns";
        if (!string.IsNullOrWhiteSpace(userId))
        {
            url += $"?user={userId}";
        }

        string cacheKey = $"self_assessment_returns_{userId ?? "all"}";
        
        if (this.cache.TryGetValue(cacheKey, out IEnumerable<SelfAssessmentReturn>? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, url));
        response.EnsureSuccessStatusCode();
        
        SelfAssessmentReturnsRoot? root = await response.Content.ReadFromJsonAsync<SelfAssessmentReturnsRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        
        IEnumerable<SelfAssessmentReturn> returns = root?.SelfAssessmentReturns ?? [];
        
        this.cache.Set(cacheKey, returns, TimeSpan.FromMinutes(5));
        
        return returns;
    }

    /// <summary>
    /// Retrieves a specific self assessment return by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the self assessment return.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="SelfAssessmentReturn"/> object with all return details.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the return is not found or cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls GET /v2/self_assessment_returns/{id} and caches the result for 5 minutes.
    /// </remarks>
    public async Task<SelfAssessmentReturn> GetByIdAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        
        string cacheKey = $"self_assessment_return_{id}";
        
        if (this.cache.TryGetValue(cacheKey, out SelfAssessmentReturn? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, $"/v2/self_assessment_returns/{id}"));
        response.EnsureSuccessStatusCode();
        
        SelfAssessmentReturnRoot? root = await response.Content.ReadFromJsonAsync<SelfAssessmentReturnRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        
        SelfAssessmentReturn? taxReturn = root?.SelfAssessmentReturn;
        
        if (taxReturn == null)
        {
            throw new InvalidOperationException($"Self assessment return {id} not found");
        }
        
        this.cache.Set(cacheKey, taxReturn, TimeSpan.FromMinutes(5));
        
        return taxReturn;
    }

    /// <summary>
    /// Marks a self assessment return as filed with HMRC.
    /// </summary>
    /// <param name="id">The unique identifier of the self assessment return to mark as filed.</param>
    /// <param name="filedOn">The date when the return was filed with HMRC.</param>
    /// <param name="filedOnline">Indicates whether the return was filed online (true) or by post (false). Defaults to true.</param>
    /// <param name="utrNumber">Optional Unique Taxpayer Reference (UTR) number for the return.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="SelfAssessmentReturn"/> object reflecting the filed status.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/self_assessment_returns/{id}/mark_as_filed and invalidates the cache entries
    /// for the return. This operation should only be performed after successfully submitting the SA100 to HMRC.
    /// </remarks>
    public async Task<SelfAssessmentReturn> MarkAsFiledAsync(string id, DateOnly filedOn, bool filedOnline = true, string? utrNumber = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        SelfAssessmentReturnFilingRoot data = new()
        {
            SelfAssessmentReturn = new SelfAssessmentReturnFiling
            {
                FiledOn = filedOn.ToString("yyyy-MM-dd"),
                FiledOnline = filedOnline,
                UtrNumber = utrNumber
            }
        };

        using JsonContent content = JsonContent.Create(data, options: SharedJsonOptions.SourceGenOptions);

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.PutAsync(new Uri(this.client.ApiBaseUrl, $"/v2/self_assessment_returns/{id}/mark_as_filed"), content);
        response.EnsureSuccessStatusCode();

        SelfAssessmentReturnRoot? root = await response.Content.ReadFromJsonAsync<SelfAssessmentReturnRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove($"self_assessment_return_{id}");
        this.cache.Remove("self_assessment_returns_all");

        return root?.SelfAssessmentReturn ?? throw new InvalidOperationException("Failed to mark self assessment return as filed");
    }
}