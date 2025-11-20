// <copyright file="HttpResponseMessageExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using System.Threading;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Extension methods for <see cref="HttpResponseMessage"/> and <see cref="HttpContent"/> to simplify JSON deserialization.
/// </summary>
/// <remarks>
/// These extensions provide convenient methods for deserializing HTTP response content to strongly-typed objects
/// using source-generated JSON serialization for optimal performance. They utilize the shared
/// <see cref="SharedJsonOptions"/> configuration to ensure consistent serialization behavior across the library.
/// </remarks>
public static class HttpResponseMessageExtensions
{
    /// <summary>
    /// Reads the HTTP content as JSON and deserializes it to the specified type using streaming deserialization.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the JSON content to.</typeparam>
    /// <param name="content">The HTTP content to read and deserialize.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized object of type <typeparamref name="T"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the JSON response cannot be deserialized.</exception>
    /// <exception cref="JsonException">Thrown when the JSON is invalid or malformed.</exception>
    /// <remarks>
    /// <para>
    /// This method uses streaming deserialization via <see cref="HttpContentJsonExtensions.ReadFromJsonAsync{T}(HttpContent, System.Text.Json.Serialization.Metadata.JsonTypeInfo{T}, CancellationToken)"/>
    /// for better memory efficiency, especially with large response payloads. It leverages
    /// source-generated JSON serialization from <see cref="SharedJsonOptions.SourceGenOptions"/>
    /// for improved performance and AOT (Ahead-of-Time) compilation compatibility.
    /// </para>
    /// <para>
    /// The method does not check the HTTP status code. Use <see cref="ReadContentAsJsonAsync{T}"/>
    /// if you want automatic status code validation.
    /// </para>
    /// </remarks>
    /// <seealso cref="SharedJsonOptions"/>
    /// <seealso cref="ReadContentAsJsonAsync{T}"/>
    public static async Task<T> ReadAsAsync<T>(this HttpContent content, CancellationToken cancellationToken = default)
    {
        // Use the new streaming API with source generation for best performance
        return await content.ReadFromJsonAsync<T>(SharedJsonOptions.SourceGenOptions, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Failed to deserialize JSON response.");
    }


    /// <summary>
    /// Ensures the HTTP response was successful and reads the content as JSON in a single operation.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the JSON content to.</typeparam>
    /// <param name="response">The HTTP response message to validate and deserialize.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized object of type <typeparamref name="T"/>.</returns>
    /// <exception cref="HttpRequestException">Thrown when the response status code indicates an unsuccessful HTTP request (status code outside 200-299 range).</exception>
    /// <exception cref="InvalidOperationException">Thrown when the JSON response cannot be deserialized.</exception>
    /// <exception cref="JsonException">Thrown when the JSON is invalid or malformed.</exception>
    /// <remarks>
    /// <para>
    /// This convenience method combines two operations:
    /// <list type="number">
    /// <item><description>Calls <see cref="HttpResponseMessage.EnsureSuccessStatusCode"/> to validate the HTTP status code</description></item>
    /// <item><description>Deserializes the response content using streaming JSON deserialization</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// This is the recommended method for most API calls as it provides automatic error handling
    /// for failed HTTP requests. The status code is included in the error message if deserialization fails.
    /// </para>
    /// </remarks>
    /// <seealso cref="SharedJsonOptions"/>
    /// <seealso cref="ReadAsAsync{T}"/>
    public static async Task<T> ReadContentAsJsonAsync<T>(this HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(SharedJsonOptions.SourceGenOptions, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException($"Failed to deserialize JSON response. Status: {response.StatusCode}");
    }
}