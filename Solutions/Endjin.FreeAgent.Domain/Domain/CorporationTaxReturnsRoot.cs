// <copyright file="CorporationTaxReturnsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record CorporationTaxReturnsRoot
{
    [JsonPropertyName("corporation_tax_returns")]
    public List<CorporationTaxReturn>? CorporationTaxReturns { get; init; }
}