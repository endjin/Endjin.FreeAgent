// <copyright file="PropertiesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="Property"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple property objects.
/// </remarks>
/// <seealso cref="Property"/>
public record PropertiesRoot
{
    /// <summary>
    /// Gets the collection of properties from the API response.
    /// </summary>
    /// <value>
    /// An immutable list of <see cref="Property"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("properties")]
    public ImmutableList<Property> Properties { get; init; } = [];
}
