// <copyright file="DepreciationProfilesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record DepreciationProfilesRoot
{
    [JsonPropertyName("depreciation_profiles")]
    public List<DepreciationProfile> DepreciationProfiles { get; init; } = [];
}