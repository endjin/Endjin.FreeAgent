// <copyright file="PayrollProfileRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record PayrollProfileRoot
{
    [JsonPropertyName("payroll_profile")]
    public PayrollProfile? PayrollProfile { get; init; }
}