// <copyright file="NotesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="NoteItem"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple notes.
/// </remarks>
/// <seealso cref="NoteItem"/>
public record NotesRoot
{
    /// <summary>
    /// Gets the collection of notes from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="NoteItem"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("notes")]
    public List<NoteItem>? Notes { get; init; }
}