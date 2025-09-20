// <copyright file="BillRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record BillRoot
{
    [JsonPropertyName("bill")]
    public Bill? Bill { get; init; }
}