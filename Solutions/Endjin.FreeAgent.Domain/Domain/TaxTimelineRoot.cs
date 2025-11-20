// <copyright file="TaxTimelineRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root response wrapper for the tax timeline API endpoint.
/// </summary>
/// <remarks>
/// This wrapper contains the collection of upcoming tax events and deadlines for the authenticated company.
/// </remarks>
public record TaxTimelineRoot
{
    /// <summary>
    /// Gets the collection of tax timeline events.
    /// </summary>
    /// <value>
    /// A list of <see cref="TaxTimelineItem"/> objects representing upcoming tax obligations and deadlines.
    /// </value>
    /// <seealso cref="TaxTimelineItem"/>
    [JsonPropertyName("timeline_items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<TaxTimelineItem>? TimelineItems { get; init; }
}
