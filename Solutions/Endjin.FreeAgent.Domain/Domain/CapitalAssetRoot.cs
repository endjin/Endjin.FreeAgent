// <copyright file="CapitalAssetRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record CapitalAssetRoot
{
    [JsonPropertyName("capital_asset")]
    public CapitalAsset? CapitalAsset { get; init; }
}