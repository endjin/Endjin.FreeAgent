// <copyright file="DepreciationProfilesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="Domain.DepreciationProfile"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple depreciation profiles.
/// </remarks>
/// <seealso cref="DepreciationProfile"/>
public record DepreciationProfilesRoot
{
    /// <summary>
    /// Gets the collection of depreciation profiles from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="Domain.DepreciationProfile"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("depreciation_profiles")]
    public List<DepreciationProfile> DepreciationProfiles { get; init; } = [];
}