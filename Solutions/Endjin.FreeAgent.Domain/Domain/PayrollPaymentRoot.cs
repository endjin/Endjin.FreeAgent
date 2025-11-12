// <copyright file="PayrollPaymentRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record PayrollPaymentRoot
{
    [JsonPropertyName("payroll_payment")]
    public PayrollPayment? PayrollPayment { get; init; }
}