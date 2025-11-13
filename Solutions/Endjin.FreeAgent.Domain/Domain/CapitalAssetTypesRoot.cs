// <copyright file="CapitalAssetTypesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="Domain.CapitalAssetType"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple capital asset types.
/// </remarks>
/// <seealso cref="CapitalAssetType"/>
public record CapitalAssetTypesRoot
{
    /// <summary>
    /// Gets the collection of capital asset types from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="Domain.CapitalAssetType"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("capital_asset_types")]
    public List<CapitalAssetType>? CapitalAssetTypes { get; init; }
}