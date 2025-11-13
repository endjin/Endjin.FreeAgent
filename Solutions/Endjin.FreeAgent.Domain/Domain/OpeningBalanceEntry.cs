// <copyright file="OpeningBalanceEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a single line item entry in an opening balance journal in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Opening balance entries are individual debit or credit entries that make up an <see cref="OpeningBalanceJournal"/>.
/// Each entry represents a balance in a specific accounting category (nominal code) from a previous accounting system
/// or period that needs to be brought forward into FreeAgent.
/// </para>
/// <para>
/// When migrating to FreeAgent or starting mid-year, businesses need to enter their existing account balances
/// as of the start date. These entries follow double-entry bookkeeping principles where total debits must equal
/// total credits within the journal.
/// </para>
/// <para>
/// Common examples:
/// <list type="bullet">
/// <item>Debit entries: Bank account balances, accounts receivable, fixed assets, expenses year-to-date</item>
/// <item>Credit entries: Accounts payable, loans, retained earnings, income year-to-date</item>
/// </list>
/// </para>
/// </remarks>
/// <seealso cref="OpeningBalanceJournal"/>
/// <seealso cref="OpeningBalance"/>
/// <seealso cref="Category"/>
public record OpeningBalanceEntry
{
    /// <summary>
    /// Gets the API URL of the accounting category (nominal code) for this entry.
    /// </summary>
    /// <value>
    /// A reference to the <see cref="Category"/> resource representing the nominal code account
    /// where this balance should be recorded (e.g., Bank Account, Sales, Retained Earnings).
    /// </value>
    [JsonPropertyName("category")]
    public Uri? Category { get; init; }

    /// <summary>
    /// Gets the monetary amount for this entry.
    /// </summary>
    /// <value>
    /// The amount for this opening balance entry. Positive values represent debits (for debit entries)
    /// or credits (for credit entries) depending on which list this entry appears in within the journal.
    /// </value>
    [JsonPropertyName("amount")]
    public decimal? Amount { get; init; }

    /// <summary>
    /// Gets a description explaining this opening balance entry.
    /// </summary>
    /// <value>
    /// Optional text describing the entry, such as "Opening bank balance" or "Retained earnings b/f".
    /// Helps identify the source and nature of the balance for future reference.
    /// </value>
    [JsonPropertyName("description")]
    public string? Description { get; init; }
}