// <copyright file="JournalEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a single journal entry within a journal set in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Journal entries are individual line items in a journal set that record debits and credits to accounting
/// categories. They are used for manual accounting adjustments, corrections, and transactions that don't fit
/// into standard transaction types (invoices, bills, expenses).
/// </para>
/// <para>
/// Each entry specifies a category (chart of accounts line), a debit value (positive for debits, negative for credits),
/// and a description. Journal entries must balance within their parent journal set (total debits equal total credits).
/// </para>
/// <para>
/// Journal entries can optionally be associated with a specific user for Director's Loan Account tracking.
/// </para>
/// <para>
/// API Endpoint: /v2/journal_entries (accessed via journal set relationship)
/// </para>
/// <para>
/// Minimum Access Level: Accounting Plus
/// </para>
/// </remarks>
/// <seealso cref="JournalSet"/>
/// <seealso cref="Category"/>
[DebuggerDisplay("{Category} {DisplayText} - {DebitValue}")]
public record JournalEntry
{
    /// <summary>
    /// Gets the URI reference to the accounting category for this journal entry.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Category"/> in the chart of accounts that this entry affects.
    /// This field is required.
    /// </value>
    [JsonPropertyName("category")]
    public required string Category { get; init; }

    /// <summary>
    /// Gets the debit value for this journal entry.
    /// </summary>
    /// <value>
    /// The monetary amount as a string. Positive values represent debits, negative values represent credits.
    /// This field is required and must be formatted as a decimal string (e.g., "123.45" or "-123.45").
    /// </value>
    [JsonPropertyName("debit_value")]
    public required string DebitValue { get; init; }

    /// <summary>
    /// Gets the description explaining this journal entry.
    /// </summary>
    /// <value>
    /// A text description of the journal entry explaining the purpose of the adjustment or transaction.
    /// This field is required.
    /// </value>
    [JsonPropertyName("description")]
    public required string Description { get; init; }

    /// <summary>
    /// Gets the unique URI identifier for this journal entry.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this journal entry in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Url { get; init; }

    /// <summary>
    /// Gets the URI reference to the user associated with this journal entry.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.User"/> for Director's Loan Account tracking. Optional, used when
    /// the journal entry represents a transaction with a director or shareholder.
    /// </value>
    [JsonPropertyName("user")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? User { get; init; }

    /// <summary>
    /// Gets the display text for this journal entry.
    /// </summary>
    /// <value>
    /// Optional display text used for UI presentation. This property is not serialized to JSON.
    /// </value>
    [JsonIgnore]
    public string? DisplayText { get; init; }
}