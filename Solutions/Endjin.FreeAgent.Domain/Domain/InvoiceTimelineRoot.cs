// <copyright file="InvoiceTimelineRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for invoice timeline entries.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses when retrieving invoice timeline data.
/// </remarks>
/// <seealso cref="InvoiceTimelineEntry"/>
/// <seealso cref="Invoice"/>
public record InvoiceTimelineRoot
{
    /// <summary>
    /// Gets the collection of invoice timeline entries from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="InvoiceTimelineEntry"/> objects representing the invoice activity timeline.
    /// </value>
    [JsonPropertyName("timeline_entries")]
    public List<InvoiceTimelineEntry>? TimelineEntries { get; init; }
}
