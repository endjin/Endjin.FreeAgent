// <copyright file="PropertyRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a single <see cref="Domain.Property"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single property object.
/// </remarks>
/// <seealso cref="Property"/>
public record PropertyRoot
{
    /// <summary>
    /// Gets the property resource from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.Property"/> object returned by the API.
    /// </value>
    [JsonPropertyName("property")]
    public Property? Property { get; init; }
}
