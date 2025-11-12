// <copyright file="BillPaymentRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record BillPaymentRoot
{
    [JsonPropertyName("bill")]
    public BillPayment? Bill { get; init; }
}