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
    /// Gets the URI reference to the invoice associated with this timeline entry.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Invoice"/> that this activity relates to.
    /// </value>
    [JsonPropertyName("invoice")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Invoice { get; init; }

    /// <summary>
    /// Gets the type of activity or event.
    /// </summary>
    /// <value>
    /// The activity type (e.g., "created", "updated", "email_sent", "paid", "overdue").
    /// </value>
    [JsonPropertyName("activity_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ActivityType { get; init; }

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
    /// Gets the date and time when this activity occurred.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing when this event took place.
    /// </value>
    [JsonPropertyName("dated_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? DatedOn { get; init; }

    /// <summary>
    /// Gets the user who performed this activity.
    /// </summary>
    /// <value>
    /// The name or identifier of the user responsible for this action, if applicable.
    /// </value>
    [JsonPropertyName("user")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? User { get; init; }
}
