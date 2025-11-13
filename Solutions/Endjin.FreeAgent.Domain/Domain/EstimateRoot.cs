// <copyright file="EstimateRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a single <see cref="Domain.Estimate"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single estimate object.
/// </remarks>
/// <seealso cref="Estimate"/>
public record EstimateRoot
{
    /// <summary>
    /// Gets the estimate from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.Estimate"/> object returned by the API.
    /// </value>
    [JsonPropertyName("estimate")]
    public Estimate? Estimate { get; init; }
}