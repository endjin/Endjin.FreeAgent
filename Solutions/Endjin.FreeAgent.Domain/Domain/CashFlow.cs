// <copyright file="CashFlow.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record CashFlow
{
    [JsonPropertyName("from_date")]
    public DateOnly? FromDate { get; init; }

    [JsonPropertyName("to_date")]
    public DateOnly? ToDate { get; init; }

    [JsonPropertyName("opening_balance")]
    public decimal? OpeningBalance { get; init; }

    [JsonPropertyName("closing_balance")]
    public decimal? ClosingBalance { get; init; }

    [JsonPropertyName("cash_in_items")]
    public List<CashFlowItem>? CashInItems { get; init; }

    [JsonPropertyName("cash_out_items")]
    public List<CashFlowItem>? CashOutItems { get; init; }

    [JsonPropertyName("total_cash_in")]
    public decimal? TotalCashIn { get; init; }

    [JsonPropertyName("total_cash_out")]
    public decimal? TotalCashOut { get; init; }

    [JsonPropertyName("net_cash_flow")]
    public decimal? NetCashFlow { get; init; }
}