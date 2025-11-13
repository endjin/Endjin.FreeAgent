// <copyright file="OpeningBalanceJournal.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents an opening balance journal entry for migrating existing account balances into FreeAgent.
/// </summary>
/// <remarks>
/// <para>
/// An opening balance journal is used when setting up FreeAgent mid-year or migrating from another accounting
/// system. It captures the existing balances from all accounts as of a specific date (typically the start of the
/// accounting year or the FreeAgent go-live date).
/// </para>
/// <para>
/// The journal follows double-entry bookkeeping principles:
/// <list type="bullet">
/// <item>All debit entries are listed in <see cref="DebitEntries"/></item>
/// <item>All credit entries are listed in <see cref="CreditEntries"/></item>
/// <item>Total debits must equal total credits for the journal to balance</item>
/// </list>
/// </para>
/// <para>
/// Common opening balance scenarios:
/// <list type="bullet">
/// <item>Starting FreeAgent mid-year with year-to-date transactions</item>
/// <item>Migrating from another accounting system</item>
/// <item>Bringing forward prior year balances at the start of a new accounting period</item>
/// </list>
/// </para>
/// <para>
/// The opening balance journal creates the foundation for accurate financial reporting in FreeAgent
/// by ensuring historical balances are correctly reflected. Once entered, it should not be modified
/// as it represents a point-in-time snapshot of the business's financial position.
/// </para>
/// </remarks>
/// <seealso cref="OpeningBalanceEntry"/>
/// <seealso cref="OpeningBalance"/>
/// <seealso cref="JournalSet"/>
public record OpeningBalanceJournal
{
    /// <summary>
    /// Gets the effective date for this opening balance journal.
    /// </summary>
    /// <value>
    /// The date on which these balances apply, typically the first day of the accounting period
    /// or the FreeAgent go-live date. All subsequent transactions will be dated after this date.
    /// </value>
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; init; }

    /// <summary>
    /// Gets a description for this opening balance journal.
    /// </summary>
    /// <value>
    /// Optional text describing the journal, such as "Opening balances 2024/25" or
    /// "Migration from Xero". Helps identify the purpose and source of the balances.
    /// </value>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the list of debit entries in this opening balance journal.
    /// </summary>
    /// <value>
    /// A collection of <see cref="OpeningBalanceEntry"/> objects representing debit balances
    /// (assets, expenses). The sum of all debit entry amounts must equal the sum of all credit
    /// entry amounts for the journal to balance.
    /// </value>
    [JsonPropertyName("debit_entries")]
    public List<OpeningBalanceEntry>? DebitEntries { get; init; }

    /// <summary>
    /// Gets the list of credit entries in this opening balance journal.
    /// </summary>
    /// <value>
    /// A collection of <see cref="OpeningBalanceEntry"/> objects representing credit balances
    /// (liabilities, equity, income). The sum of all credit entry amounts must equal the sum
    /// of all debit entry amounts for the journal to balance.
    /// </value>
    [JsonPropertyName("credit_entries")]
    public List<OpeningBalanceEntry>? CreditEntries { get; init; }
}