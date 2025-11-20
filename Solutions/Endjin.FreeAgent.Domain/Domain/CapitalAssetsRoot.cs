// <copyright file="CapitalAssetsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="CapitalAsset"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple capital asset objects.
/// </remarks>
/// <seealso cref="CapitalAsset"/>
public record CapitalAssetsRoot
{
    /// <summary>
    /// Gets the collection of capital assets from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="CapitalAsset"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("capital_assets")]
    public List<CapitalAsset>? CapitalAssets { get; init; }
}