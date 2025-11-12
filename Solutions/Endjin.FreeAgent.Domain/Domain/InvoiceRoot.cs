// <copyright file="InvoiceRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record InvoiceRoot
{
    [JsonPropertyName("invoice")]
    public Invoice? Invoice { get; init; }
}