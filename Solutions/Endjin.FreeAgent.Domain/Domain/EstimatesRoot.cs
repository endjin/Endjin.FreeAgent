// <copyright file="EstimatesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="Estimate"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple estimate objects.
/// </remarks>
/// <seealso cref="Estimate"/>
public record EstimatesRoot
{
    /// <summary>
    /// Gets the collection of estimates from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="Estimate"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("estimates")]
    public List<Estimate>? Estimates { get; init; }
}