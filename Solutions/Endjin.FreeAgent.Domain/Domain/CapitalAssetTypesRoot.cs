// <copyright file="CapitalAssetTypesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record CapitalAssetTypesRoot
{
    [JsonPropertyName("capital_asset_types")]
    public List<CapitalAssetType>? CapitalAssetTypes { get; init; }
}