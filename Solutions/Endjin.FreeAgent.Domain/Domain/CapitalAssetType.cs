// <copyright file="CapitalAssetType.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record CapitalAssetType
{
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("allowance_type")]
    public string? AllowanceType { get; init; }

    [JsonPropertyName("rate")]
    public decimal? Rate { get; init; }
}