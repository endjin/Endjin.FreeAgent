// <copyright file="CorporationTaxReturnFiling.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record CorporationTaxReturnFiling
{
    [JsonPropertyName("filed_on")]
    public string? FiledOn { get; init; }

    [JsonPropertyName("filed_online")]
    public bool FiledOnline { get; init; }

    [JsonPropertyName("hmrc_reference")]
    public string? HmrcReference { get; init; }
}