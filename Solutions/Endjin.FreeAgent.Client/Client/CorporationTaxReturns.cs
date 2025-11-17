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
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
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

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, "/v2/corporation_tax_returns"));
        response.EnsureSuccessStatusCode();
        
        CorporationTaxReturnsRoot? root = await response.Content.ReadFromJsonAsync<CorporationTaxReturnsRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        
        IEnumerable<CorporationTaxReturn> returns = root?.CorporationTaxReturns ?? [];
        
        this.cache.Set(cacheKey, returns, TimeSpan.FromMinutes(5));
        
        return returns;
    }

    /// <summary>
    /// Retrieves a specific corporation tax return by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the corporation tax return.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="CorporationTaxReturn"/> object with all return details.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the return is not found or cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls GET /v2/corporation_tax_returns/{id} and caches the result for 5 minutes.
    /// </remarks>
    public async Task<CorporationTaxReturn> GetByIdAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        
        string cacheKey = $"corporation_tax_return_{id}";
        
        if (this.cache.TryGetValue(cacheKey, out CorporationTaxReturn? cached))
        {
            return cached!;
        }

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, $"/v2/corporation_tax_returns/{id}"));
        response.EnsureSuccessStatusCode();
        
        CorporationTaxReturnRoot? root = await response.Content.ReadFromJsonAsync<CorporationTaxReturnRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);
        CorporationTaxReturn? taxReturn = (root?.CorporationTaxReturn) ?? throw new InvalidOperationException($"Corporation tax return {id} not found");

        this.cache.Set(cacheKey, taxReturn, TimeSpan.FromMinutes(5));
        
        return taxReturn;
    }

    /// <summary>
    /// Marks a corporation tax return as filed with HMRC.
    /// </summary>
    /// <param name="id">The unique identifier of the corporation tax return to mark as filed.</param>
    /// <param name="filedOn">The date when the return was filed with HMRC.</param>
    /// <param name="filedOnline">Indicates whether the return was filed online (true) or by post (false). Defaults to true.</param>
    /// <param name="hmrcReference">Optional HMRC reference number for the filed return.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="CorporationTaxReturn"/> object reflecting the filed status.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/corporation_tax_returns/{id}/mark_as_filed and invalidates the cache entries
    /// for the return. This operation should only be performed after successfully submitting the CT600 to HMRC.
    /// </remarks>
    public async Task<CorporationTaxReturn> MarkAsFiledAsync(
        string id,
        DateOnly filedOn,
        bool filedOnline = true,
        string? hmrcReference = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        CorporationTaxReturnFilingRoot data = new()
        {
            CorporationTaxReturn = new CorporationTaxReturnFiling
            {
                FiledOn = filedOn.ToString("yyyy-MM-dd"),
                FiledOnline = filedOnline,
                HmrcReference = hmrcReference
            }
        };

        using JsonContent content = JsonContent.Create(data, options: SharedJsonOptions.SourceGenOptions);

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.PutAsync(new Uri(this.client.ApiBaseUrl, $"/v2/corporation_tax_returns/{id}/mark_as_filed"), content);
        response.EnsureSuccessStatusCode();

        CorporationTaxReturnRoot? root = await response.Content.ReadFromJsonAsync<CorporationTaxReturnRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.cache.Remove($"corporation_tax_return_{id}");
        this.cache.Remove("corporation_tax_returns_all");

        return root?.CorporationTaxReturn ?? throw new InvalidOperationException("Failed to mark corporation tax return as filed");
    }
}