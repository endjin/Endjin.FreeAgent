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
/// and manage their own journal entries.
/// </para>
/// <para>
/// API Endpoint: /v2/journal_sets
/// </para>
/// <para>
/// Minimum Access Level: Accounting Plus
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
    /// </value>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Url { get; init; }

    /// <summary>
    /// Gets the date this journal set is posted to the accounts.
    /// </summary>
    /// <value>
    /// The posting date in YYYY-MM-DD format. This field is required and determines which accounting
    /// period the journal entries affect.
    /// </value>
    [JsonPropertyName("dated_on")]
    public required string DatedOn { get; init; }

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
    /// Useful for preventing duplicate entries and tracking integration-created transactions.
    /// </value>
    [JsonPropertyName("tag")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Tag { get; init; }

    /// <summary>
    /// Gets the collection of journal entries that make up this journal set.
    /// </summary>
    /// <value>
    /// An immutable list of <see cref="JournalEntry"/> objects. The entries must balance (sum to zero)
    /// when considering positive values as debits and negative values as credits.
    /// </value>
    [JsonPropertyName("journal_entries")]
    public ImmutableList<JournalEntry> JournalEntries { get; init; } = [];
}