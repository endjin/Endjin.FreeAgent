// <copyright file="BillItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a line item within a bill in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Bill items detail individual products, services, or charges that make up a bill. Each item has
/// its own description, quantity, price, and tax treatment. Bills support up to 40 line items per bill.
/// </para>
/// <para>
/// Items must be linked to accounting categories for proper expense classification. Tax calculations
/// support various VAT/GST scenarios including taxable, exempt, and out-of-scope items. Some jurisdictions
/// support dual tax rates (e.g., Canadian GST/PST).
/// </para>
/// <para>
/// Bill items can reference stock items from inventory management and support capital asset tracking
/// for depreciation purposes. For UK Construction Industry Scheme (CIS) contractors, CIS deduction
/// rates are automatically calculated.
/// </para>
/// <para>
/// API Endpoint: /v2/bill_items (accessed via bill relationship)
/// </para>
/// <para>
/// Minimum Access Level: Bills
/// </para>
/// </remarks>
/// <seealso cref="Bill"/>
/// <seealso cref="Category"/>
/// <seealso cref="Project"/>
[DebuggerDisplay("{" + nameof(Description) + "}, {" + nameof(TotalValue) + "}")]
public record BillItem
{
    /// <summary>
    /// Gets the unique URI identifier for this bill item.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this line item in the FreeAgent system.
    /// Leave blank when creating new bill items.
    /// </value>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the URI reference to the parent bill.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Bill"/> that contains this line item. This is a read-only field
    /// set automatically by the system.
    /// </value>
    [JsonPropertyName("bill")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Bill { get; init; }

    /// <summary>
    /// Gets the URI reference to the accounting category.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Category"/> used to classify this expense item in the chart of accounts.
    /// This field is required when creating bill items.
    /// </value>
    [JsonPropertyName("category")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Category { get; init; }

    /// <summary>
    /// Gets the description of the product or service.
    /// </summary>
    /// <value>
    /// The text description that appears on the bill line item. This field is required when the category
    /// is a capital asset category.
    /// </value>
    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the total value of this item including tax.
    /// </summary>
    /// <value>
    /// The total amount for this line item including all taxes. Either this field or
    /// <see cref="TotalValueExTax"/> must be provided when creating a bill item.
    /// </value>
    [JsonPropertyName("total_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? TotalValue { get; init; }

    /// <summary>
    /// Gets the total value of this item excluding tax.
    /// </summary>
    /// <value>
    /// The total amount for this line item before tax is applied. Either this field or
    /// <see cref="TotalValue"/> must be provided when creating a bill item.
    /// </value>
    [JsonPropertyName("total_value_ex_tax")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? TotalValueExTax { get; init; }

    /// <summary>
    /// Gets the quantity of units for this item.
    /// </summary>
    /// <value>
    /// The number of units for this line item. Defaults to 1 if not specified.
    /// </value>
    [JsonPropertyName("quantity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Quantity { get; init; }

    /// <summary>
    /// Gets the unit type for this item's quantity measurement.
    /// </summary>
    /// <value>
    /// The unit type such as "-no unit-", "Hours", "Days", "Weeks", "Months", "Years", "Products",
    /// "Services", "Training", or "Stock".
    /// </value>
    [JsonPropertyName("unit")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Unit { get; init; }

    /// <summary>
    /// Gets the sales tax rate applied to this item.
    /// </summary>
    /// <value>
    /// The primary VAT/GST rate as a decimal (e.g., 0.20 for 20% tax). Used to calculate
    /// <see cref="SalesTaxValue"/>.
    /// </value>
    [JsonPropertyName("sales_tax_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SalesTaxRate { get; init; }

    /// <summary>
    /// Gets the calculated sales tax amount for this item.
    /// </summary>
    /// <value>
    /// The total tax amount calculated by applying <see cref="SalesTaxRate"/> to the line item value.
    /// This is a read-only field calculated by the system.
    /// </value>
    [JsonPropertyName("sales_tax_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SalesTaxValue { get; init; }

    /// <summary>
    /// Gets the sales tax status classification for this item.
    /// </summary>
    /// <value>
    /// One of "TAXABLE", "EXEMPT", or "OUT_OF_SCOPE", determining how tax is applied to this item.
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
    /// Gets the calculated secondary sales tax amount for this item.
    /// </summary>
    /// <value>
    /// The total secondary tax amount calculated by applying <see cref="SecondSalesTaxRate"/>
    /// to the line item value for dual-tax jurisdictions. This is a read-only field calculated by the system.
    /// </value>
    [JsonPropertyName("second_sales_tax_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SecondSalesTaxValue { get; init; }

    /// <summary>
    /// Gets the secondary sales tax status classification for this item.
    /// </summary>
    /// <value>
    /// The tax status for the secondary tax rate, determining whether the second tax is applied
    /// in universal account configurations.
    /// </value>
    [JsonPropertyName("second_sales_tax_status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SecondSalesTaxStatus { get; init; }

    /// <summary>
    /// Gets the manually specified sales tax amount.
    /// </summary>
    /// <value>
    /// A manually entered tax amount that overrides automatic tax calculations when needed.
    /// </value>
    [JsonPropertyName("manual_sales_tax_amount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? ManualSalesTaxAmount { get; init; }

    /// <summary>
    /// Gets the URI reference to the stock item from inventory.
    /// </summary>
    /// <value>
    /// The URI of the stock item in the inventory management system. This field is required
    /// when the category is a Stock category.
    /// </value>
    [JsonPropertyName("stock_item")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? StockItem { get; init; }

    /// <summary>
    /// Gets the description of the referenced stock item.
    /// </summary>
    /// <value>
    /// The stock item's description from the inventory system. This is a read-only field
    /// populated automatically when a <see cref="StockItem"/> is referenced.
    /// </value>
    [JsonPropertyName("stock_item_description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? StockItemDescription { get; init; }

    /// <summary>
    /// Gets the quantity that affects stock levels.
    /// </summary>
    /// <value>
    /// The quantity by which stock levels should be adjusted. This field is required
    /// when the category is a Stock category.
    /// </value>
    [JsonPropertyName("stock_altering_quantity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? StockAlteringQuantity { get; init; }

    /// <summary>
    /// Gets the URI reference to the capital asset.
    /// </summary>
    /// <value>
    /// The URI of the capital asset created for this item when the category is a capital asset category.
    /// This is a read-only field set automatically by the system.
    /// </value>
    [JsonPropertyName("capital_asset")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? CapitalAsset { get; init; }

    /// <summary>
    /// Gets the depreciation schedule for capital assets.
    /// </summary>
    /// <value>
    /// The depreciation schedule identifier. This field is deprecated and read-only.
    /// </value>
    [JsonPropertyName("depreciation_schedule")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DepreciationSchedule { get; init; }

    /// <summary>
    /// Gets the URI reference to the project associated with this item.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Project"/> to which this expense relates, if applicable.
    /// Used for project-based expense tracking and profitability analysis.
    /// </value>
    [JsonPropertyName("project")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Project { get; init; }

    /// <summary>
    /// Gets the Construction Industry Scheme (CIS) deduction rate.
    /// </summary>
    /// <value>
    /// The CIS deduction percentage automatically calculated based on the supplier's CIS status.
    /// This is a read-only field for UK CIS contractors.
    /// </value>
    [JsonPropertyName("cis_deduction_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? CisDeductionRate { get; init; }

    /// <summary>
    /// Gets a value indicating whether this bill item should be deleted during an update operation.
    /// </summary>
    /// <value>
    /// Set to 1 to mark this item for deletion when performing nested updates on a bill.
    /// Used only for update requests, not returned in responses.
    /// </value>
    [JsonPropertyName("_destroy")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Destroy { get; init; }
}
