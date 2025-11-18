// <copyright file="Expense.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Converters;

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents an expense claim in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Expenses track business costs incurred by users that need to be reimbursed or recorded for accounting purposes.
/// Expenses can represent standard purchases, mileage claims, or recurring expenses, and can optionally be rebilled to clients via projects.
/// </para>
/// <para>
/// Mileage expenses require special handling with vehicle type, engine details, and mileage tracking.
/// Expenses support multi-currency transactions and various rebilling methods (cost, markup, or fixed price).
/// </para>
/// <para>
/// For mileage expenses, the category must be set to "Mileage", and additional mileage-specific fields are required.
/// Sales tax on foreign currency expenses is generally non-reclaimable.
/// </para>
/// <para>
/// API Endpoint: /v2/expenses
/// </para>
/// <para>
/// Minimum Access Level: My Money
/// </para>
/// </remarks>
/// <seealso cref="User"/>
/// <seealso cref="Project"/>
/// <seealso cref="ExpenseAttachment"/>
public record Expense
{
    /// <summary>
    /// Gets the unique URI identifier for this expense.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this expense in the FreeAgent system.
    /// This value is assigned by the API upon creation and is used to reference the expense in other resources.
    /// </value>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the URI reference to the <see cref="Domain.User"/> who incurred this expense.
    /// </summary>
    /// <value>
    /// The URI of the user (claimant) submitting this expense for reimbursement or recording. This field is required when creating an expense.
    /// </value>
    /// <seealso cref="Domain.User"/>
    [JsonPropertyName("user")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? User { get; init; }

    /// <summary>
    /// Gets the URI reference to the <see cref="Domain.Project"/> for rebilling this expense.
    /// </summary>
    /// <value>
    /// The URI of the project to which this expense should be rebilled. Optional; only required if the expense will be charged to a client.
    /// </value>
    /// <seealso cref="Domain.Project"/>
    /// <seealso cref="RebillType"/>
    [JsonPropertyName("project")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Project { get; init; }

    /// <summary>
    /// Gets the total amount of the expense including tax.
    /// </summary>
    /// <value>
    /// The gross expense value. This field is required when creating an expense unless the
    /// <see cref="Category"/> is "Mileage", in which case it is calculated automatically from
    /// the mileage and applicable rates.
    /// Negative values represent payments made; positive values represent refunds received.
    /// </value>
    [JsonPropertyName("gross_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? GrossValue { get; init; }

    /// <summary>
    /// Gets the expense amount converted to the company's native currency.
    /// </summary>
    /// <value>
    /// The gross value in the company's native currency, automatically calculated based on exchange rates.
    /// Only present when the expense currency differs from the native currency.
    /// </value>
    [JsonPropertyName("native_gross_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? NativeGrossValue { get; init; }

    /// <summary>
    /// Gets the sales tax rate applied to this expense.
    /// </summary>
    /// <value>
    /// The VAT/GST rate as a decimal (e.g., 0.20 for 20% tax).
    /// </value>
    [JsonPropertyName("sales_tax_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SalesTaxRate { get; init; }

    /// <summary>
    /// Gets the calculated sales tax amount for this expense.
    /// </summary>
    /// <value>
    /// The VAT/GST amount in the expense currency. This is calculated from the gross value and sales tax rate.
    /// </value>
    [JsonPropertyName("sales_tax_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SalesTaxValue { get; init; }

    /// <summary>
    /// Gets the sales tax amount converted to the company's native currency.
    /// </summary>
    /// <value>
    /// The sales tax value in the company's native currency. Only present when the expense currency differs from the native currency.
    /// </value>
    [JsonPropertyName("native_sales_tax_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? NativeSalesTaxValue { get; init; }

    /// <summary>
    /// Gets the sales tax status for this expense.
    /// </summary>
    /// <value>
    /// One of <see cref="Domain.SalesTaxStatus.Taxable"/>, <see cref="Domain.SalesTaxStatus.Exempt"/>,
    /// or <see cref="Domain.SalesTaxStatus.OutOfScope"/>, determining how VAT/GST is applied to this expense.
    /// </value>
    [JsonPropertyName("sales_tax_status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(SalesTaxStatusJsonConverter))]
    public SalesTaxStatus? SalesTaxStatus { get; init; }

    /// <summary>
    /// Gets the second sales tax rate for Universal account types.
    /// </summary>
    /// <value>
    /// An additional tax rate applied in regions with multiple sales taxes. Only applicable for Universal account types.
    /// </value>
    [JsonPropertyName("second_sales_tax_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SecondSalesTaxRate { get; init; }

    /// <summary>
    /// Gets the second sales tax status for Universal account types.
    /// </summary>
    /// <value>
    /// The tax status for the second sales tax. Only applicable for Universal account types.
    /// </value>
    [JsonPropertyName("second_sales_tax_status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(SalesTaxStatusJsonConverter))]
    public SalesTaxStatus? SecondSalesTaxStatus { get; init; }

    /// <summary>
    /// Gets the description of the expense.
    /// </summary>
    /// <value>
    /// Free-text details describing what the expense was for.
    /// </value>
    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the date when the expense was incurred.
    /// </summary>
    /// <value>
    /// The expense date. This field is required when creating an expense.
    /// </value>
    [JsonPropertyName("dated_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? DatedOn { get; init; }

    /// <summary>
    /// Gets the accounting category for this expense.
    /// </summary>
    /// <value>
    /// The full category URI (e.g., "https://api.freeagent.com/v2/categories/285") or the literal string "Mileage" for mileage expenses.
    /// This field is required when creating an expense. For non-mileage expenses, always use the full category URI, not just a category name or ID.
    /// </value>
    [JsonPropertyName("category")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Category { get; init; }

    /// <summary>
    /// Gets the URI reference to an alternative project for rebilling this expense.
    /// </summary>
    /// <value>
    /// The URI of a different project for rebilling. Use this instead of <see cref="Project"/> when rebilling to a different project.
    /// </value>
    [JsonPropertyName("rebill_to_project")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? RebillToProject { get; init; }

    /// <summary>
    /// Gets the URI reference to the invoice on which this expense was rebilled.
    /// </summary>
    /// <value>
    /// The URI of the invoice that includes this rebilled expense. This is a read-only field set by the system.
    /// </value>
    [JsonPropertyName("rebilled_on_invoice")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? RebilledOnInvoice { get; init; }

    /// <summary>
    /// Gets the URI reference to the property associated with this expense.
    /// </summary>
    /// <value>
    /// The URI of the property. Required for UkUnincorporatedLandlord accounts when the expense is property-related.
    /// </value>
    [JsonPropertyName("property")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Property { get; init; }

    /// <summary>
    /// Gets the recurring pattern for this expense.
    /// </summary>
    /// <value>
    /// The recurrence pattern. Valid values include <see cref="RecurringPattern.Weekly"/>,
    /// <see cref="RecurringPattern.TwoWeekly"/>, <see cref="RecurringPattern.FourWeekly"/>,
    /// <see cref="RecurringPattern.TwoMonthly"/>, <see cref="RecurringPattern.Quarterly"/>,
    /// <see cref="RecurringPattern.Biannually"/>, <see cref="RecurringPattern.Annually"/>,
    /// and <see cref="RecurringPattern.TwoYearly"/>.
    /// </value>
    [JsonPropertyName("recurring")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(RecurringPatternJsonConverter))]
    public RecurringPattern? Recurring { get; init; }

    /// <summary>
    /// Gets the next recurrence date for a recurring expense.
    /// </summary>
    /// <value>
    /// The date when this expense will next recur. Only applicable for recurring expenses.
    /// </value>
    [JsonPropertyName("next_recurs_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? NextRecursOn { get; init; }

    /// <summary>
    /// Gets the end date for a recurring expense.
    /// </summary>
    /// <value>
    /// The date after which this expense will no longer recur. Optional for recurring expenses.
    /// </value>
    [JsonPropertyName("recurring_end_date")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? RecurringEndDate { get; init; }

    /// <summary>
    /// Gets the URI reference to the stock item for this expense.
    /// </summary>
    /// <value>
    /// The URI of the stock item being purchased. Used for stock purchase expenses.
    /// </value>
    [JsonPropertyName("stock_item")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? StockItem { get; init; }

    /// <summary>
    /// Gets the description of the stock item.
    /// </summary>
    /// <value>
    /// The description of the stock item associated with this expense. This is a read-only field populated from the stock item.
    /// </value>
    [JsonPropertyName("stock_item_description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? StockItemDescription { get; init; }

    /// <summary>
    /// Gets the quantity of stock being purchased or consumed.
    /// </summary>
    /// <value>
    /// The quantity of stock items. Positive for purchases, negative for consumption. Used with stock item expenses.
    /// </value>
    [JsonPropertyName("stock_altering_quantity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? StockAlteringQuantity { get; init; }

    /// <summary>
    /// Gets the URI reference to the capital asset created from this expense.
    /// </summary>
    /// <value>
    /// The URI of the capital asset. This is a read-only field set when an expense is converted to a capital asset.
    /// </value>
    [JsonPropertyName("capital_asset")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? CapitalAsset { get; init; }

    /// <summary>
    /// Gets the depreciation schedule for capital expenses.
    /// </summary>
    /// <value>
    /// The depreciation schedule type. This field is deprecated but still present in the API for backwards compatibility.
    /// </value>
    [JsonPropertyName("depreciation_schedule")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DepreciationSchedule { get; init; }

    /// <summary>
    /// Gets the distance traveled for mileage expenses.
    /// </summary>
    /// <value>
    /// The number of miles or kilometers traveled (depending on company settings). Only applicable when <see cref="Category"/> is "Mileage".
    /// Decimal values are supported (e.g., 15.5 miles).
    /// </value>
    /// <seealso cref="VehicleType"/>
    [JsonPropertyName("mileage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Mileage { get; init; }

    /// <summary>
    /// Gets the mileage reimbursement rate to be reclaimed.
    /// </summary>
    /// <value>
    /// The per-mile or per-kilometer rate for reimbursement calculation. Used with AMAP (Approved Mileage Allowance Payment) for UK expenses.
    /// </value>
    [JsonPropertyName("reclaim_mileage_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? ReclaimMileageRate { get; init; }

    /// <summary>
    /// Gets the mileage rate for rebilling to a client.
    /// </summary>
    /// <value>
    /// The per-mile or per-kilometer rate to charge the client when rebilling mileage expenses.
    /// </value>
    [JsonPropertyName("rebill_mileage_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? RebillMileageRate { get; init; }

    /// <summary>
    /// Gets the rebilling method for this expense.
    /// </summary>
    /// <value>
    /// One of <see cref="Domain.RebillType.Cost"/> (rebill at actual cost), <see cref="Domain.RebillType.Markup"/>
    /// (add percentage markup), or <see cref="Domain.RebillType.Price"/> (rebill at fixed price).
    /// Only applicable when <see cref="Project"/> is specified.
    /// </value>
    /// <seealso cref="Project"/>
    /// <seealso cref="RebillFactor"/>
    [JsonPropertyName("rebill_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(RebillTypeJsonConverter))]
    public RebillType? RebillType { get; init; }

    /// <summary>
    /// Gets the initial rate mileage for calculation purposes.
    /// </summary>
    /// <value>
    /// The starting mileage value used for mileage expense calculations.
    /// </value>
    [JsonPropertyName("initial_rate_mileage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? InitialRateMileage { get; init; }

    /// <summary>
    /// Gets the receipt reference or identifier.
    /// </summary>
    /// <value>
    /// An optional reference number or identifier from the receipt for tracking purposes.
    /// </value>
    [JsonPropertyName("receipt_reference")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ReceiptReference { get; init; }

    /// <summary>
    /// Gets the European Community VAT reporting status.
    /// </summary>
    /// <value>
    /// The EC status for VAT reporting purposes. Valid values include <see cref="Domain.EcStatus.UkNonEc"/>,
    /// <see cref="Domain.EcStatus.EcGoods"/>, <see cref="Domain.EcStatus.EcServices"/>, and
    /// <see cref="Domain.EcStatus.ReverseCharge"/>. Options changed post-Brexit for GB-based companies.
    /// </value>
    [JsonPropertyName("ec_status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(EcStatusJsonConverter))]
    public EcStatus? EcStatus { get; init; }

    /// <summary>
    /// Gets the currency for this expense.
    /// </summary>
    /// <value>
    /// A three-letter ISO 4217 currency code (e.g., "GBP", "USD", "EUR"). Defaults to the company's native currency.
    /// Sales tax on foreign currency expenses is generally non-reclaimable.
    /// </value>
    [JsonPropertyName("currency")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Currency { get; init; }

    /// <summary>
    /// Gets the manual sales tax amount in the company's native currency.
    /// </summary>
    /// <value>
    /// The manually specified tax amount, used when automatic tax calculation is not applicable (e.g., for foreign currency expenses).
    /// </value>
    [JsonPropertyName("manual_sales_tax_amount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? ManualSalesTaxAmount { get; init; }

    /// <summary>
    /// Gets the rebilling factor for markup or pricing calculations.
    /// </summary>
    /// <value>
    /// The multiplier or percentage used in conjunction with <see cref="RebillType"/> to calculate the rebill amount.
    /// </value>
    /// <seealso cref="RebillType"/>
    [JsonPropertyName("rebill_factor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? RebillFactor { get; init; }

    /// <summary>
    /// Gets the type of vehicle used for mileage expenses.
    /// </summary>
    /// <value>
    /// One of <see cref="Domain.VehicleType.Car"/>, <see cref="Domain.VehicleType.Motorcycle"/>,
    /// or <see cref="Domain.VehicleType.Bicycle"/>. Required when <see cref="Category"/> is "Mileage".
    /// </value>
    /// <seealso cref="Mileage"/>
    [JsonPropertyName("vehicle_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(VehicleTypeJsonConverter))]
    public VehicleType? VehicleType { get; init; }

    /// <summary>
    /// Gets the engine type for the vehicle.
    /// </summary>
    /// <value>
    /// The engine fuel type. Valid values include <see cref="Domain.EngineType.Petrol"/> (default),
    /// <see cref="Domain.EngineType.Diesel"/>, <see cref="Domain.EngineType.Lpg"/>,
    /// <see cref="Domain.EngineType.Electric"/>, <see cref="Domain.EngineType.ElectricHomeCharger"/>,
    /// and <see cref="Domain.EngineType.ElectricPublicCharger"/>.
    /// Required for car mileage expenses.
    /// </value>
    [JsonPropertyName("engine_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(EngineTypeJsonConverter))]
    public EngineType? EngineType { get; init; }

    /// <summary>
    /// Gets the engine size category for the vehicle.
    /// </summary>
    /// <value>
    /// The engine displacement category (e.g., "Up to 1400cc", "1401cc to 2000cc", "Over 2000cc").
    /// Required for car mileage expenses with combustion engines.
    /// </value>
    [JsonPropertyName("engine_size")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EngineSize { get; init; }

    /// <summary>
    /// Gets whether mileage should be reclaimed.
    /// </summary>
    /// <value>
    /// Set to 1 to reclaim mileage, 0 to not reclaim. Controls whether mileage reimbursement is claimed.
    /// </value>
    [JsonPropertyName("reclaim_mileage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ReclaimMileage { get; init; }

    /// <summary>
    /// Gets a value indicating whether a VAT receipt is available for this mileage expense.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if a VAT receipt is held for the fuel purchase; otherwise, <see langword="false"/>.
    /// Used for mileage expense VAT reclaim purposes in the UK.
    /// </value>
    [JsonPropertyName("have_vat_receipt")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? HaveVatReceipt { get; init; }

    /// <summary>
    /// Gets the timestamp when this expense was created.
    /// </summary>
    /// <value>
    /// The creation timestamp in ISO 8601 format. This is a read-only field set by the system.
    /// </value>
    [JsonPropertyName("created_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? CreatedAt { get; init; }

    /// <summary>
    /// Gets the timestamp when this expense was last updated.
    /// </summary>
    /// <value>
    /// The last update timestamp in ISO 8601 format. This is a read-only field maintained by the system.
    /// </value>
    [JsonPropertyName("updated_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? UpdatedAt { get; init; }

    /// <summary>
    /// Gets the file attachment for this expense.
    /// </summary>
    /// <value>
    /// An <see cref="ExpenseAttachment"/> object containing receipt image or document data (maximum 5MB).
    /// </value>
    /// <seealso cref="ExpenseAttachment"/>
    [JsonPropertyName("attachment")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ExpenseAttachment? Attachment { get; init; }
}