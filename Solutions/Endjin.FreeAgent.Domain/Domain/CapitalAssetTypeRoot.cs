// <copyright file="CapitalAssetTypeRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record CapitalAssetTypeRoot
{
    [JsonPropertyName("capital_asset_type")]
    public CapitalAssetType? CapitalAssetType { get; init; }
}