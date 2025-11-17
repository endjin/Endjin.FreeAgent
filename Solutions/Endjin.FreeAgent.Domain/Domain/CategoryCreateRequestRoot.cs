// <copyright file="CategoryCreateRequestRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON request wrapper for creating a <see cref="Category"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON serialization when creating a new category via the FreeAgent API.
/// </remarks>
/// <seealso cref="CategoryCreateRequest"/>
/// <seealso cref="Category"/>
public record CategoryCreateRequestRoot
{
    /// <summary>
    /// Gets the category create request.
    /// </summary>
    /// <value>
    /// The <see cref="CategoryCreateRequest"/> object to be sent to the API.
    /// </value>
    [JsonPropertyName("category")]
    public CategoryCreateRequest? Category { get; init; }
}
