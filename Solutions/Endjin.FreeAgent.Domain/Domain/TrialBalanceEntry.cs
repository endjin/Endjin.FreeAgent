// <copyright file="TrialBalanceEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a single account entry within a trial balance report in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Trial balance entries show the debit balance, credit balance, and net balance for each accounting
/// category (nominal account) in the chart of accounts. Each entry represents one line in the general ledger.
/// </para>
/// <para>
/// In double-entry bookkeeping:
/// - Asset and expense accounts typically have debit balances
/// - Liability, equity, and income accounts typically have credit balances
/// - The balance shows the net position (debit minus credit)
/// </para>
/// <para>
/// The sum of all debit balances must equal the sum of all credit balances for the books to be balanced.
/// </para>
/// </remarks>
/// <seealso cref="TrialBalance"/>
/// <seealso cref="Category"/>
public record TrialBalanceEntry
{
    /// <summary>
    /// Gets the URI reference to the accounting category for this entry.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Category"/> in the chart of accounts that this trial balance line represents.
    /// </value>
    [JsonPropertyName("category_url")]
    public Uri? CategoryUrl { get; init; }

    /// <summary>
    /// Gets the human-readable description of the accounting category.
    /// </summary>
    /// <value>
    /// The descriptive name of the category, such as "Cash at Bank", "Sales Ledger Control Account",
    /// "Purchase Ledger Control Account", or "Share Capital".
    /// </value>
    [JsonPropertyName("category_description")]
    public string? CategoryDescription { get; init; }

    /// <summary>
    /// Gets the nominal code (account code) for this category.
    /// </summary>
    /// <value>
    /// The numeric or alphanumeric code that identifies this category in the chart of accounts,
    /// following standard accounting numbering conventions.
    /// </value>
    [JsonPropertyName("nominal_code")]
    public string? NominalCode { get; init; }

    /// <summary>
    /// Gets the total debit balance for this account.
    /// </summary>
    /// <value>
    /// The sum of all debit transactions for this account. Typically represents increases in assets
    /// and expenses, or decreases in liabilities and income.
    /// </value>
    [JsonPropertyName("debit")]
    public decimal? Debit { get; init; }

    /// <summary>
    /// Gets the total credit balance for this account.
    /// </summary>
    /// <value>
    /// The sum of all credit transactions for this account. Typically represents decreases in assets
    /// and expenses, or increases in liabilities, equity, and income.
    /// </value>
    [JsonPropertyName("credit")]
    public decimal? Credit { get; init; }

    /// <summary>
    /// Gets the net balance for this account.
    /// </summary>
    /// <value>
    /// The difference between debit and credit balances (Debit - Credit).
    /// A positive balance indicates a net debit balance, while a negative balance indicates a net credit balance.
    /// </value>
    [JsonPropertyName("balance")]
    public decimal? Balance { get; init; }
}