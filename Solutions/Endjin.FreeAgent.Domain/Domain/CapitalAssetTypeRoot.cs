// <copyright file="CapitalAssetTypeRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a <see cref="Domain.CapitalAssetType"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single capital asset type.
/// </remarks>
/// <seealso cref="CapitalAssetType"/>
public record CapitalAssetTypeRoot
{
    /// <summary>
    /// Gets the capital asset type from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.CapitalAssetType"/> object returned by the API.
    /// </value>
    [JsonPropertyName("capital_asset_type")]
    public CapitalAssetType? CapitalAssetType { get; init; }
}