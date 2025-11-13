// <copyright file="CategoriesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="Category"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple category objects.
/// </remarks>
/// <seealso cref="Category"/>
public record CategoriesRoot
{
    /// <summary>
    /// Gets the collection of accounting categories from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="Category"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("categories")]
    public List<Category> Categories { get; init; } = [];
}