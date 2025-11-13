// <copyright file="Expense.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

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
    public string? Url { get; init; }

    /// <summary>
    /// Gets the URI reference to the <see cref="Domain.User"/> who incurred this expense.
    /// </summary>
    /// <value>
    /// The URI of the user (claimant) submitting this expense for reimbursement or recording. This field is required when creating an expense.
    /// </value>
    /// <seealso cref="Domain.User"/>
    [JsonPropertyName("user")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? User { get; init; }

    /// <summary>
    /// Gets the type of expense.
    /// </summary>
    /// <value>
    /// The expense classification type.
    /// </value>
    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Type { get; init; }

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
    public string? Project { get; init; }

    /// <summary>
    /// Gets the total amount of the expense including tax.
    /// </summary>
    /// <value>
    /// The gross expense value. This field is required when creating an expense.
    /// Negative values represent payments made; positive values represent refunds received.
    /// </value>
    [JsonPropertyName("gross_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? GrossValue { get; init; }

    /// <summary>
    /// Gets the sales tax rate applied to this expense.
    /// </summary>
    /// <value>
    /// The VAT/GST rate as a decimal (e.g., 0.20 for 20% tax).
    /// </value>
    [JsonPropertyName("sales_tax_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? SalesTaxRate { get; init; }

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
    /// The expense date in YYYY-MM-DD format. This field is required when creating an expense.
    /// </value>
    [JsonPropertyName("dated_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DatedOn { get; init; }

    /// <summary>
    /// Gets the accounting category for this expense.
    /// </summary>
    /// <value>
    /// The category URI or "Mileage" for mileage expenses. This field is required when creating an expense.
    /// </value>
    [JsonPropertyName("category")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Category { get; init; }

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
    public double? Mileage { get; init; }

    /// <summary>
    /// Gets the mileage reimbursement rate to be reclaimed.
    /// </summary>
    /// <value>
    /// The per-mile or per-kilometer rate for reimbursement calculation. Used with AMAP (Approved Mileage Allowance Payment) for UK expenses.
    /// </value>
    [JsonPropertyName("reclaim_mileage_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? ReclaimMileageRate { get; init; }

    /// <summary>
    /// Gets or sets the mileage rate for rebilling to a client.
    /// </summary>
    /// <value>
    /// The per-mile or per-kilometer rate to charge the client when rebilling mileage expenses.
    /// </value>
    [JsonPropertyName("rebill_mileage_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? RebillMileageRate { get; set; }

    /// <summary>
    /// Gets or sets the rebilling method for this expense.
    /// </summary>
    /// <value>
    /// One of "Cost" (rebill at actual cost), "Markup" (add percentage markup), or "Price" (rebill at fixed price).
    /// Only applicable when <see cref="Project"/> is specified.
    /// </value>
    /// <seealso cref="Project"/>
    /// <seealso cref="RebillFactor"/>
    [JsonPropertyName("rebill_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? RebillType { get; set; }

    /// <summary>
    /// Gets or sets the initial rate mileage for calculation purposes.
    /// </summary>
    /// <value>
    /// The starting mileage value used for mileage expense calculations.
    /// </value>
    [JsonPropertyName("initial_rate_mileage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? InitialRateMileage { get; set; }

    /// <summary>
    /// Gets or sets the receipt reference or identifier.
    /// </summary>
    /// <value>
    /// An optional reference number or identifier from the receipt for tracking purposes.
    /// </value>
    [JsonPropertyName("receipt_reference")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ReceiptReference { get; set; }

    /// <summary>
    /// Gets or sets the European Community VAT reporting status.
    /// </summary>
    /// <value>
    /// The EC status for VAT reporting purposes. This field is required when creating an expense.
    /// Options changed post-Brexit for GB-based companies.
    /// </value>
    [JsonPropertyName("ec_status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EcStatus { get; set; }

    /// <summary>
    /// Gets or sets the currency for this expense.
    /// </summary>
    /// <value>
    /// A three-letter ISO 4217 currency code (e.g., "GBP", "USD", "EUR"). Defaults to the company's native currency.
    /// Sales tax on foreign currency expenses is generally non-reclaimable.
    /// </value>
    [JsonPropertyName("currency")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Currency { get; set; }

    /// <summary>
    /// Gets or sets the manual sales tax amount in the company's native currency.
    /// </summary>
    /// <value>
    /// The manually specified tax amount, used when automatic tax calculation is not applicable (e.g., for foreign currency expenses).
    /// </value>
    [JsonPropertyName("manual_sales_tax_amount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? ManualSalesTaxAmount { get; set; }

    /// <summary>
    /// Gets or sets the rebilling factor for markup or pricing calculations.
    /// </summary>
    /// <value>
    /// The multiplier or percentage used in conjunction with <see cref="RebillType"/> to calculate the rebill amount.
    /// </value>
    /// <seealso cref="RebillType"/>
    [JsonPropertyName("rebill_factor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? RebillFactor { get; set; }

    /// <summary>
    /// Gets or sets the type of vehicle used for mileage expenses.
    /// </summary>
    /// <value>
    /// One of "Car", "Motorcycle", or "Bicycle". Required when <see cref="Category"/> is "Mileage".
    /// </value>
    /// <seealso cref="Mileage"/>
    [JsonPropertyName("vehicle_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? VehicleType { get; set; }

    /// <summary>
    /// Gets or sets the engine size index for the vehicle.
    /// </summary>
    /// <value>
    /// An index value representing the engine displacement category, used for calculating mileage rates.
    /// Required for car mileage expenses.
    /// </value>
    [JsonPropertyName("engine_size_index")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? EngineSizeIndex { get; set; }

    /// <summary>
    /// Gets or sets the engine type index for the vehicle.
    /// </summary>
    /// <value>
    /// An index value representing the engine fuel type (Petrol, Diesel, LPG, Electric variants).
    /// Used for calculating mileage rates and environmental reporting.
    /// </value>
    [JsonPropertyName("engine_type_index")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? EngineTypeIndex { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a VAT receipt is available for this mileage expense.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if a VAT receipt is held for the fuel purchase; otherwise, <see langword="false"/>.
    /// Used for mileage expense VAT reclaim purposes in the UK.
    /// </value>
    [JsonPropertyName("have_vat_receipt")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? HaveVatReceipt { get; set; }

    /// <summary>
    /// Gets or sets the file attachment for this expense.
    /// </summary>
    /// <value>
    /// An <see cref="ExpenseAttachment"/> object containing receipt image or document data (maximum 5MB).
    /// </value>
    /// <seealso cref="ExpenseAttachment"/>
    [JsonPropertyName("attachment")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ExpenseAttachment? Attachment { get; set; }
}