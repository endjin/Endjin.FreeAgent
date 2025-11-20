// <copyright file="EstimateItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a line item within an estimate in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Estimate items detail individual products, services, or charges that make up an estimate. Each item has
/// its own description, quantity, price, and tax treatment. Estimate items support various unit types for
/// quantity measurement including hours, days, weeks, months, and years for time-based quotes.
/// </para>
/// <para>
/// Items can be linked to accounting categories for proper classification. Tax calculations support various
/// VAT/GST scenarios, with both tax rates and calculated tax values tracked separately.
/// </para>
/// <para>
/// API Endpoint: /v2/estimate_items (accessed via estimate relationship)
/// </para>
/// <para>
/// Minimum Access Level: Invoices
/// </para>
/// </remarks>
/// <seealso cref="Estimate"/>
/// <seealso cref="Category"/>
[DebuggerDisplay("{" + nameof(Description) + "}, {" + nameof(Price) + "}")]
public record EstimateItem
{
    /// <summary>
    /// Gets the unique URI identifier for this estimate item.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this line item in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the URI reference to the parent estimate for this item.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Estimate"/> that contains this line item.
    /// Required when creating a new estimate item via POST /v2/estimate_items.
    /// </value>
    [JsonPropertyName("estimate")]
    public Uri? Estimate { get; init; }

    /// <summary>
    /// Gets the display order position of this item within the estimate.
    /// </summary>
    /// <value>
    /// An integer value starting at 1 that determines the order in which items appear on the estimate.
    /// Lower numbers appear first.
    /// </value>
    [JsonPropertyName("position")]
    public int? Position { get; init; }

    /// <summary>
    /// Gets the unit type for this item's quantity measurement.
    /// </summary>
    /// <value>
    /// One of "Hours", "Days", "Weeks", "Months", "Years", "Products", "Services", "Training", "Expenses",
    /// "Comments", "Bills", "Discount", "Credit", "-no unit-", or other unit identifiers
    /// that describe how <see cref="Quantity"/> should be interpreted and displayed.
    /// </value>
    [JsonPropertyName("item_type")]
    public string? ItemType { get; init; }

    /// <summary>
    /// Gets the quantity of units for this item.
    /// </summary>
    /// <value>
    /// The number of units being quoted. Multiplied by <see cref="Price"/> to calculate line totals.
    /// The unit type is specified by <see cref="ItemType"/>.
    /// </value>
    [JsonPropertyName("quantity")]
    public decimal? Quantity { get; init; }

    /// <summary>
    /// Gets the unit price for this item.
    /// </summary>
    /// <value>
    /// The price per unit, excluding tax. Multiplied by <see cref="Quantity"/> to calculate
    /// the subtotal for this line item.
    /// </value>
    [JsonPropertyName("price")]
    public decimal? Price { get; init; }

    /// <summary>
    /// Gets the description of the product or service.
    /// </summary>
    /// <value>
    /// The text description that appears on the estimate line item, explaining what is being quoted.
    /// </value>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the calculated sales tax amount for this item.
    /// </summary>
    /// <value>
    /// The total tax amount calculated by applying <see cref="SalesTaxRate"/> to the line item subtotal.
    /// </value>
    [JsonPropertyName("sales_tax_value")]
    public decimal? SalesTaxValue { get; init; }

    /// <summary>
    /// Gets the sales tax rate applied to this item.
    /// </summary>
    /// <value>
    /// The VAT/GST rate as a percentage (e.g., 20.0 for 20% tax). Used to calculate <see cref="SalesTaxValue"/>.
    /// </value>
    [JsonPropertyName("sales_tax_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SalesTaxRate { get; init; }

    /// <summary>
    /// Gets the sales tax status classification for this item.
    /// </summary>
    /// <value>
    /// One of "TAXABLE" or "EXEMPT", determining whether sales tax is applied to this item.
    /// </value>
    [JsonPropertyName("sales_tax_status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SalesTaxStatus { get; init; }

    /// <summary>
    /// Gets the secondary sales tax rate for jurisdictions with dual tax systems.
    /// </summary>
    /// <value>
    /// An additional tax rate (e.g., PST in Canadian provinces with GST/PST) applied on top of
    /// the primary <see cref="SalesTaxRate"/> for universal accounts.
    /// </value>
    [JsonPropertyName("second_sales_tax_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SecondSalesTaxRate { get; init; }

    /// <summary>
    /// Gets the secondary sales tax status classification for this item.
    /// </summary>
    /// <value>
    /// The tax status for the secondary tax rate, determining whether the second tax is applied.
    /// </value>
    [JsonPropertyName("second_sales_tax_status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SecondSalesTaxStatus { get; init; }

    /// <summary>
    /// Gets the calculated secondary sales tax amount for this item.
    /// </summary>
    /// <value>
    /// The total secondary tax amount calculated by applying <see cref="SecondSalesTaxRate"/>
    /// to the line item subtotal for dual-tax jurisdictions.
    /// </value>
    [JsonPropertyName("second_sales_tax_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SecondSalesTaxValue { get; init; }

    /// <summary>
    /// Gets the URI reference to the accounting category for this item.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Category"/> used to classify this item in the chart of accounts.
    /// Determines how the revenue will be reported in financial statements if the estimate is converted to an invoice.
    /// </value>
    [JsonPropertyName("category")]
    public Uri? Category { get; init; }

    /// <summary>
    /// Gets the date and time when this estimate item was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the last modification timestamp with timezone information.
    /// </value>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this estimate item was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the creation timestamp with timezone information.
    /// </value>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }
}