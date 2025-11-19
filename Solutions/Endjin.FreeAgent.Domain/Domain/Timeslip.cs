// <copyright file="Timeslip.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a time tracking entry (timeslip) in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Timeslips record hours worked by users on specific tasks within projects. They are fundamental for tracking
/// billable time, project progress, and generating time-based invoices. Each timeslip is associated with a
/// <see cref="Domain.User"/>, <see cref="Domain.Project"/>, and <see cref="TaskItem"/>.
/// </para>
/// <para>
/// Timeslips support timer functionality for real-time tracking, where users can start and stop timers
/// that automatically calculate elapsed time. Time is recorded in decimal format (e.g., 1.5 represents 1 hour 30 minutes).
/// </para>
/// <para>
/// Timeslips can be filtered by billing status (unbilled/billed), running timers, date ranges, and associated resources.
/// Batch operations are supported for efficient creation of multiple timeslips.
/// </para>
/// <para>
/// API Endpoint: /v2/timeslips
/// </para>
/// <para>
/// Minimum Access Level: Time
/// </para>
/// </remarks>
/// <seealso cref="User"/>
/// <seealso cref="Project"/>
/// <seealso cref="TaskItem"/>
[DebuggerDisplay("Comment = {" + nameof(Comment) + "} {" + nameof(Hours) + "} hours")]
public record Timeslip
{
    /// <summary>
    /// Gets the unique URI identifier for this timeslip.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this timeslip in the FreeAgent system.
    /// This value is assigned by the API upon creation and is used to reference the timeslip in other resources.
    /// </value>
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the URI reference to the <see cref="Domain.User"/> who performed the work.
    /// </summary>
    /// <value>
    /// The URI of the user recording this time entry. This field is required when creating a timeslip.
    /// </value>
    /// <seealso cref="UserEntry"/>
    [JsonPropertyName("user")]
    public Uri? User { get; init; }

    /// <summary>
    /// Gets the URI reference to the <see cref="Domain.Project"/> this time was worked on.
    /// </summary>
    /// <value>
    /// The URI of the project context for this timeslip. This field is required when creating a timeslip.
    /// </value>
    /// <seealso cref="ProjectEntry"/>
    [JsonPropertyName("project")]
    public Uri? Project { get; init; }

    /// <summary>
    /// Gets the <see cref="Domain.Project"/> object associated with this timeslip.
    /// </summary>
    /// <value>
    /// The full project object when nested resources are requested. This property is not serialized to JSON.
    /// </value>
    /// <seealso cref="Project"/>
    [JsonIgnore]
    public Project? ProjectEntry { get; init; }

    /// <summary>
    /// Gets the URI reference to the <see cref="TaskItem"/> this time was spent on.
    /// </summary>
    /// <value>
    /// The URI of the specific task within the project. This field is required when creating a timeslip.
    /// </value>
    /// <seealso cref="TaskEntry"/>
    [JsonPropertyName("task")]
    public Uri? Task { get; init; }

    /// <summary>
    /// Gets the <see cref="TaskItem"/> object associated with this timeslip.
    /// </summary>
    /// <value>
    /// The full task object when nested resources are requested. This property is not serialized to JSON.
    /// </value>
    /// <seealso cref="Task"/>
    [JsonIgnore]
    public TaskItem? TaskEntry { get; init; }

    /// <summary>
    /// Gets the date when this work was performed.
    /// </summary>
    /// <value>
    /// The date of the work in YYYY-MM-DD format. This field is required when creating a timeslip.
    /// </value>
    [JsonPropertyName("dated_on")]
    public DateTimeOffset? DatedOn { get; init; }

    /// <summary>
    /// Gets the number of hours worked.
    /// </summary>
    /// <value>
    /// The time duration in decimal hours (e.g., 1.5 for 1 hour 30 minutes, 7.5 for a full working day).
    /// </value>
    [JsonPropertyName("hours")]
    public decimal? Hours { get; init; }

    /// <summary>
    /// Gets the descriptive comment or notes about this timeslip.
    /// </summary>
    /// <value>
    /// Free-text notes describing the work performed during this time period.
    /// </value>
    [JsonPropertyName("comment")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Comment { get; init; }

    /// <summary>
    /// Gets the date and time when this timeslip was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the last modification timestamp in UTC.
    /// </value>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this timeslip was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the creation timestamp in UTC.
    /// </value>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>
    /// Gets the URI reference to the invoice this timeslip was billed on.
    /// </summary>
    /// <value>
    /// The URI of the invoice if this timeslip has been billed; otherwise, <c>null</c>.
    /// </value>
    [JsonPropertyName("billed_on_invoice")]
    public Uri? BilledOnInvoice { get; init; }

    /// <summary>
    /// Gets the timer information if a timer is currently running on this timeslip.
    /// </summary>
    /// <value>
    /// A <see cref="Domain.Timer"/> object containing timer state if a timer is running; otherwise, <c>null</c>.
    /// </value>
    /// <seealso cref="Domain.Timer"/>
    [JsonPropertyName("timer")]
    public Timer? Timer { get; init; }

    /// <summary>
    /// Gets the <see cref="Domain.User"/> object who recorded this timeslip.
    /// </summary>
    /// <value>
    /// The full user object when nested resources are requested. This property is not serialized to JSON.
    /// </value>
    /// <seealso cref="User"/>
    [JsonIgnore]
    public User? UserEntry { get; init; }
}