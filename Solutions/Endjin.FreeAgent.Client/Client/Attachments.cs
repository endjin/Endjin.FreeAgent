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
    /// Uploads a file attachment to FreeAgent from a byte array.
    /// </summary>
    /// <param name="fileData">The file content as a byte array.</param>
    /// <param name="fileName">The name of the file being uploaded.</param>
    /// <param name="contentType">The MIME type of the file (e.g., "application/pdf", "image/jpeg").</param>
    /// <param name="description">Optional description for the attachment.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="Attachment"/> object with server-assigned values.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/attachments using multipart/form-data encoding. The uploaded attachment
    /// can then be linked to other FreeAgent entities.
    /// </remarks>
    public async Task<Attachment> UploadAsync(byte[] fileData, string fileName, string contentType, string? description = null)
    {
        using MultipartFormDataContent content = [];

        ByteArrayContent fileContent = new(fileData);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
        content.Add(fileContent, "attachment[file]", fileName);

        if (!string.IsNullOrEmpty(description))
        {
            content.Add(new StringContent(description), "attachment[description]");
        }

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, AttachmentsEndPoint),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        AttachmentRoot? root = await response.Content.ReadFromJsonAsync<AttachmentRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return root?.Attachment ?? throw new InvalidOperationException("Failed to deserialize attachment response.");
    }

    /// <summary>
    /// Uploads a file attachment to FreeAgent from a stream.
    /// </summary>
    /// <param name="fileStream">The stream containing the file content.</param>
    /// <param name="fileName">The name of the file being uploaded.</param>
    /// <param name="contentType">The MIME type of the file (e.g., "application/pdf", "image/jpeg").</param>
    /// <param name="description">Optional description for the attachment.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="Attachment"/> object with server-assigned values.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method reads the entire stream into memory before uploading. For large files, consider using
    /// the byte array overload directly if you already have the data in memory. Calls POST /v2/attachments.
    /// </remarks>
    public async Task<Attachment> UploadAsync(Stream fileStream, string fileName, string contentType, string? description = null)
    {
        using MemoryStream memoryStream = new();
        await fileStream.CopyToAsync(memoryStream).ConfigureAwait(false);
        byte[] fileData = memoryStream.ToArray();

        return await UploadAsync(fileData, fileName, contentType, description).ConfigureAwait(false);
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
    /// This method calls GET /v2/attachments/{id} and caches the result for 5 minutes. To download the
    /// actual file content, use <see cref="DownloadAsync"/>.
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
    /// Downloads the file content of an attachment from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the attachment to download.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// file content as a byte array.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/attachments/{id}/download and returns the raw file content.
    /// The content is not cached. Use <see cref="GetByIdAsync"/> to retrieve attachment metadata.
    /// </remarks>
    public async Task<byte[]> DownloadAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(
            new Uri(freeAgentClient.ApiBaseUrl, $"{AttachmentsEndPoint}/{id}/download")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
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