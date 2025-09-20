// <copyright file="CashFlowRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record CashFlowRoot
{
    [JsonPropertyName("cash_flow")]
    public CashFlow? CashFlow { get; init; }
}