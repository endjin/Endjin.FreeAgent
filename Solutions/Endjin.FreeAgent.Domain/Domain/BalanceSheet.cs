// <copyright file="BalanceSheet.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a balance sheet financial report for a company in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// A balance sheet provides a snapshot of a company's financial position at a specific point in time,
/// showing what the company owns (assets), what it owes (liabilities), and the residual equity.
/// It follows the fundamental accounting equation: Assets = Liabilities + Equity.
/// </para>
/// <para>
/// The balance sheet is structured into main sections:
/// - Capital Assets: Long-term assets like property, equipment (with net book value after depreciation)
/// - Current Assets: Short-term assets like cash, inventory, and receivables
/// - Current Liabilities: Short-term obligations due within one year
/// - Owners' Equity: Owner's capital and retained earnings
/// </para>
/// <para>
/// All monetary values are returned as integers rounded to the nearest whole number.
/// The total owners' equity should always equal the inverse of total assets in a balanced sheet.
/// </para>
/// <para>
/// API Endpoint: GET https://api.freeagent.com/v2/accounting/balance_sheet
/// </para>
/// <para>
/// API Endpoint (Opening Balances): GET https://api.freeagent.com/v2/accounting/balance_sheet/opening_balances
/// </para>
/// <para>
/// Minimum Access Level: Tax, Accounting &amp; Users
/// </para>
/// <para>
/// API Documentation: https://dev.freeagent.com/docs/balance_sheet
/// </para>
/// </remarks>
/// <seealso cref="BalanceSheetAccount"/>
/// <seealso cref="CapitalAssetsSection"/>
/// <seealso cref="AssetsSection"/>
/// <seealso cref="LiabilitiesSection"/>
/// <seealso cref="OwnersEquitySection"/>
/// <seealso cref="ProfitAndLoss"/>
/// <seealso cref="TrialBalanceSummaryEntry"/>
public record BalanceSheet
{
    /// <summary>
    /// Gets the start date of the accounting period.
    /// </summary>
    /// <value>
    /// The date when the current accounting period began.
    /// </value>
    /// <remarks>
    /// This field is not present in opening balances responses.
    /// </remarks>
    [JsonPropertyName("accounting_period_start_date")]
    public DateOnly? AccountingPeriodStartDate { get; init; }

    /// <summary>
    /// Gets the date through which balance sheet values are calculated.
    /// </summary>
    /// <value>
    /// The "as at" date representing the point in time at which the financial position is reported.
    /// </value>
    /// <remarks>
    /// This field is not present in opening balances responses.
    /// </remarks>
    [JsonPropertyName("as_at_date")]
    public DateOnly? AsAtDate { get; init; }

    /// <summary>
    /// Gets the ISO currency code of the company's native currency.
    /// </summary>
    /// <value>
    /// Three-letter ISO 4217 currency code (e.g., "GBP", "USD", "EUR").
    /// </value>
    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    /// <summary>
    /// Gets the capital assets (fixed assets) section of the balance sheet.
    /// </summary>
    /// <value>
    /// An object containing the detailed accounts and net book value for capital assets.
    /// </value>
    /// <remarks>
    /// Capital assets are long-term assets used in business operations.
    /// The net book value represents original cost minus accumulated depreciation.
    /// </remarks>
    [JsonPropertyName("capital_assets")]
    public CapitalAssetsSection? CapitalAssets { get; init; }

    /// <summary>
    /// Gets the current assets section of the balance sheet.
    /// </summary>
    /// <value>
    /// An object containing the detailed accounts for current assets.
    /// </value>
    /// <remarks>
    /// Current assets are short-term assets expected to be converted to cash
    /// or used within one year.
    /// </remarks>
    [JsonPropertyName("current_assets")]
    public AssetsSection? CurrentAssets { get; init; }

    /// <summary>
    /// Gets the current liabilities section of the balance sheet.
    /// </summary>
    /// <value>
    /// An object containing the detailed accounts for current liabilities.
    /// </value>
    /// <remarks>
    /// Current liabilities are short-term obligations due within one year.
    /// Negative values indicate amounts owed by the business.
    /// </remarks>
    [JsonPropertyName("current_liabilities")]
    public LiabilitiesSection? CurrentLiabilities { get; init; }

    /// <summary>
    /// Gets the net current assets (working capital).
    /// </summary>
    /// <value>
    /// The combined total of current assets and current liabilities, rounded to the nearest integer.
    /// </value>
    /// <remarks>
    /// This represents the company's short-term liquidity position and ability to meet immediate obligations.
    /// Calculated as: Current Assets + Current Liabilities (where liabilities are typically negative).
    /// </remarks>
    [JsonPropertyName("net_current_assets")]
    public int? NetCurrentAssets { get; init; }

    /// <summary>
    /// Gets the sum of all asset categories.
    /// </summary>
    /// <value>
    /// The total of all assets (capital assets + current assets), rounded to the nearest integer.
    /// </value>
    [JsonPropertyName("total_assets")]
    public int? TotalAssets { get; init; }

    /// <summary>
    /// Gets the owners' equity section of the balance sheet.
    /// </summary>
    /// <value>
    /// An object containing the detailed equity accounts and retained profit.
    /// </value>
    /// <remarks>
    /// Owners' equity represents the residual interest in the assets after deducting liabilities.
    /// According to the accounting equation: Assets = Liabilities + Equity
    /// </remarks>
    [JsonPropertyName("owners_equity")]
    public OwnersEquitySection? OwnersEquity { get; init; }

    /// <summary>
    /// Gets the total owners' equity.
    /// </summary>
    /// <value>
    /// The total equity in the business, rounded to the nearest integer.
    /// </value>
    /// <remarks>
    /// This should always be the inverse of total assets in a balanced sheet.
    /// </remarks>
    [JsonPropertyName("total_owners_equity")]
    public int? TotalOwnersEquity { get; init; }
}
