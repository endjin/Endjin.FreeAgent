// <copyright file="ProfitAndLoss.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record ProfitAndLoss
{
    [JsonPropertyName("from_date")]
    public DateOnly? FromDate { get; init; }

    [JsonPropertyName("to_date")]
    public DateOnly? ToDate { get; init; }

    [JsonPropertyName("turnover")]
    public decimal? Turnover { get; init; }

    [JsonPropertyName("cost_of_sales")]
    public decimal? CostOfSales { get; init; }

    [JsonPropertyName("gross_profit")]
    public decimal? GrossProfit { get; init; }

    [JsonPropertyName("administrative_expenses")]
    public decimal? AdministrativeExpenses { get; init; }

    [JsonPropertyName("operating_profit")]
    public decimal? OperatingProfit { get; init; }

    [JsonPropertyName("net_profit")]
    public decimal? NetProfit { get; init; }

    [JsonPropertyName("income_entries")]
    public List<ProfitAndLossEntry>? IncomeEntries { get; init; }

    [JsonPropertyName("cost_of_sales_entries")]
    public List<ProfitAndLossEntry>? CostOfSalesEntries { get; init; }

    [JsonPropertyName("administrative_expenses_entries")]
    public List<ProfitAndLossEntry>? AdministrativeExpensesEntries { get; init; }
}