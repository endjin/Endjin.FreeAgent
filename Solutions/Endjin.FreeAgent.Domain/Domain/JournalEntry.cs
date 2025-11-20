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
/// and an optional description. Journal entries must balance within their parent journal set (total debits equal total credits).
/// </para>
/// <para>
/// Journal entries can optionally be associated with a specific user for Director's Loan Account tracking,
/// or with capital assets, stock items, bank accounts, or properties depending on the category type.
/// </para>
/// <para>
/// Journal entries are nested within journal sets and accessed via the journal_set API endpoints.
/// </para>
/// <para>
/// Minimum Access Level: Tax, Accounting and Users
/// </para>
/// </remarks>
/// <seealso cref="JournalSet"/>
/// <seealso cref="Category"/>
[DebuggerDisplay("{Category} {Description} - {DebitValue}")]
public record JournalEntry
{
    /// <summary>
    /// Gets the unique URI identifier for this journal entry.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this journal entry in the FreeAgent system.
    /// This field is read-only and assigned by the API.
    /// </value>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the URI reference to the accounting category for this journal entry.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Category"/> in the chart of accounts that this entry affects.
    /// This field is required.
    /// </value>
    [JsonPropertyName("category")]
    public required Uri Category { get; init; }

    /// <summary>
    /// Gets the debit value for this journal entry.
    /// </summary>
    /// <value>
    /// The monetary amount. Positive values represent debits, negative values represent credits.
    /// This field is required.
    /// </value>
    [JsonPropertyName("debit_value")]
    public required decimal DebitValue { get; init; }

    /// <summary>
    /// Gets the description explaining this journal entry.
    /// </summary>
    /// <value>
    /// A text description of the journal entry explaining the purpose of the adjustment or transaction.
    /// This field is optional.
    /// </value>
    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the URI reference to the capital asset type for this journal entry.
    /// </summary>
    /// <value>
    /// Required for category codes 601-607 (capital asset categories). The URI references
    /// the capital asset type that this entry relates to.
    /// </value>
    [JsonPropertyName("capital_asset_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? CapitalAssetType { get; init; }

    /// <summary>
    /// Gets the URI reference to the user associated with this journal entry.
    /// </summary>
    /// <value>
    /// Required for user categories (e.g., Director's Loan Account). The URI references
    /// the <see cref="Domain.User"/> that this entry relates to.
    /// </value>
    [JsonPropertyName("user")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? User { get; init; }

    /// <summary>
    /// Gets the URI reference to the stock item for this journal entry.
    /// </summary>
    /// <value>
    /// Required for stock categories. The URI references the stock item that this entry relates to.
    /// </value>
    [JsonPropertyName("stock_item")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? StockItem { get; init; }

    /// <summary>
    /// Gets the quantity change for stock altering entries.
    /// </summary>
    /// <value>
    /// The quantity by which to adjust the stock item. Positive values increase stock,
    /// negative values decrease stock.
    /// </value>
    [JsonPropertyName("stock_altering_quantity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? StockAlteringQuantity { get; init; }

    /// <summary>
    /// Gets the URI reference to the bank account for this journal entry.
    /// </summary>
    /// <value>
    /// Historical data only. The URI references the bank account that this entry relates to.
    /// </value>
    [JsonPropertyName("bank_account")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? BankAccount { get; init; }

    /// <summary>
    /// Gets the URI reference to the property for this journal entry.
    /// </summary>
    /// <value>
    /// For UK Unincorporated Landlord P&amp;L categories. The URI references the property
    /// that this entry relates to.
    /// </value>
    [JsonPropertyName("property")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Property { get; init; }

    /// <summary>
    /// Gets a value indicating whether this entry should be deleted during an update operation.
    /// </summary>
    /// <value>
    /// Set to <c>true</c> to remove this entry when updating a journal set. Only used in PUT requests.
    /// </value>
    [JsonPropertyName("_destroy")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Destroy { get; init; }
}