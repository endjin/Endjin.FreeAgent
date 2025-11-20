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
/// Minimum Access Level: Invoices, Estimates and Files
/// </para>
/// </remarks>
/// <seealso cref="Category"/>
/// <seealso cref="Invoice"/>
/// <seealso cref="Bill"/>
[DebuggerDisplay("Description = {" + nameof(Description) + "}")]
public record StockItem
{
    /// <summary>
    /// Gets the unique URI identifier for this stock item.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this stock item in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    public required Uri Url { get; init; }

    /// <summary>
    /// Gets the description of this stock item.
    /// </summary>
    /// <value>
    /// A free-text description or code to identify the item when it's added to an invoice or estimate.
    /// </value>
    [JsonPropertyName("description")]
    public required string Description { get; init; }

    /// <summary>
    /// Gets the opening quantity of stock as of FreeAgent Start Date.
    /// </summary>
    /// <value>
    /// The number of units in stock as of FreeAgent Start Date.
    /// </value>
    [JsonPropertyName("opening_quantity")]
    public required decimal OpeningQuantity { get; init; }

    /// <summary>
    /// Gets the opening balance (total value) of stock as of FreeAgent Start Date.
    /// </summary>
    /// <value>
    /// The value of stock on hand as of FreeAgent Start Date.
    /// </value>
    [JsonPropertyName("opening_balance")]
    public required decimal OpeningBalance { get; init; }

    /// <summary>
    /// Gets the URI reference to the cost of sales accounting category.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Category"/> in the chart of accounts where cost of sales is recognized
    /// when this stock item is sold. Typically an expense account that reduces gross profit.
    /// </value>
    [JsonPropertyName("cost_of_sale_category")]
    public required Uri CostOfSaleCategory { get; init; }

    /// <summary>
    /// Gets the current quantity of stock on hand.
    /// </summary>
    /// <value>
    /// The current number of units in stock, updated as stock is purchased and sold.
    /// </value>
    [JsonPropertyName("stock_on_hand")]
    public required decimal StockOnHand { get; init; }

    /// <summary>
    /// Gets the date and time when this stock item was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> in UTC representing when this stock item was first added to the system.
    /// </value>
    [JsonPropertyName("created_at")]
    public required DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this stock item was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> in UTC representing the last time this stock item record was modified.
    /// </value>
    [JsonPropertyName("updated_at")]
    public required DateTimeOffset UpdatedAt { get; init; }
}