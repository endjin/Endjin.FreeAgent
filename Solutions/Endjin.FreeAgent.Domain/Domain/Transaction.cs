// <copyright file="Transaction.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents an accounting transaction in the FreeAgent system.
/// </summary>
/// <remarks>
/// <para>
/// Transactions represent individual accounting entries in the general ledger. Each transaction
/// records a debit or credit to a specific nominal account (category). Transactions are generated
/// automatically when invoices, bills, expenses, and other financial documents are processed.
/// </para>
/// <para>
/// The <see cref="DebitValue"/> property contains the transaction amount where positive values
/// represent debits and negative values represent credits. For transactions in foreign currencies,
/// the <see cref="ForeignCurrencyData"/> property contains the original currency details.
/// </para>
/// <para>
/// This is a read-only resource. Transactions cannot be created, updated, or deleted directly
/// through the API. They are created automatically by FreeAgent when financial documents are processed.
/// </para>
/// <para>
/// API Endpoint: /v2/accounting/transactions
/// </para>
/// <para>
/// Minimum Access Level: Tax, Accounting &amp; Users
/// </para>
/// </remarks>
/// <seealso cref="Category"/>
/// <seealso cref="ForeignCurrencyData"/>
[DebuggerDisplay("DatedOn = {" + nameof(DatedOn) + "}, DebitValue = {" + nameof(DebitValue) + "}")]
public record Transaction
{
    /// <summary>
    /// Gets the unique URI identifier for this transaction.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this transaction in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the date of the transaction.
    /// </summary>
    /// <value>
    /// The date when this transaction was recorded in the accounting system.
    /// </value>
    [JsonPropertyName("dated_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? DatedOn { get; init; }

    /// <summary>
    /// Gets the date and time when this transaction was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> in UTC representing when this transaction was first created in the system.
    /// </value>
    [JsonPropertyName("created_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this transaction was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> in UTC representing the last time this transaction record was modified.
    /// </value>
    [JsonPropertyName("updated_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? UpdatedAt { get; init; }

    /// <summary>
    /// Gets the description or source reference for this transaction.
    /// </summary>
    /// <value>
    /// A text description identifying the source or nature of the transaction (e.g., invoice number, bill reference).
    /// </value>
    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the URI reference to the accounting category for this transaction.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Category"/> (nominal account) to which this transaction is posted.
    /// </value>
    [JsonPropertyName("category")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Category { get; init; }

    /// <summary>
    /// Gets the display name of the accounting category.
    /// </summary>
    /// <value>
    /// The human-readable name of the nominal account category.
    /// </value>
    [JsonPropertyName("category_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CategoryName { get; init; }

    /// <summary>
    /// Gets the nominal code of the accounting category.
    /// </summary>
    /// <value>
    /// The account code (e.g., "001", "200") identifying the nominal account in the chart of accounts.
    /// </value>
    [JsonPropertyName("nominal_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? NominalCode { get; init; }

    /// <summary>
    /// Gets the debit value of the transaction.
    /// </summary>
    /// <value>
    /// The transaction amount in the company's native currency. Positive values represent debits;
    /// negative values represent credits.
    /// </value>
    [JsonPropertyName("debit_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? DebitValue { get; init; }

    /// <summary>
    /// Gets the URL of the source item that generated this transaction.
    /// </summary>
    /// <value>
    /// A reference to the originating document (e.g., bill, invoice, expense, bank transaction explanation)
    /// that created this transaction.
    /// </value>
    [JsonPropertyName("source_item_url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SourceItemUrl { get; init; }

    /// <summary>
    /// Gets the foreign currency data for transactions in non-native currencies.
    /// </summary>
    /// <value>
    /// Contains the currency code and amount in the original foreign currency when the transaction
    /// is not in the company's native currency; otherwise <see langword="null"/>.
    /// </value>
    [JsonPropertyName("foreign_currency_data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ForeignCurrencyData? ForeignCurrencyData { get; init; }
}
