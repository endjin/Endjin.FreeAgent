// <copyright file="BalanceSheetRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record BalanceSheetRoot
{
    [JsonPropertyName("balance_sheet")]
    public BalanceSheet? BalanceSheet { get; init; }
}