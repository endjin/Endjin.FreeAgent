// <copyright file="ProjectRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a single <see cref="Domain.Project"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single project object.
/// </remarks>
/// <seealso cref="Project"/>
public record ProjectRoot
{
    /// <summary>
    /// Gets the project from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.Project"/> object returned by the API.
    /// </value>
    [JsonPropertyName("project")]
    public Project? Project { get; init; }
}