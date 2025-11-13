// <copyright file="NoteItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a note or comment attached to an entity in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Notes allow users to add comments, explanations, or additional information to various entities such as
/// contacts, projects, invoices, and other records. Notes support collaboration by tracking the author and
/// timestamps, providing an audit trail of communications and decisions.
/// </para>
/// <para>
/// Each note is associated with a parent entity via the parent URL, creating a hierarchical relationship
/// that maintains context and organization.
/// </para>
/// <para>
/// API Endpoint: /v2/notes
/// </para>
/// <para>
/// Minimum Access Level: Varies by parent resource
/// </para>
/// </remarks>
/// <seealso cref="Contact"/>
/// <seealso cref="Project"/>
public record NoteItem
{
    /// <summary>
    /// Gets the unique URI identifier for this note.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this note in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    /// <summary>
    /// Gets the content of the note.
    /// </summary>
    /// <value>
    /// The text content of the note, which may include comments, explanations, or other relevant information.
    /// </value>
    [JsonPropertyName("note")]
    public string? Note { get; init; }

    /// <summary>
    /// Gets the URI of the parent entity this note is attached to.
    /// </summary>
    /// <value>
    /// The URL of the parent resource (contact, project, invoice, etc.) that this note belongs to.
    /// </value>
    [JsonPropertyName("parent_url")]
    public string? ParentUrl { get; init; }

    /// <summary>
    /// Gets the URI reference to the user who authored this note.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="User"/> who created this note.
    /// </value>
    [JsonPropertyName("author")]
    public string? Author { get; init; }

    /// <summary>
    /// Gets the date and time when this note was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing the creation timestamp in UTC.
    /// </value>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this note was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing the last modification timestamp in UTC.
    /// </value>
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; init; }
}