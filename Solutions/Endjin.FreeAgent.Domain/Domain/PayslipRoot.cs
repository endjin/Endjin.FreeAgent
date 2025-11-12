// <copyright file="PayslipRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record PayslipRoot
{
    [JsonPropertyName("payslip")]
    public Payslip? Payslip { get; init; }
}