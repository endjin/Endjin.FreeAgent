// <copyright file="StockItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a stock/inventory item in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Stock items track inventory for businesses that buy and sell physical goods. They manage quantities,
/// values, and movements of stock, automatically updating cost of sales and stock-on-hand accounts as
/// items are purchased and sold.
/// </para>
/// <para>
/// Stock items support both periodic and perpetual inventory systems:
/// - Opening quantities and balances establish initial stock levels
/// - Purchases increase stock quantity and value
/// - Sales decrease stock quantity and recognize cost of sales
/// - Stock movements are reflected in the chart of accounts
/// </para>
/// <para>
/// Each stock item links to two accounting categories: one for the asset value (stock on hand) and one
/// for recognizing costs when stock is sold (cost of sales). This ensures proper accounting treatment
/// and accurate profit calculations.
/// </para>
/// <para>
/// API Endpoint: /v2/stock_items
/// </para>
/// <para>
/// Minimum Access Level: Standard
/// </para>
/// </remarks>
/// <seealso cref="Category"/>
/// <seealso cref="Invoice"/>
/// <seealso cref="Bill"/>
public record StockItem
{
    /// <summary>
    /// Gets the unique URI identifier for this stock item.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this stock item in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the description of this stock item.
    /// </summary>
    /// <value>
    /// A descriptive name for the stock item, such as "Widget Model A" or "Premium Coffee Beans 1kg".
    /// </value>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the opening quantity of stock at the start of tracking.
    /// </summary>
    /// <value>
    /// The initial number of units in stock when this item was first created or at the start
    /// of the accounting period.
    /// </value>
    [JsonPropertyName("opening_quantity")]
    public decimal? OpeningQuantity { get; init; }

    /// <summary>
    /// Gets the opening balance (total value) of stock at the start of tracking.
    /// </summary>
    /// <value>
    /// The initial monetary value of all units in stock, used to establish the starting position
    /// for stock valuation and cost of sales calculations.
    /// </value>
    [JsonPropertyName("opening_balance")]
    public decimal? OpeningBalance { get; init; }

    /// <summary>
    /// Gets the URI reference to the cost of sales accounting category.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Category"/> in the chart of accounts where cost of sales is recognized
    /// when this stock item is sold. Typically an expense account that reduces gross profit.
    /// </value>
    [JsonPropertyName("cost_of_sale_category")]
    public Uri? CostOfSaleCategory { get; init; }

    /// <summary>
    /// Gets the URI reference to the stock on hand accounting category.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Category"/> in the chart of accounts representing the asset value
    /// of unsold stock. Typically a current asset account on the balance sheet.
    /// </value>
    [JsonPropertyName("stock_on_hand_category")]
    public Uri? StockOnHandCategory { get; init; }

    /// <summary>
    /// Gets the type classification of this stock item.
    /// </summary>
    /// <value>
    /// The stock item type such as "Product" or "Raw Material", used to categorize different
    /// types of inventory.
    /// </value>
    [JsonPropertyName("stock_item_type")]
    public string? StockItemType { get; init; }

    /// <summary>
    /// Gets the date and time when this stock item was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing when this stock item was first added to the system.
    /// </value>
    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this stock item was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing the last time this stock item record was modified.
    /// </value>
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }
}