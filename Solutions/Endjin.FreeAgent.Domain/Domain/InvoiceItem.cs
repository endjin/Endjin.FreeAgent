// <copyright file="InvoiceItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a line item within an invoice in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Invoice items are individual products, services, or charges that make up an invoice. Each item has
/// its own description, quantity, price, and tax treatment. Invoice items support multiple item types
/// including products, services, time tracking entries, expenses, and comments.
/// </para>
/// <para>
/// Items can be linked to projects and accounting categories for proper classification. Tax calculations
/// support various VAT/GST scenarios including exempt, zero-rated, and standard-rated goods and services.
/// Some jurisdictions support dual tax rates (e.g., Canadian GST/PST).
/// </para>
/// <para>
/// Invoice items can reference stock items from inventory management and maintain ordering through
/// the Position property.
/// </para>
/// <para>
/// API Endpoint: /v2/invoice_items (accessed via invoice relationship)
/// </para>
/// <para>
/// Minimum Access Level: Invoices
/// </para>
/// </remarks>
/// <seealso cref="Invoice"/>
/// <seealso cref="Category"/>
/// <seealso cref="Project"/>
public record InvoiceItem
{
    /// <summary>
    /// Gets the unique identifier for this invoice item.
    /// </summary>
    /// <value>
    /// The unique ID for this line item. Required when updating or deleting individual invoice items.
    /// </value>
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Id { get; init; }

    /// <summary>
    /// Gets the unique URI identifier for this invoice item.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this line item in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the display order position of this item within the invoice.
    /// </summary>
    /// <value>
    /// A decimal value starting at 1 that determines the order in which items appear on the invoice.
    /// Lower numbers appear first.
    /// </value>
    [JsonPropertyName("position")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Position { get; init; }

    /// <summary>
    /// Gets the description of the product or service.
    /// </summary>
    /// <value>
    /// The text description that appears on the invoice line item, explaining what is being charged.
    /// </value>
    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the type of this invoice item.
    /// </summary>
    /// <value>
    /// One of "Hours", "Days", "Weeks", "Months", "Years", "Products", "Services", "Training",
    /// "Expenses", "Comment", "Bills", "Discount", "Credit", "VAT", or "Stock".
    /// Determines how the item is classified and processed.
    /// </value>
    [JsonPropertyName("item_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ItemType { get; init; }

    /// <summary>
    /// Gets the quantity of units for this item.
    /// </summary>
    /// <value>
    /// The number of units being invoiced. Multiplied by <see cref="Price"/> to calculate line totals.
    /// Can represent hours for time-based items or units for products.
    /// </value>
    [JsonPropertyName("quantity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Quantity { get; init; }

    /// <summary>
    /// Gets the unit price for this item.
    /// </summary>
    /// <value>
    /// The price per unit, excluding tax. Multiplied by <see cref="Quantity"/> to calculate
    /// the line item total.
    /// </value>
    [JsonPropertyName("price")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Price { get; init; }

    /// <summary>
    /// Gets the sales tax rate applied to this item.
    /// </summary>
    /// <value>
    /// The primary VAT/GST rate as a percentage value (e.g., 20 for 20% tax). Used to calculate
    /// <see cref="SalesTaxValue"/>.
    /// </value>
    [JsonPropertyName("sales_tax_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SalesTaxRate { get; init; }

    /// <summary>
    /// Gets the total value of sales tax for this item.
    /// </summary>
    /// <value>
    /// The total sales tax amount calculated by applying <see cref="SalesTaxRate"/> to the line item.
    /// This is a calculated field returned by the API but not listed in the official API documentation.
    /// </value>
    [JsonPropertyName("sales_tax_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SalesTaxValue { get; init; }

    /// <summary>
    /// Gets the sales tax status classification for this item.
    /// </summary>
    /// <value>
    /// Either "TAXABLE" or "EXEMPT", determining how primary sales tax is applied to this item.
    /// </value>
    [JsonPropertyName("sales_tax_status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SalesTaxStatus { get; init; }

    /// <summary>
    /// Gets the secondary sales tax rate for jurisdictions with dual tax systems.
    /// </summary>
    /// <value>
    /// An additional tax rate as a percentage value (e.g., 7 for 7% PST in Canadian provinces with GST/PST)
    /// applied on top of the primary <see cref="SalesTaxRate"/>.
    /// </value>
    [JsonPropertyName("second_sales_tax_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SecondSalesTaxRate { get; init; }

    /// <summary>
    /// Gets the secondary sales tax status classification for this item.
    /// </summary>
    /// <value>
    /// Either "TAXABLE" or "EXEMPT", determining how secondary sales tax is applied to this item.
    /// Used in jurisdictions with dual tax systems (e.g., Canadian GST/PST).
    /// </value>
    [JsonPropertyName("second_sales_tax_status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SecondSalesTaxStatus { get; init; }

    /// <summary>
    /// Gets the total value of second sales tax for this item.
    /// </summary>
    /// <value>
    /// [Universal accounts only] The total second sales tax amount calculated by applying
    /// <see cref="SecondSalesTaxRate"/> to the line item.
    /// This is a calculated field returned by the API but not listed in the official API documentation.
    /// </value>
    [JsonPropertyName("second_sales_tax_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SecondSalesTaxValue { get; init; }

    /// <summary>
    /// Gets the URI reference to the accounting category for this item.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Category"/> used to classify this item in the chart of accounts.
    /// Determines how the revenue is reported in financial statements.
    /// </value>
    [JsonPropertyName("category")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Category { get; init; }

    /// <summary>
    /// Gets the URI reference to the project associated with this item.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Project"/> to which this item relates, if applicable.
    /// Used for project-based revenue tracking and profitability analysis.
    /// </value>
    [JsonPropertyName("project")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Project { get; init; }

    /// <summary>
    /// Gets the URI reference to the stock item from inventory.
    /// </summary>
    /// <value>
    /// The URI of the stock item in the inventory management system, if this line item
    /// references a tracked inventory product.
    /// </value>
    [JsonPropertyName("stock_item")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? StockItem { get; init; }

    /// <summary>
    /// Gets a value indicating whether this invoice item should be deleted during an update operation.
    /// </summary>
    /// <value>
    /// Set to 1 to mark this item for deletion when performing nested updates on an invoice.
    /// Used only for update requests, not returned in responses.
    /// </value>
    [JsonPropertyName("_destroy")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Destroy { get; init; }
}