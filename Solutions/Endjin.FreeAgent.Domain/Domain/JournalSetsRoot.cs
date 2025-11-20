// <copyright file="JournalSetsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="JournalSet"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple journal set objects.
/// </remarks>
/// <seealso cref="JournalSet"/>
public record JournalSetsRoot
{
    /// <summary>
    /// Gets the collection of journal sets from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="JournalSet"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("journal_sets")]
    public List<JournalSet> JournalSets { get; init; } = [];
}
