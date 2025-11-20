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
/// retrieval of return data, marking returns as filed or unfiled, and managing individual payment statuses.
/// </para>
/// <para>
/// All operations require a user ID as returns are user-specific resources. Returns are identified by their
/// period end date (typically 5 April) rather than a numeric ID.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance. Cache entries are invalidated automatically
/// when return or payment status changes.
/// </para>
/// </remarks>
/// <seealso cref="SelfAssessmentReturn"/>
/// <seealso cref="SelfAssessmentPayment"/>
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
    /// Retrieves all self assessment returns for a specific user from FreeAgent.
    /// </summary>
    /// <param name="userId">The user ID for which to retrieve self assessment returns.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="SelfAssessmentReturn"/> objects for the specified user.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="userId"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/users/{userId}/self_assessment_returns and caches the result for 5 minutes.
    /// Returns include both filed and unfiled self assessment periods.
    /// </remarks>
    public async Task<IEnumerable<SelfAssessmentReturn>> GetAllAsync(string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        string url = $"/v2/users/{userId}/self_assessment_returns";
        string cacheKey = $"self_assessment_returns_{userId}";

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
    /// Retrieves a specific self assessment return by its period end date from FreeAgent.
    /// </summary>
    /// <param name="userId">The user ID who owns the self assessment return.</param>
    /// <param name="periodEndsOn">The period end date that identifies the self assessment return (typically 5 April).</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="SelfAssessmentReturn"/> object with all return details including payments.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="userId"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the return is not found or cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls GET /v2/users/{userId}/self_assessment_returns/{periodEndsOn} and caches the result for 5 minutes.
    /// </remarks>
    public async Task<SelfAssessmentReturn> GetByPeriodEndAsync(string userId, DateOnly periodEndsOn)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        string periodEndsOnFormatted = periodEndsOn.ToString("yyyy-MM-dd");
        string cacheKey = $"self_assessment_return_{userId}_{periodEndsOnFormatted}";

        if (this.cache.TryGetValue(cacheKey, out SelfAssessmentReturn? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, $"/v2/users/{userId}/self_assessment_returns/{periodEndsOnFormatted}"));
        response.EnsureSuccessStatusCode();

        SelfAssessmentReturnRoot? root = await response.Content.ReadFromJsonAsync<SelfAssessmentReturnRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        SelfAssessmentReturn? taxReturn = root?.SelfAssessmentReturn;

        if (taxReturn == null)
        {
            throw new InvalidOperationException($"Self assessment return for user {userId} with period ending {periodEndsOnFormatted} not found");
        }

        this.cache.Set(cacheKey, taxReturn, TimeSpan.FromMinutes(5));

        return taxReturn;
    }

    /// <summary>
    /// Marks a self assessment return as filed with HMRC.
    /// </summary>
    /// <param name="userId">The user ID who owns the self assessment return.</param>
    /// <param name="periodEndsOn">The period end date that identifies the self assessment return.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="SelfAssessmentReturn"/> object reflecting the filed status.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="userId"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/users/{userId}/self_assessment_returns/{periodEndsOn}/mark_as_filed
    /// and invalidates the cache entries for the return. Use this operation to mark returns that were
    /// submitted to HMRC outside of FreeAgent (e.g., paper submissions or third-party software).
    /// </remarks>
    public async Task<SelfAssessmentReturn> MarkAsFiledAsync(string userId, DateOnly periodEndsOn)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        string periodEndsOnFormatted = periodEndsOn.ToString("yyyy-MM-dd");

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.PutAsync(
            new Uri(this.client.ApiBaseUrl, $"/v2/users/{userId}/self_assessment_returns/{periodEndsOnFormatted}/mark_as_filed"),
            null);
        response.EnsureSuccessStatusCode();

        SelfAssessmentReturnRoot? root = await response.Content.ReadFromJsonAsync<SelfAssessmentReturnRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.InvalidateCache(userId, periodEndsOnFormatted);

        return root?.SelfAssessmentReturn ?? throw new InvalidOperationException("Failed to mark self assessment return as filed");
    }

    /// <summary>
    /// Marks a self assessment return as unfiled, reversing a previous filed status.
    /// </summary>
    /// <param name="userId">The user ID who owns the self assessment return.</param>
    /// <param name="periodEndsOn">The period end date that identifies the self assessment return.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="SelfAssessmentReturn"/> object reflecting the unfiled status.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="userId"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/users/{userId}/self_assessment_returns/{periodEndsOn}/mark_as_unfiled
    /// and invalidates the cache entries for the return. Use this operation to reverse a manually
    /// marked filed status if the return was incorrectly marked.
    /// </remarks>
    public async Task<SelfAssessmentReturn> MarkAsUnfiledAsync(string userId, DateOnly periodEndsOn)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        string periodEndsOnFormatted = periodEndsOn.ToString("yyyy-MM-dd");

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.PutAsync(
            new Uri(this.client.ApiBaseUrl, $"/v2/users/{userId}/self_assessment_returns/{periodEndsOnFormatted}/mark_as_unfiled"),
            null);
        response.EnsureSuccessStatusCode();

        SelfAssessmentReturnRoot? root = await response.Content.ReadFromJsonAsync<SelfAssessmentReturnRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.InvalidateCache(userId, periodEndsOnFormatted);

        return root?.SelfAssessmentReturn ?? throw new InvalidOperationException("Failed to mark self assessment return as unfiled");
    }

    /// <summary>
    /// Marks a specific payment within a self assessment return as paid.
    /// </summary>
    /// <param name="userId">The user ID who owns the self assessment return.</param>
    /// <param name="periodEndsOn">The period end date that identifies the self assessment return.</param>
    /// <param name="paymentDate">The due date of the payment to mark as paid.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="SelfAssessmentReturn"/> object reflecting the payment status change.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="userId"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/users/{userId}/self_assessment_returns/{periodEndsOn}/payments/{paymentDate}/mark_as_paid
    /// and invalidates the cache entries for the return. Use this operation to record that a tax payment
    /// has been made to HMRC.
    /// </remarks>
    public async Task<SelfAssessmentReturn> MarkPaymentAsPaidAsync(string userId, DateOnly periodEndsOn, DateOnly paymentDate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        string periodEndsOnFormatted = periodEndsOn.ToString("yyyy-MM-dd");
        string paymentDateFormatted = paymentDate.ToString("yyyy-MM-dd");

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.PutAsync(
            new Uri(this.client.ApiBaseUrl, $"/v2/users/{userId}/self_assessment_returns/{periodEndsOnFormatted}/payments/{paymentDateFormatted}/mark_as_paid"),
            null);
        response.EnsureSuccessStatusCode();

        SelfAssessmentReturnRoot? root = await response.Content.ReadFromJsonAsync<SelfAssessmentReturnRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.InvalidateCache(userId, periodEndsOnFormatted);

        return root?.SelfAssessmentReturn ?? throw new InvalidOperationException("Failed to mark payment as paid");
    }

    /// <summary>
    /// Marks a specific payment within a self assessment return as unpaid.
    /// </summary>
    /// <param name="userId">The user ID who owns the self assessment return.</param>
    /// <param name="periodEndsOn">The period end date that identifies the self assessment return.</param>
    /// <param name="paymentDate">The due date of the payment to mark as unpaid.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="SelfAssessmentReturn"/> object reflecting the payment status change.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="userId"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/users/{userId}/self_assessment_returns/{periodEndsOn}/payments/{paymentDate}/mark_as_unpaid
    /// and invalidates the cache entries for the return. Use this operation to reverse a previously
    /// marked payment if it was incorrectly recorded.
    /// </remarks>
    public async Task<SelfAssessmentReturn> MarkPaymentAsUnpaidAsync(string userId, DateOnly periodEndsOn, DateOnly paymentDate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        string periodEndsOnFormatted = periodEndsOn.ToString("yyyy-MM-dd");
        string paymentDateFormatted = paymentDate.ToString("yyyy-MM-dd");

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.PutAsync(
            new Uri(this.client.ApiBaseUrl, $"/v2/users/{userId}/self_assessment_returns/{periodEndsOnFormatted}/payments/{paymentDateFormatted}/mark_as_unpaid"),
            null);
        response.EnsureSuccessStatusCode();

        SelfAssessmentReturnRoot? root = await response.Content.ReadFromJsonAsync<SelfAssessmentReturnRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.InvalidateCache(userId, periodEndsOnFormatted);

        return root?.SelfAssessmentReturn ?? throw new InvalidOperationException("Failed to mark payment as unpaid");
    }

    /// <summary>
    /// Invalidates cached data for a specific self assessment return.
    /// </summary>
    /// <param name="userId">The user ID who owns the self assessment return.</param>
    /// <param name="periodEndsOnFormatted">The formatted period end date string.</param>
    private void InvalidateCache(string userId, string periodEndsOnFormatted)
    {
        this.cache.Remove($"self_assessment_return_{userId}_{periodEndsOnFormatted}");
        this.cache.Remove($"self_assessment_returns_{userId}");
    }
}
