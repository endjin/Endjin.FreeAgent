// <copyright file="SalesTaxRatesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record SalesTaxRatesRoot
{
    [JsonPropertyName("sales_tax_rates")]
    public List<SalesTaxRate> SalesTaxRates { get; init; } = [];
}