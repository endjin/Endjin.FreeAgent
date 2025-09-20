// <copyright file="PayrollPaymentsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record PayrollPaymentsRoot
{
    [JsonPropertyName("payroll_payments")]
    public List<PayrollPayment>? PayrollPayments { get; init; }
}