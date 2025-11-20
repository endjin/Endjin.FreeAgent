// <copyright file="AttachmentRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a single <see cref="Domain.Attachment"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single attachment object.
/// </remarks>
/// <seealso cref="Attachment"/>
public record AttachmentRoot
{
    /// <summary>
    /// Gets the attachment from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.Attachment"/> object returned by the API.
    /// </value>
    [JsonPropertyName("attachment")]
    public Attachment? Attachment { get; init; }
}