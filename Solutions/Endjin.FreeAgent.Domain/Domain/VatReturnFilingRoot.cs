// <copyright file="VatReturnFilingRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record VatReturnFilingRoot
{
    [JsonPropertyName("vat_return")]
    public VatReturnFiling? VatReturn { get; init; }
}