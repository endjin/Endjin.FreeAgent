// <copyright file="Estimates.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using System.Text.RegularExpressions;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing estimates (quotes) via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent estimates, which are quotes sent to potential customers
/// before work begins. Estimates can be in various statuses (draft, sent, approved, rejected, invoiced) and
/// can later be converted into invoices once accepted.
/// </para>
/// <para>
/// Results are cached for 5 minutes to improve performance. The class also provides specialized methods for
/// calculating projected monthly revenue based on estimate data.
/// </para>
/// </remarks>
/// <seealso cref="Estimate"/>
/// <seealso cref="EstimateItem"/>
/// <seealso cref="Contact"/>
/// <seealso cref="Project"/>
public partial class Estimates
{
    private const string EstimatesEndPoint = "v2/estimates";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Estimates"/> class.
    /// </summary>
    /// <param name="freeAgentClient">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing estimate data.</param>
    public Estimates(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    /// <summary>
    /// Retrieves all estimates from FreeAgent.
    /// </summary>
    /// <param name="updatedSince">Optional filter to retrieve only estimates updated after this timestamp (ISO 8601 format).</param>
    /// <param name="nestedEstimateItems">If true, includes estimate line items in the response. Defaults to false.</param>
    /// <param name="fromDate">Optional start date for filtering estimates (YYYY-MM-DD format).</param>
    /// <param name="toDate">Optional end date for filtering estimates (YYYY-MM-DD format).</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// all <see cref="Estimate"/> objects matching the specified criteria.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/estimates with optional query parameters and handles pagination automatically.
    /// </remarks>
    public async Task<IEnumerable<Estimate>> GetAllAsync(
        DateTimeOffset? updatedSince = null,
        bool nestedEstimateItems = false,
        DateOnly? fromDate = null,
        DateOnly? toDate = null)
    {
        List<string> queryParams = [];

        if (updatedSince.HasValue)
        {
            queryParams.Add($"updated_since={Uri.EscapeDataString(updatedSince.Value.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))}");
        }

        if (nestedEstimateItems)
        {
            queryParams.Add("nested_estimate_items=true");
        }

        if (fromDate.HasValue)
        {
            queryParams.Add($"from_date={fromDate.Value:yyyy-MM-dd}");
        }

        if (toDate.HasValue)
        {
            queryParams.Add($"to_date={toDate.Value:yyyy-MM-dd}");
        }

        string queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;

        List<EstimatesRoot> results = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<EstimatesRoot>(
            new Uri(freeAgentClient.ApiBaseUrl, $"{EstimatesEndPoint}{queryString}")).ConfigureAwait(false);

        return results.SelectMany(x => x.Estimates ?? Enumerable.Empty<Estimate>());
    }

    /// <summary>
    /// Retrieves estimates from FreeAgent filtered by status.
    /// </summary>
    /// <param name="status">The status filter to apply. Valid values: "all", "recent", "draft", "non_draft", "sent", "approved", "rejected", "invoiced".</param>
    /// <param name="updatedSince">Optional filter to retrieve only estimates updated after this timestamp (ISO 8601 format).</param>
    /// <param name="nestedEstimateItems">If true, includes estimate line items in the response. Defaults to false.</param>
    /// <param name="fromDate">Optional start date for filtering estimates (YYYY-MM-DD format).</param>
    /// <param name="toDate">Optional end date for filtering estimates (YYYY-MM-DD format).</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="Estimate"/> objects matching the specified criteria.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/estimates?view={status} with optional additional query parameters, handles pagination
    /// automatically, and caches the result for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<Estimate>> GetAllByStatusAsync(
        string status,
        DateTimeOffset? updatedSince = null,
        bool nestedEstimateItems = false,
        DateOnly? fromDate = null,
        DateOnly? toDate = null)
    {
        List<string> queryParams = [$"view={status}"];

        if (updatedSince.HasValue)
        {
            queryParams.Add($"updated_since={Uri.EscapeDataString(updatedSince.Value.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))}");
        }

        if (nestedEstimateItems)
        {
            queryParams.Add("nested_estimate_items=true");
        }

        if (fromDate.HasValue)
        {
            queryParams.Add($"from_date={fromDate.Value:yyyy-MM-dd}");
        }

        if (toDate.HasValue)
        {
            queryParams.Add($"to_date={toDate.Value:yyyy-MM-dd}");
        }

        string queryString = string.Join("&", queryParams);
        string cacheKey = $"{EstimatesEndPoint}?{queryString}";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<Estimate>? results))
        {
            List<EstimatesRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<EstimatesRoot>(
                new Uri(freeAgentClient.ApiBaseUrl, $"{EstimatesEndPoint}?{queryString}")).ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Estimates ?? Enumerable.Empty<Estimate>())];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves a specific estimate by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the estimate to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="Estimate"/> object with the specified ID.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no estimate with the specified ID is found.</exception>
    /// <remarks>
    /// This method calls GET /v2/estimates/{id} and caches the result for 5 minutes.
    /// </remarks>
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

    /// <summary>
    /// Creates a new estimate in FreeAgent.
    /// </summary>
    /// <param name="estimate">The <see cref="Estimate"/> object containing the estimate details to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="Estimate"/> object with server-assigned values (e.g., ID, URL, reference).
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/estimates to create a new estimate. The cache is not updated as
    /// only aggregate queries are cached.
    /// </remarks>
    public async Task<Estimate> CreateAsync(Estimate estimate)
    {
        EstimateRoot root = new() { Estimate = estimate };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, EstimatesEndPoint),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        EstimateRoot? result = await response.Content.ReadFromJsonAsync<EstimateRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.Estimate ?? throw new InvalidOperationException("Failed to deserialize estimate response.");
    }

    /// <summary>
    /// Updates an existing estimate in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the estimate to update.</param>
    /// <param name="estimate">The <see cref="Estimate"/> object containing the updated estimate details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="Estimate"/> object as returned by the API.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/estimates/{id} to update the estimate. The cache entry for this estimate
    /// is invalidated after a successful update.
    /// </remarks>
    public async Task<Estimate> UpdateAsync(string id, Estimate estimate)
    {
        EstimateRoot root = new() { Estimate = estimate };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{EstimatesEndPoint}/{id}"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        EstimateRoot? result = await response.Content.ReadFromJsonAsync<EstimateRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache for this estimate
        string cacheKey = $"{EstimatesEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return result?.Estimate ?? throw new InvalidOperationException("Failed to deserialize estimate response.");
    }

    /// <summary>
    /// Deletes an estimate from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the estimate to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls DELETE /v2/estimates/{id} to delete the estimate. The cache entry for this estimate
    /// is invalidated after successful deletion. Only estimates in draft status can be deleted.
    /// </remarks>
    public async Task DeleteAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.DeleteAsync(new Uri(this.freeAgentClient.ApiBaseUrl, $"{EstimatesEndPoint}/{id}")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        // Invalidate cache
        string cacheKey = $"{EstimatesEndPoint}/{id}";
        this.cache.Remove(cacheKey);
    }

    /// <summary>
    /// Marks an estimate as sent in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the estimate to mark as sent.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="Estimate"/> object with its status changed to sent.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/estimates/{id}/transitions/mark_as_sent to mark the estimate as sent.
    /// The cache entry for this estimate is invalidated.
    /// </remarks>
    public async Task<Estimate> MarkAsSentAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{EstimatesEndPoint}/{id}/transitions/mark_as_sent"),
            JsonContent.Create(new { })).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        EstimateRoot? root = await response.Content.ReadFromJsonAsync<EstimateRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        string cacheKey = $"{EstimatesEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return root?.Estimate ?? throw new InvalidOperationException("Failed to deserialize estimate response.");
    }

    /// <summary>
    /// Marks an estimate as draft in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the estimate to mark as draft.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="Estimate"/> object with its status changed to draft.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/estimates/{id}/transitions/mark_as_draft to return the estimate to draft status,
    /// allowing further editing. The cache entry for this estimate is invalidated.
    /// </remarks>
    public async Task<Estimate> MarkAsDraftAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{EstimatesEndPoint}/{id}/transitions/mark_as_draft"),
            JsonContent.Create(new { })).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        EstimateRoot? root = await response.Content.ReadFromJsonAsync<EstimateRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        string cacheKey = $"{EstimatesEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return root?.Estimate ?? throw new InvalidOperationException("Failed to deserialize estimate response.");
    }

    /// <summary>
    /// Marks an estimate as approved in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the estimate to mark as approved.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="Estimate"/> object with its status changed to approved.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/estimates/{id}/transitions/mark_as_approved to mark the estimate as approved
    /// by the client. The cache entry for this estimate is invalidated.
    /// </remarks>
    public async Task<Estimate> MarkAsApprovedAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{EstimatesEndPoint}/{id}/transitions/mark_as_approved"),
            JsonContent.Create(new { })).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        EstimateRoot? root = await response.Content.ReadFromJsonAsync<EstimateRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        string cacheKey = $"{EstimatesEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return root?.Estimate ?? throw new InvalidOperationException("Failed to deserialize estimate response.");
    }

    /// <summary>
    /// Marks an estimate as rejected in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the estimate to mark as rejected.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="Estimate"/> object with its status changed to rejected.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/estimates/{id}/transitions/mark_as_rejected to mark the estimate as rejected
    /// by the client. The cache entry for this estimate is invalidated.
    /// </remarks>
    public async Task<Estimate> MarkAsRejectedAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{EstimatesEndPoint}/{id}/transitions/mark_as_rejected"),
            JsonContent.Create(new { })).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        EstimateRoot? root = await response.Content.ReadFromJsonAsync<EstimateRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        string cacheKey = $"{EstimatesEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return root?.Estimate ?? throw new InvalidOperationException("Failed to deserialize estimate response.");
    }

    /// <summary>
    /// Converts an estimate to an invoice in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the estimate to convert.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="Estimate"/> object with status "Invoiced" and an invoice URL reference.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/estimates/{id}/transitions/convert_to_invoice to convert an approved estimate
    /// into a draft invoice. The estimate must be in approved status. The response contains the updated estimate
    /// with status changed to "Invoiced" and an invoice URL. The cache entry for this estimate is invalidated.
    /// </remarks>
    public async Task<Estimate> ConvertToInvoiceAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{EstimatesEndPoint}/{id}/transitions/convert_to_invoice"),
            JsonContent.Create(new { })).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        EstimateRoot? root = await response.Content.ReadFromJsonAsync<EstimateRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        string cacheKey = $"{EstimatesEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return root?.Estimate ?? throw new InvalidOperationException("Failed to deserialize estimate response.");
    }

    /// <summary>
    /// Retrieves the PDF representation of an estimate from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the estimate to retrieve as PDF.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a byte array
    /// with the PDF content of the estimate.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the PDF content is null or invalid.</exception>
    /// <remarks>
    /// This method calls GET /v2/estimates/{id}/pdf to retrieve the estimate as a PDF document.
    /// The API returns a JSON response with base64-encoded PDF content, which is decoded before returning.
    /// The result is not cached as PDF generation may vary.
    /// </remarks>
    public async Task<byte[]> GetPdfAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{EstimatesEndPoint}/{id}/pdf")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        EstimatePdfRoot? result = await response.Content.ReadFromJsonAsync<EstimatePdfRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        if (result?.Pdf?.Content is null)
        {
            throw new InvalidOperationException("The PDF content was not returned in the expected format.");
        }

        return Convert.FromBase64String(result.Pdf.Content);
    }

    /// <summary>
    /// Creates a duplicate of an existing estimate in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the estimate to duplicate.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// newly created <see cref="Estimate"/> object with Draft status and the same details as the original.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/estimates/{id}/duplicate to create a copy of the estimate.
    /// The new estimate will have Draft status with all the same line items and details.
    /// </remarks>
    public async Task<Estimate> DuplicateAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{EstimatesEndPoint}/{id}/duplicate"),
            JsonContent.Create(new { })).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        EstimateRoot? root = await response.Content.ReadFromJsonAsync<EstimateRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return root?.Estimate ?? throw new InvalidOperationException("Failed to deserialize estimate response.");
    }

    /// <summary>
    /// Sends an estimate via email through FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the estimate to send.</param>
    /// <param name="email">The <see cref="EstimateEmail"/> object containing email details (recipients, subject, body).</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="Estimate"/> object with its status changed to sent.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/estimates/{id}/send_email to send the estimate. The estimate status is
    /// automatically changed to "sent". The cache entry for this estimate is invalidated.
    /// </remarks>
    public async Task<Estimate> SendEmailAsync(string id, EstimateEmail email)
    {
        EstimateEmailRoot emailRoot = new()
        {
            Estimate = new EstimateEmailWrapper { Email = email }
        };

        using JsonContent content = JsonContent.Create(emailRoot, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{EstimatesEndPoint}/{id}/send_email"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        EstimateRoot? root = await response.Content.ReadFromJsonAsync<EstimateRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache
        string cacheKey = $"{EstimatesEndPoint}/{id}";
        this.cache.Remove(cacheKey);

        return root?.Estimate ?? throw new InvalidOperationException("Failed to deserialize estimate response.");
    }

    /// <summary>
    /// Retrieves all estimates associated with a specific contact from FreeAgent.
    /// </summary>
    /// <param name="contactUri">The URI of the contact to filter estimates by.</param>
    /// <param name="updatedSince">Optional filter to retrieve only estimates updated after this timestamp.</param>
    /// <param name="nestedEstimateItems">If true, includes estimate line items in the response.</param>
    /// <param name="fromDate">Optional start date for filtering estimates.</param>
    /// <param name="toDate">Optional end date for filtering estimates.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="Estimate"/> objects associated with the specified contact.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/estimates?contact={contactUri}, handles pagination automatically, and caches
    /// the result for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<Estimate>> GetAllByContactAsync(
        Uri contactUri,
        DateTimeOffset? updatedSince = null,
        bool nestedEstimateItems = false,
        DateOnly? fromDate = null,
        DateOnly? toDate = null)
    {
        List<string> queryParams = [$"contact={Uri.EscapeDataString(contactUri.ToString())}"];

        if (updatedSince.HasValue)
        {
            queryParams.Add($"updated_since={Uri.EscapeDataString(updatedSince.Value.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))}");
        }

        if (nestedEstimateItems)
        {
            queryParams.Add("nested_estimate_items=true");
        }

        if (fromDate.HasValue)
        {
            queryParams.Add($"from_date={fromDate.Value:yyyy-MM-dd}");
        }

        if (toDate.HasValue)
        {
            queryParams.Add($"to_date={toDate.Value:yyyy-MM-dd}");
        }

        string queryString = string.Join("&", queryParams);
        string cacheKey = $"{EstimatesEndPoint}?{queryString}";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<Estimate>? results))
        {
            List<EstimatesRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<EstimatesRoot>(
                new Uri(freeAgentClient.ApiBaseUrl, $"{EstimatesEndPoint}?{queryString}")).ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Estimates ?? Enumerable.Empty<Estimate>())];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves all estimates associated with a specific project from FreeAgent.
    /// </summary>
    /// <param name="projectUri">The URI of the project to filter estimates by.</param>
    /// <param name="updatedSince">Optional filter to retrieve only estimates updated after this timestamp.</param>
    /// <param name="nestedEstimateItems">If true, includes estimate line items in the response.</param>
    /// <param name="fromDate">Optional start date for filtering estimates.</param>
    /// <param name="toDate">Optional end date for filtering estimates.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="Estimate"/> objects associated with the specified project.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/estimates?project={projectUri}, handles pagination automatically, and caches
    /// the result for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<Estimate>> GetAllByProjectAsync(
        Uri projectUri,
        DateTimeOffset? updatedSince = null,
        bool nestedEstimateItems = false,
        DateOnly? fromDate = null,
        DateOnly? toDate = null)
    {
        List<string> queryParams = [$"project={Uri.EscapeDataString(projectUri.ToString())}"];

        if (updatedSince.HasValue)
        {
            queryParams.Add($"updated_since={Uri.EscapeDataString(updatedSince.Value.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))}");
        }

        if (nestedEstimateItems)
        {
            queryParams.Add("nested_estimate_items=true");
        }

        if (fromDate.HasValue)
        {
            queryParams.Add($"from_date={fromDate.Value:yyyy-MM-dd}");
        }

        if (toDate.HasValue)
        {
            queryParams.Add($"to_date={toDate.Value:yyyy-MM-dd}");
        }

        string queryString = string.Join("&", queryParams);
        string cacheKey = $"{EstimatesEndPoint}?{queryString}";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<Estimate>? results))
        {
            List<EstimatesRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<EstimatesRoot>(
                new Uri(freeAgentClient.ApiBaseUrl, $"{EstimatesEndPoint}?{queryString}")).ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Estimates ?? Enumerable.Empty<Estimate>())];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves all estimates associated with a specific invoice from FreeAgent.
    /// </summary>
    /// <param name="invoiceUri">The URI of the invoice to filter estimates by.</param>
    /// <param name="updatedSince">Optional filter to retrieve only estimates updated after this timestamp.</param>
    /// <param name="nestedEstimateItems">If true, includes estimate line items in the response.</param>
    /// <param name="fromDate">Optional start date for filtering estimates.</param>
    /// <param name="toDate">Optional end date for filtering estimates.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="Estimate"/> objects associated with the specified invoice.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/estimates?invoice={invoiceUri}, handles pagination automatically, and caches
    /// the result for 5 minutes.
    /// </remarks>
    public async Task<IEnumerable<Estimate>> GetAllByInvoiceAsync(
        Uri invoiceUri,
        DateTimeOffset? updatedSince = null,
        bool nestedEstimateItems = false,
        DateOnly? fromDate = null,
        DateOnly? toDate = null)
    {
        List<string> queryParams = [$"invoice={Uri.EscapeDataString(invoiceUri.ToString())}"];

        if (updatedSince.HasValue)
        {
            queryParams.Add($"updated_since={Uri.EscapeDataString(updatedSince.Value.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))}");
        }

        if (nestedEstimateItems)
        {
            queryParams.Add("nested_estimate_items=true");
        }

        if (fromDate.HasValue)
        {
            queryParams.Add($"from_date={fromDate.Value:yyyy-MM-dd}");
        }

        if (toDate.HasValue)
        {
            queryParams.Add($"to_date={toDate.Value:yyyy-MM-dd}");
        }

        string queryString = string.Join("&", queryParams);
        string cacheKey = $"{EstimatesEndPoint}?{queryString}";

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<Estimate>? results))
        {
            List<EstimatesRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<EstimatesRoot>(
                new Uri(freeAgentClient.ApiBaseUrl, $"{EstimatesEndPoint}?{queryString}")).ConfigureAwait(false);

            results = [.. response.SelectMany(x => x.Estimates ?? Enumerable.Empty<Estimate>())];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    /// <summary>
    /// Calculates projected monthly revenue based on approved and draft estimates.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a dictionary
    /// mapping months to lists of revenue items (contact, project, and price) expected in that month.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// <para>
    /// This method analyzes estimate line items to project revenue by month. It extracts dates from
    /// estimate item descriptions using a regex pattern that matches month and year (e.g., "January 2024").
    /// The method aggregates data from approved and draft estimates, active projects, and contacts.
    /// </para>
    /// <para>
    /// Results are cached for 5 minutes. The calculation involves multiple API calls to retrieve
    /// contacts, projects, and estimates, which are executed in parallel for performance.
    /// </para>
    /// </remarks>
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