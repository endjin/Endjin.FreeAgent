// <copyright file="MileageRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a single <see cref="Domain.Mileage"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single mileage object.
/// </remarks>
/// <seealso cref="Mileage"/>
public record MileageRoot
{
    /// <summary>
    /// Gets the mileage entry from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.Mileage"/> object returned by the API.
    /// </value>
    [JsonPropertyName("mileage")]
    public Mileage? Mileage { get; init; }
}