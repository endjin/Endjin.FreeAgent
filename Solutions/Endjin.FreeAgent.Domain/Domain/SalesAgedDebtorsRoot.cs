// <copyright file="SalesAgedDebtorsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record SalesAgedDebtorsRoot
{
    [JsonPropertyName("sales_aged_debtors")]
    public SalesAgedDebtors? SalesAgedDebtors { get; init; }
}