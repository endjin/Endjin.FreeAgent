// <copyright file="VatReturns.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing VAT returns via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent VAT returns, which are periodic submissions to HMRC
/// reporting the VAT collected on sales and the VAT paid on purchases. VAT returns are typically filed
/// quarterly or monthly depending on the business registration. This service allows retrieval of VAT
/// return data and marking returns as filed with HMRC.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance. Cache entries are invalidated automatically
/// when VAT returns are marked as filed or when return status changes.
/// </para>
/// </remarks>
/// <seealso cref="VatReturn"/>
/// <seealso cref="Company"/>
public class VatReturns
{
    private const string VatReturnsEndPoint = "v2/vat_returns";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="VatReturns"/> class.
    /// </summary>
    /// <param name="freeAgentClient">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing VAT return data.</param>
    public VatReturns(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    /// <summary>
    /// Retrieves all VAT returns from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="VatReturn"/> objects for all VAT periods.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/vat_returns, handles pagination automatically, and caches the
    /// result for 5 minutes. Returns include both filed and unfiled VAT periods.
    /// </remarks>
    public async Task<IEnumerable<VatReturn>> GetAllAsync()
    {
        string cacheKey = VatReturnsEndPoint;

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<VatReturn>? results))
        {
            List<VatReturnsRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<VatReturnsRoot>(new Uri(this.freeAgentClient.ApiBaseUrl, VatReturnsEndPoint)).ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.VatReturns)];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves a specific VAT return by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the VAT return.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="VatReturn"/> object with all VAT return details.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the VAT return is not found or cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls GET /v2/vat_returns/{id} and caches the result for 5 minutes.
    /// </remarks>
    public async Task<VatReturn> GetByIdAsync(string id)
    {
        string cacheKey = $"{VatReturnsEndPoint}/{id}";

        if (!this.cache.TryGetValue(cacheKey, out VatReturn? results))
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(new Uri(freeAgentClient.ApiBaseUrl, $"{VatReturnsEndPoint}/{id}")).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            VatReturnRoot? root = await response.Content.ReadFromJsonAsync<VatReturnRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            results = root?.VatReturn;
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? throw new InvalidOperationException($"VAT return with ID {id} not found.");
    }

    /// <summary>
    /// Marks a VAT return as filed with HMRC.
    /// </summary>
    /// <param name="id">The unique identifier of the VAT return to mark as filed.</param>
    /// <param name="filedOn">The date when the VAT return was filed with HMRC.</param>
    /// <param name="filedOnline">Indicates whether the return was filed online (true) or by post (false).</param>
    /// <param name="hmrcReference">Optional HMRC reference number for the filed return.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="VatReturn"/> object reflecting the filed status.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/vat_returns/{id}/mark_as_filed and invalidates the cache entry for the
    /// VAT return. This operation should only be performed after successfully submitting the return to HMRC.
    /// </remarks>
    public async Task<VatReturn> MarkAsFiledAsync(string id, DateOnly filedOn, bool filedOnline, string? hmrcReference = null)
    {
        VatReturnFilingRoot filingData = new()
        {
            VatReturn = new VatReturnFiling
            {
                FiledOn = filedOn.ToString("yyyy-MM-dd"),
                FiledOnline = filedOnline,
                HmrcReference = hmrcReference
            }
        };

        using JsonContent content = JsonContent.Create(filingData, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{VatReturnsEndPoint}/{id}/mark_as_filed"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        VatReturnRoot? root = await response.Content.ReadFromJsonAsync<VatReturnRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        string cacheKey = $"{VatReturnsEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return root?.VatReturn ?? throw new InvalidOperationException("Failed to deserialize VAT return response.");
    }
}