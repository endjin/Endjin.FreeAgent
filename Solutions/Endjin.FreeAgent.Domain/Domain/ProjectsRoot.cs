// <copyright file="ProjectsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="Project"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple project objects.
/// </remarks>
/// <seealso cref="Project"/>
public record ProjectsRoot
{
    /// <summary>
    /// Gets the collection of projects from the API response.
    /// </summary>
    /// <value>
    /// An immutable list of <see cref="Project"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("projects")]
    public ImmutableList<Project> Projects { get; init; } = [];
}