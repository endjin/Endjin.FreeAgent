// <copyright file="ProfitAndLoss.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a profit and loss summary report for a company in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// A profit and loss summary provides a high-level view of a company's financial performance
/// over a specific period, showing income, expenses, operating profit, and retained profit figures.
/// All monetary values are returned in the company's native currency.
/// </para>
/// <para>
/// The summary includes:
/// <list type="bullet">
/// <item><description>Income: Total revenue from business activities</description></item>
/// <item><description>Expenses: Total costs and operating expenses</description></item>
/// <item><description>Operating Profit: Day-to-day profit or loss</description></item>
/// <item><description>Deductions: Items reducing operating profit (e.g., tax, dividends)</description></item>
/// <item><description>Retained Profit: Profit available for distribution or reinvestment</description></item>
/// </list>
/// </para>
/// <para>
/// API Endpoint: GET /v2/accounting/profit_and_loss/summary
/// </para>
/// <para>
/// Minimum Access Level: Tax, Accounting &amp; Users
/// </para>
/// </remarks>
/// <seealso cref="ProfitAndLossDeduction"/>
/// <seealso cref="BalanceSheet"/>
/// <seealso cref="CashFlow"/>
public record ProfitAndLoss
{
    /// <summary>
    /// Gets the start date of the reporting period.
    /// </summary>
    /// <value>
    /// The first date of the period covered by this profit and loss summary.
    /// </value>
    [JsonPropertyName("from")]
    public DateOnly? From { get; init; }

    /// <summary>
    /// Gets the end date of the reporting period.
    /// </summary>
    /// <value>
    /// The last date of the period covered by this profit and loss summary.
    /// </value>
    [JsonPropertyName("to")]
    public DateOnly? To { get; init; }

    /// <summary>
    /// Gets the total income for the period.
    /// </summary>
    /// <value>
    /// The total revenue from all business activities during the reporting period.
    /// </value>
    [JsonPropertyName("income")]
    public decimal? Income { get; init; }

    /// <summary>
    /// Gets the total expenses for the period.
    /// </summary>
    /// <value>
    /// The total costs and operating expenses incurred during the reporting period.
    /// </value>
    [JsonPropertyName("expenses")]
    public decimal? Expenses { get; init; }

    /// <summary>
    /// Gets the operating profit for the period.
    /// </summary>
    /// <value>
    /// The profit from day-to-day business operations (Income minus Expenses).
    /// </value>
    [JsonPropertyName("operating_profit")]
    public decimal? OperatingProfit { get; init; }

    /// <summary>
    /// Gets the deductions from operating profit.
    /// </summary>
    /// <value>
    /// A list of <see cref="ProfitAndLossDeduction"/> items that reduce operating profit,
    /// such as corporation tax, dividends, or director's drawings.
    /// </value>
    [JsonPropertyName("less")]
    public List<ProfitAndLossDeduction>? Less { get; init; }

    /// <summary>
    /// Gets the retained profit for the period.
    /// </summary>
    /// <value>
    /// The profit remaining after all deductions from operating profit.
    /// </value>
    [JsonPropertyName("retained_profit")]
    public decimal? RetainedProfit { get; init; }

    /// <summary>
    /// Gets the retained profit brought forward from the previous accounting year.
    /// </summary>
    /// <value>
    /// The accumulated retained profit carried over from prior accounting periods.
    /// </value>
    [JsonPropertyName("retained_profit_brought_forward")]
    public decimal? RetainedProfitBroughtForward { get; init; }

    /// <summary>
    /// Gets the retained profit carried forward to the next accounting year.
    /// </summary>
    /// <value>
    /// The total distributable profit, calculated as retained profit plus retained profit brought forward.
    /// This represents the accumulated profits available for distribution to shareholders or reinvestment.
    /// </value>
    [JsonPropertyName("retained_profit_carried_forward")]
    public decimal? RetainedProfitCarriedForward { get; init; }
}
