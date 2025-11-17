// <copyright file="CategoryUpdateRequestRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON request wrapper for updating a <see cref="Category"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON serialization when updating a category via the FreeAgent API.
/// </remarks>
/// <seealso cref="CategoryUpdateRequest"/>
/// <seealso cref="Category"/>
public record CategoryUpdateRequestRoot
{
    /// <summary>
    /// Gets the category update request.
    /// </summary>
    /// <value>
    /// The <see cref="CategoryUpdateRequest"/> object to be sent to the API.
    /// </value>
    [JsonPropertyName("category")]
    public CategoryUpdateRequest? Category { get; init; }
}
