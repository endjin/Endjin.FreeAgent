// <copyright file="VatReturnRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record VatReturnRoot
{
    [JsonPropertyName("vat_return")]
    public VatReturn? VatReturn { get; init; }
}