// <copyright file="EstimateDefaultAdditionalTextRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for estimate default additional text.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses when retrieving
/// or updating default additional text for estimates.
/// </remarks>
/// <seealso cref="EstimateDefaultAdditionalText"/>
/// <seealso cref="Estimate"/>
public record EstimateDefaultAdditionalTextRoot
{
    /// <summary>
    /// Gets the default additional text object from the API response.
    /// </summary>
    /// <value>
    /// An <see cref="EstimateDefaultAdditionalText"/> object containing the default text template.
    /// </value>
    [JsonPropertyName("estimate")]
    public EstimateDefaultAdditionalText? Estimate { get; init; }
}
