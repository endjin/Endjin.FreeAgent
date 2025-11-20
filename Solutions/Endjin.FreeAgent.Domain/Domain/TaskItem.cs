// <copyright file="TaskItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a task within a project in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Tasks define the types of work that can be performed on a project. They are used to categorize timeslips
/// and enable project-level time tracking and reporting. Each task can have its own billing rate and billable status.
/// </para>
/// <para>
/// Tasks support Active and Completed statuses. Completed tasks remain accessible for historical timeslip records
/// but are hidden from active task lists. Billing rates can be specified hourly or daily.
/// </para>
/// <para>
/// API Endpoint: /v2/tasks
/// </para>
/// <para>
/// Minimum Access Level: Time
/// </para>
/// </remarks>
/// <seealso cref="Project"/>
/// <seealso cref="Timeslip"/>
[DebuggerDisplay("Name = {" + nameof(Name) + "}")]
public record TaskItem
{
    /// <summary>
    /// Gets the URI reference to the project this task belongs to.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Project"/> containing this task.
    /// </value>
    [JsonPropertyName("project")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Project { get; init; }

    /// <summary>
    /// Gets the name of the task.
    /// </summary>
    /// <value>
    /// A descriptive name for the type of work this task represents. This field is required.
    /// </value>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Gets the currency code for this task's billing rate.
    /// </summary>
    /// <value>
    /// An ISO 4217 currency code such as "USD", "GBP", or "EUR".
    /// </value>
    [JsonPropertyName("currency")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Currency { get; init; }

    /// <summary>
    /// Gets a value indicating whether time tracked against this task is billable to the client.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if timeslips for this task can be billed to the client; otherwise, <see langword="false"/>
    /// for non-billable internal tasks. This field is required.
    /// </value>
    [JsonPropertyName("is_billable")]
    public required bool IsBillable { get; init; }

    /// <summary>
    /// Gets the billing rate for this task.
    /// </summary>
    /// <value>
    /// The rate charged per unit of time (hour or day) for work on this task.
    /// </value>
    /// <remarks>
    /// Requires "Contacts &amp; Projects" access level to view or modify this property.
    /// </remarks>
    /// <seealso cref="BillingPeriod"/>
    [JsonPropertyName("billing_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? BillingRate { get; init; }

    /// <summary>
    /// Gets the billing period unit for the task rate.
    /// </summary>
    /// <value>
    /// Either "hour" or "day", determining the unit for <see cref="BillingRate"/>.
    /// </value>
    /// <remarks>
    /// Requires "Contacts &amp; Projects" access level to view or modify this property.
    /// </remarks>
    [JsonPropertyName("billing_period")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? BillingPeriod { get; init; }

    /// <summary>
    /// Gets the status of the task.
    /// </summary>
    /// <value>
    /// "Active", "Completed", or "Hidden". Completed and Hidden tasks are excluded from active task lists but remain in the system.
    /// This field is required.
    /// </value>
    [JsonPropertyName("status")]
    public required string Status { get; init; }

    /// <summary>
    /// Gets the date and time when this task was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the creation timestamp in UTC.
    /// </value>
    [JsonPropertyName("created_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this task was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the last modification timestamp in UTC.
    /// </value>
    [JsonPropertyName("updated_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? UpdatedAt { get; init; }

    /// <summary>
    /// Gets a value indicating whether this task can be deleted.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the task can be deleted; otherwise, <see langword="false"/>.
    /// Tasks with associated timeslips typically cannot be deleted.
    /// </value>
    [JsonPropertyName("is_deletable")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsDeletable { get; init; }

    /// <summary>
    /// Gets the unique URI identifier for this task.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this task in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Url { get; init; }
}