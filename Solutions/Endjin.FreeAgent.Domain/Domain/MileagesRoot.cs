// <copyright file="MileagesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="Mileage"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple mileage objects.
/// </remarks>
/// <seealso cref="Mileage"/>
public record MileagesRoot
{
    /// <summary>
    /// Gets the collection of mileage entries from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="Mileage"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("mileages")]
    public List<Mileage>? Mileages { get; init; }
}