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
    /// <para>
    /// This method calls GET /v2/vat_returns, handles pagination automatically, and caches the
    /// result for 5 minutes. Returns include both filed and unfiled VAT periods.
    /// </para>
    /// <para>
    /// Minimum Access Level: Tax, Accounting &amp; Users
    /// </para>
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
    /// Retrieves a specific VAT return by its period end date from FreeAgent.
    /// </summary>
    /// <param name="id">The period end date of the VAT return (e.g., "2024-03-31").</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="VatReturn"/> object with all VAT return details.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the VAT return is not found or cannot be deserialized.</exception>
    /// <remarks>
    /// <para>
    /// This method calls GET /v2/vat_returns/{period_ends_on} and caches the result for 5 minutes.
    /// </para>
    /// <para>
    /// Minimum Access Level: Tax, Accounting &amp; Users
    /// </para>
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
    /// <param name="periodEndsOn">The period end date of the VAT return to mark as filed (e.g., "2024-03-31").</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="VatReturn"/> object reflecting the filed status.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// <para>
    /// This method calls PUT /v2/vat_returns/{period_ends_on}/mark_as_filed and invalidates the cache entry for the
    /// VAT return. This operation should only be performed after successfully submitting the return to HMRC.
    /// </para>
    /// <para>
    /// Minimum Access Level: Full Access
    /// </para>
    /// </remarks>
    public async Task<VatReturn> MarkAsFiledAsync(string periodEndsOn)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{VatReturnsEndPoint}/{periodEndsOn}/mark_as_filed"),
            null).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        VatReturnRoot? root = await response.Content.ReadFromJsonAsync<VatReturnRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        string cacheKey = $"{VatReturnsEndPoint}/{periodEndsOn}";
        this.cache.Remove(cacheKey);
        this.cache.Remove(VatReturnsEndPoint);

        return root?.VatReturn ?? throw new InvalidOperationException("Failed to deserialize VAT return response.");
    }

    /// <summary>
    /// Marks a VAT return as unfiled with HMRC.
    /// </summary>
    /// <param name="periodEndsOn">The period end date of the VAT return to mark as unfiled (e.g., "2024-03-31").</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="VatReturn"/> object reflecting the unfiled status.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// <para>
    /// This method calls PUT /v2/vat_returns/{period_ends_on}/mark_as_unfiled and invalidates the cache entry for the
    /// VAT return. Use this operation to reverse a previously filed status.
    /// </para>
    /// <para>
    /// Minimum Access Level: Full Access
    /// </para>
    /// </remarks>
    public async Task<VatReturn> MarkAsUnfiledAsync(string periodEndsOn)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{VatReturnsEndPoint}/{periodEndsOn}/mark_as_unfiled"),
            null).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        VatReturnRoot? root = await response.Content.ReadFromJsonAsync<VatReturnRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        string cacheKey = $"{VatReturnsEndPoint}/{periodEndsOn}";
        this.cache.Remove(cacheKey);
        this.cache.Remove(VatReturnsEndPoint);

        return root?.VatReturn ?? throw new InvalidOperationException("Failed to deserialize VAT return response.");
    }

    /// <summary>
    /// Marks a VAT return payment as paid.
    /// </summary>
    /// <param name="periodEndsOn">The period end date of the VAT return (e.g., "2024-03-31").</param>
    /// <param name="paymentDate">The payment due date to mark as paid (e.g., "2024-05-07").</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="VatReturn"/> object reflecting the payment status.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// <para>
    /// This method calls PUT /v2/vat_returns/{period_ends_on}/payments/{payment_date}/mark_as_paid
    /// and invalidates the cache entry for the VAT return.
    /// </para>
    /// <para>
    /// Minimum Access Level: Tax, Accounting &amp; Users (assumed based on similar VAT return operations)
    /// </para>
    /// </remarks>
    public async Task<VatReturn> MarkPaymentAsPaidAsync(string periodEndsOn, string paymentDate)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{VatReturnsEndPoint}/{periodEndsOn}/payments/{paymentDate}/mark_as_paid"),
            null).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        VatReturnRoot? root = await response.Content.ReadFromJsonAsync<VatReturnRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        string cacheKey = $"{VatReturnsEndPoint}/{periodEndsOn}";
        this.cache.Remove(cacheKey);
        this.cache.Remove(VatReturnsEndPoint);

        return root?.VatReturn ?? throw new InvalidOperationException("Failed to deserialize VAT return response.");
    }

    /// <summary>
    /// Marks a VAT return payment as unpaid.
    /// </summary>
    /// <param name="periodEndsOn">The period end date of the VAT return (e.g., "2024-03-31").</param>
    /// <param name="paymentDate">The payment due date to mark as unpaid (e.g., "2024-05-07").</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="VatReturn"/> object reflecting the payment status.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// <para>
    /// This method calls PUT /v2/vat_returns/{period_ends_on}/payments/{payment_date}/mark_as_unpaid
    /// and invalidates the cache entry for the VAT return.
    /// </para>
    /// <para>
    /// Minimum Access Level: Tax, Accounting &amp; Users (assumed based on similar VAT return operations)
    /// </para>
    /// </remarks>
    public async Task<VatReturn> MarkPaymentAsUnpaidAsync(string periodEndsOn, string paymentDate)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{VatReturnsEndPoint}/{periodEndsOn}/payments/{paymentDate}/mark_as_unpaid"),
            null).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        VatReturnRoot? root = await response.Content.ReadFromJsonAsync<VatReturnRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        string cacheKey = $"{VatReturnsEndPoint}/{periodEndsOn}";
        this.cache.Remove(cacheKey);
        this.cache.Remove(VatReturnsEndPoint);

        return root?.VatReturn ?? throw new InvalidOperationException("Failed to deserialize VAT return response.");
    }
}
