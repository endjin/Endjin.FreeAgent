// <copyright file="SalesTaxRate.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record SalesTaxRate
{
    [JsonPropertyName("rate")]
    public decimal Rate { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("valid_from")]
    public DateOnly? ValidFrom { get; init; }

    [JsonPropertyName("valid_to")]
    public DateOnly? ValidTo { get; init; }
}