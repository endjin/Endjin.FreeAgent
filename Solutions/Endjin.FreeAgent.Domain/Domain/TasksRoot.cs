// <copyright file="TasksRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="TaskItem"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple tasks.
/// </remarks>
/// <seealso cref="TaskItem"/>
public record TasksRoot
{
    /// <summary>
    /// Gets the collection of tasks from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="TaskItem"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("tasks")]
    public List<TaskItem>? Tasks { get; init; }
}