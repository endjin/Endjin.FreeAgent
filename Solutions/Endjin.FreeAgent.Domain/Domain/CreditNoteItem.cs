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
/// Minimum Access Level: Estimates and Invoices
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
    /// A decimal value starting at 1 that determines the order in which items appear on the credit note.
    /// Lower numbers appear first.
    /// </value>
    [JsonPropertyName("position")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Position { get; init; }

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
    /// Gets the calculated sales tax amount for this item.
    /// </summary>
    /// <value>
    /// The total tax amount calculated by applying <see cref="SalesTaxRate"/> to the <see cref="Subtotal"/>.
    /// </value>
    [JsonPropertyName("sales_tax_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SalesTaxValue { get; init; }

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

    /// <summary>
    /// Gets the type of the item.
    /// </summary>
    /// <value>
    /// The unit of measurement or type classification for this line item. Valid values include:
    /// "Hours", "Days", "Weeks", "Months", "Years", "Products", "Services", "Training",
    /// "Expenses", "Comment", "Bills", "Discount", "Credit", "VAT", or blank/null for default.
    /// </value>
    [JsonPropertyName("item_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ItemType { get; init; }

    /// <summary>
    /// Gets the second sales tax rate (for jurisdictions with multiple tax rates).
    /// </summary>
    /// <value>
    /// The secondary tax rate as a decimal (e.g., 0.05 for 5% tax).
    /// </value>
    [JsonPropertyName("second_sales_tax_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SecondSalesTaxRate { get; init; }

    /// <summary>
    /// Gets the sales tax status for this item.
    /// </summary>
    /// <value>
    /// The tax status classification for primary tax. Valid values are "TAXABLE", "EXEMPT", or "OUT_OF_SCOPE".
    /// </value>
    [JsonPropertyName("sales_tax_status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SalesTaxStatus { get; init; }

    /// <summary>
    /// Gets the second sales tax status (for jurisdictions with multiple tax types).
    /// </summary>
    /// <value>
    /// The tax status classification for secondary tax.
    /// </value>
    [JsonPropertyName("second_sales_tax_status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SecondSalesTaxStatus { get; init; }

    /// <summary>
    /// Gets a value indicating whether this item suffers CIS deduction.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if CIS (Construction Industry Scheme) deduction applies to this item;
    /// otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("suffers_cis_deduction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? SuffersCisDeduction { get; init; }

    /// <summary>
    /// Gets the URI reference to the stock item.
    /// </summary>
    /// <value>
    /// The URI of the stock item being credited.
    /// </value>
    [JsonPropertyName("stock_item")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? StockItem { get; init; }

    /// <summary>
    /// Gets the URI reference to the project.
    /// </summary>
    /// <value>
    /// The URI of the project associated with this line item.
    /// </value>
    [JsonPropertyName("project")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Project { get; init; }

    /// <summary>
    /// Gets the subtotal amount for this item before tax.
    /// </summary>
    /// <value>
    /// The pre-tax total calculated as <see cref="Quantity"/> × <see cref="Price"/>.
    /// </value>
    [JsonPropertyName("subtotal")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Subtotal { get; init; }

    /// <summary>
    /// Gets the total amount for this item including all taxes.
    /// </summary>
    /// <value>
    /// The final line total calculated as <see cref="Subtotal"/> + <see cref="SalesTaxValue"/>
    /// (and any additional taxes).
    /// </value>
    [JsonPropertyName("total")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Total { get; init; }

    /// <summary>
    /// Gets the unique identifier for this credit note item.
    /// </summary>
    /// <value>
    /// The integer ID used for updating or deleting specific line items.
    /// </value>
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Id { get; init; }

    /// <summary>
    /// Gets the deletion flag for this item.
    /// </summary>
    /// <value>
    /// Set to 1 to mark this item for deletion when updating a credit note.
    /// </value>
    [JsonPropertyName("_destroy")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Destroy { get; init; }
}