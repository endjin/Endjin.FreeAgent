// <copyright file="VatReturnsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record VatReturnsRoot
{
    [JsonPropertyName("vat_returns")]
    public List<VatReturn> VatReturns { get; init; } = [];
}