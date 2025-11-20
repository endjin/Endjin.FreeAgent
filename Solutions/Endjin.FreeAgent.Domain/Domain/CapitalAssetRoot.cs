// <copyright file="CapitalAssetRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a single <see cref="Domain.CapitalAsset"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single capital asset object.
/// </remarks>
/// <seealso cref="CapitalAsset"/>
public record CapitalAssetRoot
{
    /// <summary>
    /// Gets the capital asset from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.CapitalAsset"/> object returned by the API.
    /// </value>
    [JsonPropertyName("capital_asset")]
    public CapitalAsset? CapitalAsset { get; init; }
}