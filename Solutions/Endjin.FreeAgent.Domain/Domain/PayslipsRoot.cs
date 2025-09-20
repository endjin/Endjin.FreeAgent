// <copyright file="PayslipsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record PayslipsRoot
{
    [JsonPropertyName("payslips")]
    public List<Payslip> Payslips { get; init; } = [];
}