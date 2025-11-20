// <copyright file="Timer.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a running timer on a timeslip in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Timers are used for real-time tracking of work on timeslips. When a timer is running,
/// this object is present on the timeslip and contains information about when the timer
/// effectively started (accounting for any previously recorded time).
/// </para>
/// <para>
/// Timers can be started via POST /v2/timeslips/:id/timer and stopped via DELETE /v2/timeslips/:id/timer.
/// </para>
/// </remarks>
/// <seealso cref="Timeslip"/>
public record Timer
{
    /// <summary>
    /// Gets a value indicating whether the timer is currently running.
    /// </summary>
    /// <value>
    /// Always <c>true</c> when this object is present on a timeslip.
    /// </value>
    [JsonPropertyName("running")]
    public bool Running { get; init; }

    /// <summary>
    /// Gets the effective start time of the timer.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing when the timer effectively started,
    /// which accounts for any time already recorded on the timeslip before the timer was started.
    /// </value>
    [JsonPropertyName("start_from")]
    public DateTimeOffset? StartFrom { get; init; }
}
