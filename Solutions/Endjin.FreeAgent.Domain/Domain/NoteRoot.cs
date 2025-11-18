// <copyright file="NoteRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a single <see cref="NoteItem"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON serialization and deserialization of FreeAgent API requests and responses
/// that contain a single note object.
/// </remarks>
/// <seealso cref="NoteItem"/>
public record NoteRoot
{
    /// <summary>
    /// Gets the note resource from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="NoteItem"/> object returned by or sent to the API.
    /// </value>
    [JsonPropertyName("note")]
    public NoteItem? Note { get; init; }
}
