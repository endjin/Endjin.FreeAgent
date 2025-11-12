// <copyright file="CapitalAssetsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record CapitalAssetsRoot
{
    [JsonPropertyName("capital_assets")]
    public List<CapitalAsset>? CapitalAssets { get; init; }
}