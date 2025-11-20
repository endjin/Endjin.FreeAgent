// <copyright file="EstimateItemRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a single <see cref="Domain.EstimateItem"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single estimate item object.
/// </remarks>
/// <seealso cref="EstimateItem"/>
public record EstimateItemRoot
{
    /// <summary>
    /// Gets the estimate item from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.EstimateItem"/> object returned by the API.
    /// </value>
    [JsonPropertyName("estimate_item")]
    public EstimateItem? EstimateItem { get; init; }
}
