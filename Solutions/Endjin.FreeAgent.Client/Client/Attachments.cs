// <copyright file="Attachments.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using System.IO;
using System.Net.Http.Headers;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing file attachments via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides comprehensive access to FreeAgent attachments, which are files that can be
/// attached to various entities such as invoices, bills, expenses, and other documents. Attachments support
/// uploading, downloading, retrieving metadata, and deletion of files.
/// </para>
/// <para>
/// Attachment metadata is cached for 5 minutes to improve performance. Cache entries are invalidated
/// automatically when attachments are deleted. File content itself is not cached.
/// </para>
/// <para>
/// <strong>Content URL Expiration:</strong> The FreeAgent API provides temporary URLs (<see cref="Attachment.ContentSrc"/>,
/// <see cref="Attachment.ContentSrcMedium"/>, <see cref="Attachment.ContentSrcSmall"/>) for accessing attachment content
/// directly. These URLs expire at the time specified in <see cref="Attachment.ExpiresAt"/>. After expiration, you must
/// call <see cref="GetByIdAsync"/> again to obtain fresh URLs.
/// </para>
/// </remarks>
/// <seealso cref="Attachment"/>
/// <seealso cref="Invoice"/>
/// <seealso cref="Bill"/>
/// <seealso cref="Expense"/>
public class Attachments
{
    private const string AttachmentsEndPoint = "v2/attachments";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Attachments"/> class.
    /// </summary>
    /// <param name="freeAgentClient">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing attachment metadata.</param>
    public Attachments(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    /// <summary>
    /// Retrieves attachment metadata by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the attachment.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="Attachment"/> object with its metadata.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the attachment is not found or cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls GET /v2/attachments/{id} and caches the result for 5 minutes.
    /// </remarks>
    public async Task<Attachment> GetByIdAsync(string id)
    {
        string cacheKey = $"{AttachmentsEndPoint}/{id}";

        if (!this.cache.TryGetValue(cacheKey, out Attachment? results))
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(
                new Uri(freeAgentClient.ApiBaseUrl, $"{AttachmentsEndPoint}/{id}")).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            AttachmentRoot? root = await response.Content.ReadFromJsonAsync<AttachmentRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            results = root?.Attachment;
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? throw new InvalidOperationException($"Attachment with ID {id} not found.");
    }

    /// <summary>
    /// Deletes an attachment from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the attachment to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls DELETE /v2/attachments/{id} and invalidates the cache entry for the deleted attachment.
    /// The deletion is permanent and cannot be undone.
    /// </remarks>
    public async Task DeleteAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.DeleteAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{AttachmentsEndPoint}/{id}")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        // Invalidate cache
        string cacheKey = $"{AttachmentsEndPoint}/{id}";
        this.cache.Remove(cacheKey);
    }
}