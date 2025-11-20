// <copyright file="CashFlow.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a cash flow report for a company in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// A cash flow report provides a high-level summary of incoming and outgoing cash over a
/// specific period, broken down by month. This is a simplified view that shows aggregated
/// totals rather than individual transactions.
/// </para>
/// <para>
/// The report includes:
/// - From/To dates defining the reporting period
/// - Balance: Net cash flow (incoming total minus outgoing total)
/// - Incoming: Revenue aggregated by month
/// - Outgoing: Expenses aggregated by month
/// </para>
/// <para>
/// This report helps understand cash flow trends over time and provides visibility into
/// monthly revenue and expense patterns. For future-dated requests, totals return as 0
/// (projected cash flow is not available).
/// </para>
/// <para>
/// API Endpoint: /v2/cashflow
/// </para>
/// <para>
/// Minimum Access Level: Banking (read-only)
/// </para>
/// </remarks>
/// <seealso cref="CashFlowDirection"/>
/// <seealso cref="CashFlowMonthly"/>
public record CashFlow
{
    /// <summary>
    /// Gets the start date of the reporting period.
    /// </summary>
    /// <value>
    /// The first date of the period covered by this cash flow report, in YYYY-MM-DD format.
    /// </value>
    [JsonPropertyName("from")]
    public string? From { get; init; }

    /// <summary>
    /// Gets the end date of the reporting period.
    /// </summary>
    /// <value>
    /// The last date of the period covered by this cash flow report, in YYYY-MM-DD format.
    /// </value>
    [JsonPropertyName("to")]
    public string? To { get; init; }

    /// <summary>
    /// Gets the net cash flow balance for the period.
    /// </summary>
    /// <value>
    /// The difference between incoming and outgoing cash (Incoming Total - Outgoing Total).
    /// A positive value indicates more cash came in than went out; a negative value indicates
    /// more cash was spent than received.
    /// </value>
    [JsonPropertyName("balance")]
    public decimal? Balance { get; init; }

    /// <summary>
    /// Gets the incoming cash flow data (revenue).
    /// </summary>
    /// <value>
    /// A <see cref="CashFlowDirection"/> object containing the total incoming cash and
    /// monthly breakdown of revenue across the reporting period.
    /// </value>
    [JsonPropertyName("incoming")]
    public CashFlowDirection? Incoming { get; init; }

    /// <summary>
    /// Gets the outgoing cash flow data (expenses).
    /// </summary>
    /// <value>
    /// A <see cref="CashFlowDirection"/> object containing the total outgoing cash and
    /// monthly breakdown of expenses across the reporting period.
    /// </value>
    [JsonPropertyName("outgoing")]
    public CashFlowDirection? Outgoing { get; init; }
}
