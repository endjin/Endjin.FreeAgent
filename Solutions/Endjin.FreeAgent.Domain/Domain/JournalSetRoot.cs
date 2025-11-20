// <copyright file="JournalSetRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON request/response wrapper for a <see cref="Domain.JournalSet"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON serialization and deserialization of FreeAgent API operations with journal sets.
/// </remarks>
/// <seealso cref="JournalSet"/>
/// <seealso cref="JournalEntry"/>
public record JournalSetRoot
{
    /// <summary>
    /// Gets the journal set from the API request/response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.JournalSet"/> object for the API transaction.
    /// </value>
    [JsonPropertyName("journal_set")]
    public JournalSet? JournalSet { get; init; }
}