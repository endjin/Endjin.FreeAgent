// <copyright file="CorporationTaxReturns.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing corporation tax returns via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent corporation tax returns, which are annual submissions
/// to HMRC reporting a limited company's taxable profits and corporation tax liability. Corporation tax
/// returns (CT600) must be filed within 12 months of the company's accounting period end. This service
/// allows retrieval of return data and marking returns as filed with HMRC.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance. Cache entries are invalidated automatically
/// when returns are marked as filed or when return status changes.
/// </para>
/// </remarks>
/// <seealso cref="CorporationTaxReturn"/>
/// <seealso cref="Company"/>
public class CorporationTaxReturns
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="CorporationTaxReturns"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing corporation tax return data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public CorporationTaxReturns(FreeAgentClient client, IMemoryCache cache)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(cache);

        this.client = client;
        this.cache = cache;
    }

    /// <summary>
    /// Retrieves all corporation tax returns from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="CorporationTaxReturn"/> objects for all accounting periods.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/corporation_tax_returns and caches the result for 5 minutes.
    /// Returns include both filed and unfiled corporation tax periods.
    /// </remarks>
    public async Task<IEnumerable<CorporationTaxReturn>> GetAllAsync()
    {
        string cacheKey = "corporation_tax_returns_all";
        
        if (this.cache.TryGetValue(cacheKey, out IEnumerable<CorporationTaxReturn>? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, "/v2/corporation_tax_returns")).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        
        CorporationTaxReturnsRoot? root = await response.Content.ReadFromJsonAsync<CorporationTaxReturnsRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        
        IEnumerable<CorporationTaxReturn> returns = root?.CorporationTaxReturns ?? [];
        
        this.cache.Set(cacheKey, returns, TimeSpan.FromMinutes(5));
        
        return returns;
    }

    /// <summary>
    /// Retrieves a specific corporation tax return by its period end date from FreeAgent.
    /// </summary>
    /// <param name="periodEndsOn">The period end date of the corporation tax return.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="CorporationTaxReturn"/> object with all return details.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the return is not found or cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls GET /v2/corporation_tax_returns/{period_ends_on} where period_ends_on is formatted as yyyy-MM-dd.
    /// The result is cached for 5 minutes.
    /// </remarks>
    public async Task<CorporationTaxReturn> GetByPeriodEndDateAsync(DateOnly periodEndsOn)
    {
        string periodEndsOnFormatted = periodEndsOn.ToString("yyyy-MM-dd");
        string cacheKey = $"corporation_tax_return_{periodEndsOnFormatted}";

        if (this.cache.TryGetValue(cacheKey, out CorporationTaxReturn? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, $"/v2/corporation_tax_returns/{periodEndsOnFormatted}")).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        CorporationTaxReturnRoot? root = await response.Content.ReadFromJsonAsync<CorporationTaxReturnRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        CorporationTaxReturn? taxReturn = (root?.CorporationTaxReturn) ?? throw new InvalidOperationException($"Corporation tax return for period ending {periodEndsOnFormatted} not found");

        this.cache.Set(cacheKey, taxReturn, TimeSpan.FromMinutes(5));

        return taxReturn;
    }

    /// <summary>
    /// Marks a corporation tax return as filed with HMRC.
    /// </summary>
    /// <param name="periodEndsOn">The period end date of the corporation tax return to mark as filed.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="CorporationTaxReturn"/> object reflecting the filed status.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/corporation_tax_returns/{period_ends_on}/mark_as_filed where period_ends_on is formatted as yyyy-MM-dd.
    /// It invalidates the cache entries for the return. The filing_status is updated to "marked_as_filed" automatically.
    /// </remarks>
    public async Task<CorporationTaxReturn> MarkAsFiledAsync(DateOnly periodEndsOn)
    {
        string periodEndsOnFormatted = periodEndsOn.ToString("yyyy-MM-dd");

        await this.client.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.client.HttpClient.PutAsync(
            new Uri(this.client.ApiBaseUrl, $"/v2/corporation_tax_returns/{periodEndsOnFormatted}/mark_as_filed"),
            null).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        CorporationTaxReturnRoot? root = await response.Content.ReadFromJsonAsync<CorporationTaxReturnRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove($"corporation_tax_return_{periodEndsOnFormatted}");
        this.cache.Remove("corporation_tax_returns_all");

        return root?.CorporationTaxReturn ?? throw new InvalidOperationException("Failed to mark corporation tax return as filed");
    }

    /// <summary>
    /// Marks a corporation tax return as unfiled with HMRC.
    /// </summary>
    /// <param name="periodEndsOn">The period end date of the corporation tax return to mark as unfiled.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="CorporationTaxReturn"/> object reflecting the unfiled status.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/corporation_tax_returns/{period_ends_on}/mark_as_unfiled where period_ends_on is formatted as yyyy-MM-dd.
    /// It invalidates the cache entries for the return. The filing_status is updated to "unfiled" automatically.
    /// </remarks>
    public async Task<CorporationTaxReturn> MarkAsUnfiledAsync(DateOnly periodEndsOn)
    {
        string periodEndsOnFormatted = periodEndsOn.ToString("yyyy-MM-dd");

        await this.client.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.client.HttpClient.PutAsync(
            new Uri(this.client.ApiBaseUrl, $"/v2/corporation_tax_returns/{periodEndsOnFormatted}/mark_as_unfiled"),
            null).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        CorporationTaxReturnRoot? root = await response.Content.ReadFromJsonAsync<CorporationTaxReturnRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove($"corporation_tax_return_{periodEndsOnFormatted}");
        this.cache.Remove("corporation_tax_returns_all");

        return root?.CorporationTaxReturn ?? throw new InvalidOperationException("Failed to mark corporation tax return as unfiled");
    }

    /// <summary>
    /// Marks a corporation tax return as paid.
    /// </summary>
    /// <param name="periodEndsOn">The period end date of the corporation tax return to mark as paid.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="CorporationTaxReturn"/> object reflecting the paid status.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/corporation_tax_returns/{period_ends_on}/mark_as_paid where period_ends_on is formatted as yyyy-MM-dd.
    /// It invalidates the cache entries for the return. The payment_status is updated to "marked_as_paid" automatically.
    /// </remarks>
    public async Task<CorporationTaxReturn> MarkAsPaidAsync(DateOnly periodEndsOn)
    {
        string periodEndsOnFormatted = periodEndsOn.ToString("yyyy-MM-dd");

        await this.client.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.client.HttpClient.PutAsync(
            new Uri(this.client.ApiBaseUrl, $"/v2/corporation_tax_returns/{periodEndsOnFormatted}/mark_as_paid"),
            null).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        CorporationTaxReturnRoot? root = await response.Content.ReadFromJsonAsync<CorporationTaxReturnRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove($"corporation_tax_return_{periodEndsOnFormatted}");
        this.cache.Remove("corporation_tax_returns_all");

        return root?.CorporationTaxReturn ?? throw new InvalidOperationException("Failed to mark corporation tax return as paid");
    }

    /// <summary>
    /// Marks a corporation tax return as unpaid.
    /// </summary>
    /// <param name="periodEndsOn">The period end date of the corporation tax return to mark as unpaid.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="CorporationTaxReturn"/> object reflecting the unpaid status.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/corporation_tax_returns/{period_ends_on}/mark_as_unpaid where period_ends_on is formatted as yyyy-MM-dd.
    /// It invalidates the cache entries for the return. The payment_status is updated to "unpaid" automatically.
    /// </remarks>
    public async Task<CorporationTaxReturn> MarkAsUnpaidAsync(DateOnly periodEndsOn)
    {
        string periodEndsOnFormatted = periodEndsOn.ToString("yyyy-MM-dd");

        await this.client.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.client.HttpClient.PutAsync(
            new Uri(this.client.ApiBaseUrl, $"/v2/corporation_tax_returns/{periodEndsOnFormatted}/mark_as_unpaid"),
            null).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        CorporationTaxReturnRoot? root = await response.Content.ReadFromJsonAsync<CorporationTaxReturnRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove($"corporation_tax_return_{periodEndsOnFormatted}");
        this.cache.Remove("corporation_tax_returns_all");

        return root?.CorporationTaxReturn ?? throw new InvalidOperationException("Failed to mark corporation tax return as unpaid");
    }
}