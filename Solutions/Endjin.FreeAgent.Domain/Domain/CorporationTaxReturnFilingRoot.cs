// <copyright file="CorporationTaxReturnFilingRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record CorporationTaxReturnFilingRoot
{
    [JsonPropertyName("corporation_tax_return")]
    public CorporationTaxReturnFiling? CorporationTaxReturn { get; init; }
}