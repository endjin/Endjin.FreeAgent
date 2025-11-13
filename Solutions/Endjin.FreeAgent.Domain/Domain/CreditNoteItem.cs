// <copyright file="CreditNoteItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a line item within a credit note in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Credit note items detail individual products, services, or charges being credited back to a customer.
/// Each item has its own description, quantity, price, and tax treatment. Credit note items work similarly
/// to invoice items but represent reversals or reductions in revenue.
/// </para>
/// <para>
/// Items can be linked to accounting categories for proper classification in financial reports. The credit
/// amount is calculated from the quantity and price, with applicable tax adjustments.
/// </para>
/// <para>
/// API Endpoint: /v2/credit_note_items (accessed via credit note relationship)
/// </para>
/// <para>
/// Minimum Access Level: Invoices
/// </para>
/// </remarks>
/// <seealso cref="CreditNote"/>
/// <seealso cref="Category"/>
public record CreditNoteItem
{
    /// <summary>
    /// Gets the unique URI identifier for this credit note item.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this line item in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the display order position of this item within the credit note.
    /// </summary>
    /// <value>
    /// A zero-based index determining the order in which items appear on the credit note.
    /// Lower numbers appear first.
    /// </value>
    [JsonPropertyName("position")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Position { get; init; }

    /// <summary>
    /// Gets the description of the product or service being credited.
    /// </summary>
    /// <value>
    /// The text description that appears on the credit note line item, explaining what is being credited.
    /// </value>
    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the quantity of units being credited.
    /// </summary>
    /// <value>
    /// The number of units being credited. Multiplied by <see cref="Price"/> to calculate line credit totals.
    /// </value>
    [JsonPropertyName("quantity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Quantity { get; init; }

    /// <summary>
    /// Gets the unit price for this credit item.
    /// </summary>
    /// <value>
    /// The credit price per unit, excluding tax. Multiplied by <see cref="Quantity"/> to calculate
    /// the total credit amount.
    /// </value>
    [JsonPropertyName("price")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Price { get; init; }

    /// <summary>
    /// Gets the sales tax rate applied to this credit item.
    /// </summary>
    /// <value>
    /// The VAT/GST rate as a decimal (e.g., 0.20 for 20% tax). Used to calculate the tax component
    /// of the credit.
    /// </value>
    [JsonPropertyName("sales_tax_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SalesTaxRate { get; init; }

    /// <summary>
    /// Gets the URI reference to the accounting category for this item.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Category"/> used to classify this credit in the chart of accounts.
    /// Determines how the revenue reversal is reported in financial statements.
    /// </value>
    [JsonPropertyName("category")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Category { get; init; }
}