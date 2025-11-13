// <copyright file="BalanceSheet.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a balance sheet financial report for a company in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// A balance sheet provides a snapshot of a company's financial position at a specific point in time,
/// showing what the company owns (assets), what it owes (liabilities), and the residual equity (capital and reserves).
/// It follows the fundamental accounting equation: Assets = Liabilities + Equity.
/// </para>
/// <para>
/// The balance sheet is structured into main sections:
/// - Fixed Assets: Long-term assets like property, equipment, and intangible assets
/// - Current Assets: Short-term assets like cash, inventory, and receivables
/// - Current Liabilities: Short-term obligations due within one year
/// - Capital and Reserves: Owner's equity and retained earnings
/// </para>
/// <para>
/// Each section contains both summary totals and detailed entries broken down by accounting category.
/// This allows for high-level analysis as well as detailed investigation of specific accounts.
/// </para>
/// <para>
/// API Endpoint: /v2/accounting/balance_sheet
/// </para>
/// <para>
/// Minimum Access Level: Accounting Plus
/// </para>
/// </remarks>
/// <seealso cref="BalanceSheetEntry"/>
/// <seealso cref="ProfitAndLoss"/>
/// <seealso cref="TrialBalance"/>
public record BalanceSheet
{
    /// <summary>
    /// Gets the date for which this balance sheet is prepared.
    /// </summary>
    /// <value>
    /// The balance sheet date, representing the point in time at which the financial position is reported.
    /// </value>
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; init; }

    /// <summary>
    /// Gets the total value of fixed (non-current) assets.
    /// </summary>
    /// <value>
    /// The sum of all long-term assets such as property, equipment, vehicles, and intangible assets.
    /// These are assets expected to provide economic benefit for more than one year.
    /// </value>
    [JsonPropertyName("fixed_assets")]
    public decimal? FixedAssets { get; init; }

    /// <summary>
    /// Gets the total value of current assets.
    /// </summary>
    /// <value>
    /// The sum of all short-term assets such as cash, bank balances, accounts receivable, and inventory.
    /// These are assets expected to be converted to cash or used within one year.
    /// </value>
    [JsonPropertyName("current_assets")]
    public decimal? CurrentAssets { get; init; }

    /// <summary>
    /// Gets the total value of current liabilities.
    /// </summary>
    /// <value>
    /// The sum of all short-term obligations such as accounts payable, accrued expenses, and short-term loans.
    /// These are liabilities due for payment within one year.
    /// </value>
    [JsonPropertyName("current_liabilities")]
    public decimal? CurrentLiabilities { get; init; }

    /// <summary>
    /// Gets the net current assets (working capital).
    /// </summary>
    /// <value>
    /// The difference between current assets and current liabilities (Current Assets - Current Liabilities).
    /// This represents the company's short-term liquidity position and ability to meet immediate obligations.
    /// </value>
    [JsonPropertyName("net_current_assets")]
    public decimal? NetCurrentAssets { get; init; }

    /// <summary>
    /// Gets the total assets less current liabilities.
    /// </summary>
    /// <value>
    /// The sum of fixed assets and net current assets (Fixed Assets + Net Current Assets).
    /// This represents the total long-term investment in the business.
    /// </value>
    [JsonPropertyName("total_assets_less_current_liabilities")]
    public decimal? TotalAssetsLessCurrentLiabilities { get; init; }

    /// <summary>
    /// Gets the total capital and reserves (equity).
    /// </summary>
    /// <value>
    /// The owner's equity in the business, including share capital, retained earnings, and reserves.
    /// This equals Total Assets Less Current Liabilities and represents the net worth of the company.
    /// </value>
    [JsonPropertyName("capital_and_reserves")]
    public decimal? CapitalAndReserves { get; init; }

    /// <summary>
    /// Gets the detailed breakdown of fixed assets by category.
    /// </summary>
    /// <value>
    /// A list of <see cref="BalanceSheetEntry"/> objects showing individual fixed asset categories
    /// with their nominal codes and values.
    /// </value>
    [JsonPropertyName("fixed_asset_entries")]
    public List<BalanceSheetEntry>? FixedAssetEntries { get; init; }

    /// <summary>
    /// Gets the detailed breakdown of current assets by category.
    /// </summary>
    /// <value>
    /// A list of <see cref="BalanceSheetEntry"/> objects showing individual current asset categories
    /// such as bank accounts, debtors, and stock with their values.
    /// </value>
    [JsonPropertyName("current_asset_entries")]
    public List<BalanceSheetEntry>? CurrentAssetEntries { get; init; }

    /// <summary>
    /// Gets the detailed breakdown of current liabilities by category.
    /// </summary>
    /// <value>
    /// A list of <see cref="BalanceSheetEntry"/> objects showing individual current liability categories
    /// such as creditors, VAT, and accrued expenses with their values.
    /// </value>
    [JsonPropertyName("current_liability_entries")]
    public List<BalanceSheetEntry>? CurrentLiabilityEntries { get; init; }

    /// <summary>
    /// Gets the detailed breakdown of capital and reserves by category.
    /// </summary>
    /// <value>
    /// A list of <see cref="BalanceSheetEntry"/> objects showing equity components such as
    /// share capital, retained earnings, and other reserves.
    /// </value>
    [JsonPropertyName("capital_and_reserve_entries")]
    public List<BalanceSheetEntry>? CapitalAndReserveEntries { get; init; }
}