// <copyright file="CorporationTaxReturnRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record CorporationTaxReturnRoot
{
    [JsonPropertyName("corporation_tax_return")]
    public CorporationTaxReturn? CorporationTaxReturn { get; init; }
}