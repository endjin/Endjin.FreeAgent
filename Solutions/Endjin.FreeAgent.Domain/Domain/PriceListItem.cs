// <copyright file="PriceListItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a price list item in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Price list items are pre-configured products or services that can be quickly added to invoices
/// and estimates. They store standard information like description, quantity, unit price, and tax
/// settings, making document creation faster and more consistent.
/// </para>
/// <para>
/// Each price list item has a unique code that can be referenced when adding items to invoices
/// or estimates. The item type determines the unit of measurement (Hours, Days, Products, etc.)
/// and affects how the item is displayed and calculated.
/// </para>
/// <para>
/// Price list items can optionally reference:
/// </para>
/// <list type="bullet">
/// <item><description>An income category for accounting classification</description></item>
/// <item><description>A stock item for inventory tracking (when item_type is "Stock")</description></item>
/// </list>
/// <para>
/// API Endpoint: /v2/price_list_items
/// </para>
/// <para>
/// Minimum Access Level: Invoices, Estimates and Files
/// </para>
/// </remarks>
/// <seealso cref="Invoice"/>
/// <seealso cref="Estimate"/>
/// <seealso cref="StockItem"/>
/// <seealso cref="Category"/>
public record PriceListItem
{
    /// <summary>
    /// Gets the unique URI identifier for this price list item.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this price list item in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the unique code for this price list item.
    /// </summary>
    /// <value>
    /// A unique code used to reference this item when adding it to invoices and estimates.
    /// This code must be unique across all price list items in the account.
    /// </value>
    [JsonPropertyName("code")]
    public string? Code { get; init; }

    /// <summary>
    /// Gets the default quantity for this price list item.
    /// </summary>
    /// <value>
    /// The default number of units to add when this item is selected, such as hours worked
    /// or number of products.
    /// </value>
    [JsonPropertyName("quantity")]
    public decimal? Quantity { get; init; }

    /// <summary>
    /// Gets the type of item, which determines the unit of measurement.
    /// </summary>
    /// <value>
    /// The item type, which can be one of: Hours, Days, Weeks, Months, Years, Products,
    /// Services, Training, Expenses, Comment, Bills, Discount, Credit, VAT, or Stock.
    /// </value>
    [JsonPropertyName("item_type")]
    public string? ItemType { get; init; }

    /// <summary>
    /// Gets the description of this price list item.
    /// </summary>
    /// <value>
    /// A free-text description that will appear on invoices and estimates when this item is added.
    /// </value>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the unit price for this price list item.
    /// </summary>
    /// <value>
    /// The price per unit of this item. The total line value is calculated by multiplying
    /// this price by the quantity.
    /// </value>
    [JsonPropertyName("price")]
    public decimal? Price { get; init; }

    /// <summary>
    /// Gets the VAT status for this price list item (UK accounts only).
    /// </summary>
    /// <value>
    /// The VAT status, which can be one of: out_of_scope (default), reduced, standard, or zero.
    /// This property only applies to UK accounts.
    /// </value>
    [JsonPropertyName("vat_status")]
    public string? VatStatus { get; init; }

    /// <summary>
    /// Gets the primary sales tax rate for this price list item (Universal and US accounts only).
    /// </summary>
    /// <value>
    /// The percentage rate for the primary sales tax applied to this item.
    /// This property only applies to Universal and US accounts.
    /// </value>
    [JsonPropertyName("sales_tax_rate")]
    public decimal? SalesTaxRate { get; init; }

    /// <summary>
    /// Gets the secondary sales tax rate for this price list item (Universal accounts only).
    /// </summary>
    /// <value>
    /// The percentage rate for an additional sales tax applied to this item.
    /// This property only applies to Universal accounts that support multiple tax rates.
    /// </value>
    [JsonPropertyName("second_sales_tax_rate")]
    public decimal? SecondSalesTaxRate { get; init; }

    /// <summary>
    /// Gets the URI reference to the income accounting category.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Category"/> in the chart of accounts where income from
    /// this item is recorded. This determines how sales are classified in financial reports.
    /// </value>
    [JsonPropertyName("category")]
    public Uri? Category { get; init; }

    /// <summary>
    /// Gets the URI reference to the associated stock item.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="StockItem"/> linked to this price list item. This is required
    /// when the <see cref="ItemType"/> is "Stock", and enables automatic inventory tracking
    /// when the item is invoiced.
    /// </value>
    [JsonPropertyName("stock_item")]
    public Uri? StockItem { get; init; }

    /// <summary>
    /// Gets the date and time when this price list item was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing when this price list item was first added to the system.
    /// </value>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this price list item was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the last time this price list item record was modified.
    /// </value>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; init; }
}
