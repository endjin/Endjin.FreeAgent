// <copyright file="CashFlow.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a cash flow statement for a company in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// A cash flow statement tracks the movement of cash in and out of the business over a specific period,
/// showing actual money received and paid rather than accrual-based accounting figures. It is one of the
/// fundamental financial statements used to assess liquidity and cash management.
/// </para>
/// <para>
/// The cash flow statement shows:
/// - Opening Balance: Cash position at the start of the period
/// - Cash In Items: Receipts from customers, loans, and other sources
/// - Cash Out Items: Payments to suppliers, expenses, taxes, and other outflows
/// - Net Cash Flow: The difference between cash in and cash out
/// - Closing Balance: Cash position at the end of the period
/// </para>
/// <para>
/// This statement is crucial for understanding the company's ability to generate cash, meet obligations,
/// and fund operations. A profitable company can still face cash flow problems if income is not received
/// in time to cover expenses.
/// </para>
/// <para>
/// API Endpoint: /v2/accounting/cash_flow
/// </para>
/// <para>
/// Minimum Access Level: Accounting Plus
/// </para>
/// </remarks>
/// <seealso cref="CashFlowItem"/>
/// <seealso cref="BankAccount"/>
/// <seealso cref="ProfitAndLoss"/>
public record CashFlow
{
    /// <summary>
    /// Gets the start date of the reporting period.
    /// </summary>
    /// <value>
    /// The first date of the period covered by this cash flow statement.
    /// </value>
    [JsonPropertyName("from_date")]
    public DateOnly? FromDate { get; init; }

    /// <summary>
    /// Gets the end date of the reporting period.
    /// </summary>
    /// <value>
    /// The last date of the period covered by this cash flow statement.
    /// </value>
    [JsonPropertyName("to_date")]
    public DateOnly? ToDate { get; init; }

    /// <summary>
    /// Gets the opening cash balance at the start of the period.
    /// </summary>
    /// <value>
    /// The total cash position across all bank accounts at the beginning of the reporting period.
    /// This should match the closing balance from the previous period.
    /// </value>
    [JsonPropertyName("opening_balance")]
    public decimal? OpeningBalance { get; init; }

    /// <summary>
    /// Gets the closing cash balance at the end of the period.
    /// </summary>
    /// <value>
    /// The total cash position across all bank accounts at the end of the reporting period.
    /// Calculated as: Opening Balance + Total Cash In - Total Cash Out.
    /// </value>
    [JsonPropertyName("closing_balance")]
    public decimal? ClosingBalance { get; init; }

    /// <summary>
    /// Gets the detailed list of cash inflows during the period.
    /// </summary>
    /// <value>
    /// A list of <see cref="CashFlowItem"/> objects representing individual cash receipts from
    /// customers, loans, investments, and other sources of cash.
    /// </value>
    [JsonPropertyName("cash_in_items")]
    public List<CashFlowItem>? CashInItems { get; init; }

    /// <summary>
    /// Gets the detailed list of cash outflows during the period.
    /// </summary>
    /// <value>
    /// A list of <see cref="CashFlowItem"/> objects representing individual cash payments for
    /// suppliers, expenses, taxes, loan repayments, and other uses of cash.
    /// </value>
    [JsonPropertyName("cash_out_items")]
    public List<CashFlowItem>? CashOutItems { get; init; }

    /// <summary>
    /// Gets the total cash received during the period.
    /// </summary>
    /// <value>
    /// The sum of all cash inflows, representing total money received into the business.
    /// </value>
    [JsonPropertyName("total_cash_in")]
    public decimal? TotalCashIn { get; init; }

    /// <summary>
    /// Gets the total cash paid out during the period.
    /// </summary>
    /// <value>
    /// The sum of all cash outflows, representing total money paid out from the business.
    /// </value>
    [JsonPropertyName("total_cash_out")]
    public decimal? TotalCashOut { get; init; }

    /// <summary>
    /// Gets the net cash flow for the period.
    /// </summary>
    /// <value>
    /// The difference between total cash in and total cash out (Total Cash In - Total Cash Out).
    /// A positive value indicates more cash came in than went out; a negative value indicates
    /// more cash was spent than received.
    /// </value>
    [JsonPropertyName("net_cash_flow")]
    public decimal? NetCashFlow { get; init; }
}