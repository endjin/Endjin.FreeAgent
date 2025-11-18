// <copyright file="InvoiceTimelineEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents an activity entry in an invoice timeline from the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Timeline entries provide a chronological log of all activities and events related to invoices,
/// such as creation, updates, emails sent, payment received, and status changes. This provides
/// a complete audit trail for invoice lifecycle management.
/// </para>
/// <para>
/// The timeline is useful for tracking communication history, monitoring overdue invoices,
/// and understanding the progression of invoices through their lifecycle.
/// </para>
/// <para>
/// API Endpoint: /v2/invoices/timeline
/// </para>
/// </remarks>
/// <seealso cref="Invoice"/>
public record InvoiceTimelineEntry
{
    /// <summary>
    /// Gets the invoice reference.
    /// </summary>
    /// <value>
    /// The reference number or identifier of the invoice this timeline entry relates to.
    /// </value>
    [JsonPropertyName("reference")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Reference { get; init; }

    /// <summary>
    /// Gets the summary of the timeline entry.
    /// </summary>
    /// <value>
    /// A brief summary of the timeline event.
    /// </value>
    [JsonPropertyName("summary")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Summary { get; init; }

    /// <summary>
    /// Gets the description of the activity.
    /// </summary>
    /// <value>
    /// A human-readable description of what happened in this timeline event.
    /// </value>
    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the date when this activity occurred.
    /// </summary>
    /// <value>
    /// A <see cref="DateOnly"/> representing when this event took place.
    /// </value>
    [JsonPropertyName("dated_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? DatedOn { get; init; }

    /// <summary>
    /// Gets the amount associated with this timeline entry.
    /// </summary>
    /// <value>
    /// The monetary amount related to this timeline event (e.g., payment amount).
    /// </value>
    [JsonPropertyName("amount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Amount { get; init; }
}
