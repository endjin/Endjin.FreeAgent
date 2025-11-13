// <copyright file="TrialBalance.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a trial balance report for a company in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// A trial balance is a fundamental accounting report that lists all general ledger accounts with their
/// debit and credit balances at a specific point in time. It serves as a check that the double-entry
/// bookkeeping system is balanced (total debits equal total credits) and forms the basis for preparing
/// financial statements.
/// </para>
/// <para>
/// The trial balance includes:
/// - All accounting categories (nominal accounts) from the chart of accounts
/// - Debit balances for asset and expense accounts
/// - Credit balances for liability, equity, and income accounts
/// - Total debit and total credit amounts which must be equal
/// </para>
/// <para>
/// This report is essential for accountants and bookkeepers to verify the accuracy of accounting records
/// before preparing the balance sheet and profit and loss statement.
/// </para>
/// <para>
/// API Endpoint: /v2/accounting/trial_balance
/// </para>
/// <para>
/// Minimum Access Level: Accounting Plus
/// </para>
/// </remarks>
/// <seealso cref="TrialBalanceEntry"/>
/// <seealso cref="BalanceSheet"/>
/// <seealso cref="Category"/>
public record TrialBalance
{
    /// <summary>
    /// Gets the date for which this trial balance is prepared.
    /// </summary>
    /// <value>
    /// The trial balance date, representing the point in time at which account balances are reported.
    /// </value>
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; init; }

    /// <summary>
    /// Gets the list of all trial balance entries (account balances).
    /// </summary>
    /// <value>
    /// A list of <see cref="TrialBalanceEntry"/> objects representing each account in the chart of accounts
    /// with its debit balance, credit balance, and net balance.
    /// </value>
    [JsonPropertyName("entries")]
    public List<TrialBalanceEntry>? Entries { get; init; }

    /// <summary>
    /// Gets the total of all debit balances.
    /// </summary>
    /// <value>
    /// The sum of all debit balances across all accounts. This should equal the total credit balance
    /// if the books are balanced correctly.
    /// </value>
    [JsonPropertyName("total_debit")]
    public decimal? TotalDebit { get; init; }

    /// <summary>
    /// Gets the total of all credit balances.
    /// </summary>
    /// <value>
    /// The sum of all credit balances across all accounts. This should equal the total debit balance
    /// if the books are balanced correctly, confirming the fundamental accounting equation.
    /// </value>
    [JsonPropertyName("total_credit")]
    public decimal? TotalCredit { get; init; }
}