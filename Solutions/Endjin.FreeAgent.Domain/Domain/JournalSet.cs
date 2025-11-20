// <copyright file="JournalSet.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a journal set (a collection of balanced journal entries) in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Journal sets are groups of journal entries that must balance (total debits equal total credits). They are used
/// for manual accounting adjustments, corrections, period-end entries, and other transactions that don't fit into
/// standard invoice, bill, or expense workflows.
/// </para>
/// <para>
/// Each journal set has a date, description, and contains multiple journal entries. Journal sets are particularly
/// useful for:
/// - Accruals and prepayments
/// - Period-end adjustments
/// - Depreciation entries
/// - Director's loan account transactions
/// - Accounting corrections
/// </para>
/// <para>
/// Journal sets can be tagged for integration with external applications, allowing third-party systems to identify
/// and manage their own journal entries. Tagged journal sets will not be editable by users in the app.
/// </para>
/// <para>
/// API Endpoint: /v2/journal_sets
/// </para>
/// <para>
/// Minimum Access Level: Tax, Accounting and Users
/// </para>
/// </remarks>
/// <seealso cref="JournalEntry"/>
/// <seealso cref="Category"/>
public record JournalSet
{
    /// <summary>
    /// Gets the unique URI identifier for this journal set.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this journal set in the FreeAgent system.
    /// This field is read-only and assigned by the API.
    /// </value>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the timestamp when this journal set was last updated.
    /// </summary>
    /// <value>
    /// An ISO 8601 formatted timestamp indicating when the journal set was last modified.
    /// This field is read-only and assigned by the API.
    /// </value>
    [JsonPropertyName("updated_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? UpdatedAt { get; init; }

    /// <summary>
    /// Gets the date this journal set is posted to the accounts.
    /// </summary>
    /// <value>
    /// The posting date that determines which accounting period the journal entries affect.
    /// This field is not applicable for opening balances and can be null in those cases.
    /// </value>
    [JsonPropertyName("dated_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? DatedOn { get; init; }

    /// <summary>
    /// Gets the description of this journal set.
    /// </summary>
    /// <value>
    /// A text description explaining the purpose of this journal set, such as "Month-end accruals" or
    /// "Depreciation for Q1". This field is required.
    /// </value>
    [JsonPropertyName("description")]
    public required string Description { get; init; }

    /// <summary>
    /// Gets the optional tag for external application integration.
    /// </summary>
    /// <value>
    /// A string tag identifier that external applications can use to identify and manage journal sets they create.
    /// Tagged journal sets will not be editable by users in the FreeAgent app.
    /// </value>
    [JsonPropertyName("tag")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Tag { get; init; }

    /// <summary>
    /// Gets the collection of journal entries that make up this journal set.
    /// </summary>
    /// <value>
    /// An immutable list of <see cref="JournalEntry"/> objects. The total debits must equal total credits,
    /// where positive values represent debits and negative values represent credits.
    /// </value>
    [JsonPropertyName("journal_entries")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ImmutableList<JournalEntry> JournalEntries { get; init; } = [];

    /// <summary>
    /// Gets the bank account data for opening balances.
    /// </summary>
    /// <value>
    /// An array of bank account opening balance entries. This field is read-only and only present for opening balance journal sets.
    /// </value>
    [JsonPropertyName("bank_accounts")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ImmutableList<BankAccountOpeningBalance>? BankAccounts { get; init; }

    /// <summary>
    /// Gets the stock item data for opening balances.
    /// </summary>
    /// <value>
    /// An array of stock item opening balance entries. This field is read-only and only present for opening balance journal sets.
    /// </value>
    [JsonPropertyName("stock_items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ImmutableList<StockItemOpeningBalance>? StockItems { get; init; }
}