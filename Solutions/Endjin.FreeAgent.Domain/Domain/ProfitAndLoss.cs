// <copyright file="ProfitAndLoss.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a profit and loss (income statement) financial report for a company in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// A profit and loss statement (also known as an income statement or P&amp;L) shows a company's financial performance
/// over a specific period, detailing revenues, costs, expenses, and the resulting profit or loss. It is one of the
/// fundamental financial statements used to assess business performance and profitability.
/// </para>
/// <para>
/// The profit and loss statement is structured to show:
/// - Turnover (revenue/sales): Total income from business activities
/// - Cost of Sales: Direct costs of producing goods or services sold
/// - Gross Profit: Turnover minus Cost of Sales
/// - Administrative Expenses: Operating expenses and overheads
/// - Operating Profit: Gross Profit minus Administrative Expenses
/// - Net Profit: Final profit after all adjustments
/// </para>
/// <para>
/// Each section contains both summary totals and detailed entries broken down by accounting category,
/// allowing for analysis of income sources and expense patterns.
/// </para>
/// <para>
/// API Endpoint: /v2/accounting/profit_and_loss
/// </para>
/// <para>
/// Minimum Access Level: Accounting Plus
/// </para>
/// </remarks>
/// <seealso cref="ProfitAndLossEntry"/>
/// <seealso cref="BalanceSheet"/>
/// <seealso cref="CashFlow"/>
public record ProfitAndLoss
{
    /// <summary>
    /// Gets the start date of the reporting period.
    /// </summary>
    /// <value>
    /// The first date of the period covered by this profit and loss statement.
    /// </value>
    [JsonPropertyName("from_date")]
    public DateOnly? FromDate { get; init; }

    /// <summary>
    /// Gets the end date of the reporting period.
    /// </summary>
    /// <value>
    /// The last date of the period covered by this profit and loss statement.
    /// </value>
    [JsonPropertyName("to_date")]
    public DateOnly? ToDate { get; init; }

    /// <summary>
    /// Gets the total turnover (revenue) for the period.
    /// </summary>
    /// <value>
    /// The total income from sales of goods and services before any deductions.
    /// Also known as revenue, sales, or gross receipts.
    /// </value>
    [JsonPropertyName("turnover")]
    public decimal? Turnover { get; init; }

    /// <summary>
    /// Gets the total cost of sales for the period.
    /// </summary>
    /// <value>
    /// The direct costs attributable to producing the goods or services sold, including
    /// materials, direct labor, and manufacturing overheads.
    /// </value>
    [JsonPropertyName("cost_of_sales")]
    public decimal? CostOfSales { get; init; }

    /// <summary>
    /// Gets the gross profit for the period.
    /// </summary>
    /// <value>
    /// The profit remaining after deducting cost of sales from turnover (Turnover - Cost of Sales).
    /// Gross profit represents the profit from core trading activities before operating expenses.
    /// </value>
    [JsonPropertyName("gross_profit")]
    public decimal? GrossProfit { get; init; }

    /// <summary>
    /// Gets the total administrative expenses for the period.
    /// </summary>
    /// <value>
    /// The operating expenses and overheads including salaries, rent, utilities, marketing,
    /// professional fees, and other business expenses not directly related to production.
    /// </value>
    [JsonPropertyName("administrative_expenses")]
    public decimal? AdministrativeExpenses { get; init; }

    /// <summary>
    /// Gets the operating profit for the period.
    /// </summary>
    /// <value>
    /// The profit from normal business operations (Gross Profit - Administrative Expenses).
    /// Also known as EBIT (Earnings Before Interest and Tax).
    /// </value>
    [JsonPropertyName("operating_profit")]
    public decimal? OperatingProfit { get; init; }

    /// <summary>
    /// Gets the net profit for the period.
    /// </summary>
    /// <value>
    /// The final profit after all income, expenses, taxes, and adjustments.
    /// This is the "bottom line" profit available to owners.
    /// </value>
    [JsonPropertyName("net_profit")]
    public decimal? NetProfit { get; init; }

    /// <summary>
    /// Gets the detailed breakdown of income by category.
    /// </summary>
    /// <value>
    /// A list of <see cref="ProfitAndLossEntry"/> objects showing individual income categories
    /// that comprise the total turnover, with values and percentages.
    /// </value>
    [JsonPropertyName("income_entries")]
    public List<ProfitAndLossEntry>? IncomeEntries { get; init; }

    /// <summary>
    /// Gets the detailed breakdown of cost of sales by category.
    /// </summary>
    /// <value>
    /// A list of <see cref="ProfitAndLossEntry"/> objects showing individual cost categories
    /// that comprise the total cost of sales.
    /// </value>
    [JsonPropertyName("cost_of_sales_entries")]
    public List<ProfitAndLossEntry>? CostOfSalesEntries { get; init; }

    /// <summary>
    /// Gets the detailed breakdown of administrative expenses by category.
    /// </summary>
    /// <value>
    /// A list of <see cref="ProfitAndLossEntry"/> objects showing individual expense categories
    /// such as salaries, rent, utilities, and other operating costs.
    /// </value>
    [JsonPropertyName("administrative_expenses_entries")]
    public List<ProfitAndLossEntry>? AdministrativeExpensesEntries { get; init; }
}