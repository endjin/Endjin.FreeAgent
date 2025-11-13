// <copyright file="TaskRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a <see cref="Domain.TaskItem"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single task.
/// </remarks>
/// <seealso cref="TaskItem"/>
public record TaskRoot
{
    /// <summary>
    /// Gets the task from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.TaskItem"/> object returned by the API.
    /// </value>
    [JsonPropertyName("task")]
    public TaskItem? TaskItem { get; init; }
}