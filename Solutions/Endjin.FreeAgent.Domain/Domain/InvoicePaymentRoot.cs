// <copyright file="InvoicePaymentRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record InvoicePaymentRoot
{
    [JsonPropertyName("invoice")]
    public InvoicePayment? Invoice { get; init; }
}