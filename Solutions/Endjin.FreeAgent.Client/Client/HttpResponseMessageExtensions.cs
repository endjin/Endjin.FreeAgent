// <copyright file="HttpResponseMessageExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using System.Threading;

namespace Endjin.FreeAgent.Client;

public static class HttpResponseMessageExtensions
{
    /// <summary>
    /// Reads the HTTP content as JSON and deserializes it to the specified type.
    /// Uses streaming deserialization for better memory efficiency.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="content">The HTTP content.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized object.</returns>
    public static async Task<T> ReadAsAsync<T>(this HttpContent content, CancellationToken cancellationToken = default)
    {
        // Use the new streaming API with source generation for best performance
        return await content.ReadFromJsonAsync<T>(SharedJsonOptions.SourceGenOptions, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Failed to deserialize JSON response.");
    }


    /// <summary>
    /// Ensures the response was successful and reads the content as JSON.
    /// Combines EnsureSuccessStatusCode with streaming deserialization.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="response">The HTTP response message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized object.</returns>
    public static async Task<T> ReadContentAsJsonAsync<T>(this HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(SharedJsonOptions.SourceGenOptions, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException($"Failed to deserialize JSON response. Status: {response.StatusCode}");
    }
}
