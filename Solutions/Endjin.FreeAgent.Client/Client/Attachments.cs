// <copyright file="Attachments.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using System.IO;
using System.Net.Http.Headers;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class Attachments
{
    private const string AttachmentsEndPoint = "v2/attachments";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    public Attachments(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

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

    public async Task<Attachment> UploadAsync(Stream fileStream, string fileName, string contentType, string? description = null)
    {
        using MemoryStream memoryStream = new();
        await fileStream.CopyToAsync(memoryStream).ConfigureAwait(false);
        byte[] fileData = memoryStream.ToArray();

        return await UploadAsync(fileData, fileName, contentType, description).ConfigureAwait(false);
    }

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

    public async Task<byte[]> DownloadAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(
            new Uri(freeAgentClient.ApiBaseUrl, $"{AttachmentsEndPoint}/{id}/download")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
    }

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