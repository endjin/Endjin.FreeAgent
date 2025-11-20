// <copyright file="Bill.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a bill (supplier invoice) in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Bills are financial documents received from suppliers for goods or services purchased.
/// Each bill is associated with a <see cref="Domain.Contact"/> (supplier) and can optionally be linked to
/// a <see cref="Domain.Project"/> for rebilling to clients.
/// </para>
/// <para>
/// Bills support various statuses including Draft, Open, Overdue, Paid, and Part Paid.
/// They can be categorized, support multi-currency transactions, and allow for rebilling
/// to clients with different pricing methods (cost, markup, or fixed price).
/// </para>
/// <para>
/// Bills can be recurring for regular supplier payments and support attachment of receipt documents.
/// </para>
/// <para>
/// API Endpoint: /v2/bills
/// </para>
/// <para>
/// Minimum Access Level: Bills
/// </para>
/// </remarks>
/// <seealso cref="Contact"/>
/// <seealso cref="Project"/>
/// <seealso cref="Category"/>
[DebuggerDisplay("Reference = {" + nameof(Reference) + "}, Status = {" + nameof(Status) + "}")]
public record Bill
{
    /// <summary>
    /// Gets the unique URI identifier for this bill.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this bill in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the URI reference to the supplier contact.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Contact"/> representing the supplier. This field is required when creating a bill.
    /// </value>
    [JsonPropertyName("contact")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Contact { get; init; }

    /// <summary>
    /// Gets the URI reference to the associated project for rebilling.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Project"/> to which this bill should be rebilled, if applicable.
    /// </value>
    [JsonPropertyName("project")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Project { get; init; }

    /// <summary>
    /// Gets the URI reference to the property for landlord accounts.
    /// </summary>
    /// <value>
    /// The URI of the property associated with this bill. This field is required for
    /// UkUnincorporatedLandlord company types.
    /// </value>
    [JsonPropertyName("property")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Property { get; init; }

    /// <summary>
    /// Gets the supplier's reference number for this bill.
    /// </summary>
    /// <value>
    /// The invoice or reference number provided by the supplier.
    /// </value>
    [JsonPropertyName("reference")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Reference { get; init; }

    /// <summary>
    /// Gets the date the bill was issued by the supplier.
    /// </summary>
    /// <value>
    /// The bill date in YYYY-MM-DD format. This field is required when creating a bill.
    /// </value>
    [JsonPropertyName("dated_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? DatedOn { get; init; }

    /// <summary>
    /// Gets the date when payment is due.
    /// </summary>
    /// <value>
    /// The payment due date in YYYY-MM-DD format. This field is required when creating a bill.
    /// </value>
    [JsonPropertyName("due_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? DueOn { get; init; }

    /// <summary>
    /// Gets the date when the bill was paid.
    /// </summary>
    /// <value>
    /// The payment date, or <see langword="null"/> if not yet paid.
    /// </value>
    [JsonPropertyName("paid_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? PaidOn { get; init; }

    /// <summary>
    /// Gets the current status of the bill.
    /// </summary>
    /// <value>
    /// One of "Zero Value", "Open", "Paid", "Overdue", or "Refunded".
    /// This is a read-only field calculated by the system based on payment state.
    /// </value>
    [JsonPropertyName("status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Status { get; init; }

    /// <summary>
    /// Gets the human-readable status with relative due date information.
    /// </summary>
    /// <value>
    /// A descriptive status string that includes the status and relative timing (e.g., "Open and due in 5 days").
    /// This is a read-only field calculated by the system.
    /// </value>
    [JsonPropertyName("long_status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LongStatus { get; init; }

    /// <summary>
    /// Gets the total value of the bill including tax.
    /// </summary>
    /// <value>
    /// The total amount to be paid. This field is calculated from bill items or can be specified directly.
    /// </value>
    [JsonPropertyName("total_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? TotalValue { get; init; }

    /// <summary>
    /// Gets the outstanding amount still due on this bill.
    /// </summary>
    /// <value>
    /// The remaining unpaid amount in the bill's currency. This is a read-only field calculated by the system
    /// as <see cref="TotalValue"/> minus any payments made.
    /// </value>
    [JsonPropertyName("due_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? DueValue { get; init; }

    /// <summary>
    /// Gets the outstanding amount in the company's native currency.
    /// </summary>
    /// <value>
    /// The remaining unpaid amount converted to the company's base currency using <see cref="ExchangeRate"/>.
    /// This is a read-only field calculated by the system.
    /// </value>
    [JsonPropertyName("native_due_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? NativeDueValue { get; init; }

    /// <summary>
    /// Gets the net value of the bill excluding all taxes.
    /// </summary>
    /// <value>
    /// The pre-tax total amount. This is a read-only field calculated by the system from bill items.
    /// </value>
    [JsonPropertyName("net_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? NetValue { get; init; }

    /// <summary>
    /// Gets the total sales tax amount on this bill.
    /// </summary>
    /// <value>
    /// The total VAT/GST amount calculated from all bill items. This is a read-only field.
    /// </value>
    [JsonPropertyName("sales_tax_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SalesTaxValue { get; init; }

    /// <summary>
    /// Gets the total secondary sales tax amount for dual-tax jurisdictions.
    /// </summary>
    /// <value>
    /// The total secondary tax (e.g., PST in Canadian provinces) calculated from all bill items.
    /// This is a read-only field for universal accounts.
    /// </value>
    [JsonPropertyName("second_sales_tax_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SecondSalesTaxValue { get; init; }

    /// <summary>
    /// Gets the EC (European Community) VAT classification for this bill.
    /// </summary>
    /// <value>
    /// <para>
    /// The EC VAT status, used for VAT reporting and compliance within the European Community.
    /// Valid values are:
    /// </para>
    /// <list type="bullet">
    /// <item><description>"UK/Non-EC" - Bill is for UK or non-EC supplier (default)</description></item>
    /// <item><description>"EC Goods" - Bill is for goods from an EC supplier</description></item>
    /// <item><description>"EC Services" - Bill is for services from an EC supplier</description></item>
    /// <item><description>"Reverse Charge" - Reverse charge VAT mechanism applies</description></item>
    /// </list>
    /// <para>
    /// <strong>Important date-based restrictions:</strong>
    /// </para>
    /// <list type="bullet">
    /// <item><description>"EC Goods" and "EC Services" are invalid for bills dated on or after January 1, 2021 in Great Britain</description></item>
    /// <item><description>"Reverse Charge" is only valid for bills dated on or after January 1, 2021</description></item>
    /// </list>
    /// </value>
    [JsonPropertyName("ec_status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EcStatus { get; init; }

    /// <summary>
    /// Gets the total amount already paid towards this bill.
    /// </summary>
    /// <value>
    /// The sum of all payments applied to this bill. Used to calculate <see cref="DueValue"/>.
    /// </value>
    [JsonPropertyName("paid_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? PaidValue { get; init; }

    /// <summary>
    /// Gets a value indicating whether total values include tax.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if <see cref="TotalValue"/> includes tax (for non-native currencies);
    /// <see langword="false"/> if tax-exclusive (for native currency). Defaults based on currency.
    /// </value>
    [JsonPropertyName("input_total_values_inc_tax")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? InputTotalValuesIncTax { get; init; }

    /// <summary>
    /// Gets the rebilling method when rebilling to a project.
    /// </summary>
    /// <value>
    /// One of "cost" (at actual cost), "markup" (with percentage markup), or "price" (fixed rebill price).
    /// </value>
    [JsonPropertyName("rebill_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? RebillType { get; init; }

    /// <summary>
    /// Gets the rebilling factor for markup or pricing calculations.
    /// </summary>
    /// <value>
    /// The multiplier or percentage used with <see cref="RebillType"/> to calculate the rebill amount.
    /// </value>
    [JsonPropertyName("rebill_factor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? RebillFactor { get; init; }

    /// <summary>
    /// Gets the URI reference to the project for rebilling.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Project"/> to which this expense should be rebilled.
    /// This is the same as <see cref="Project"/> field for rebilling scenarios.
    /// </value>
    [JsonPropertyName("rebill_to_project")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? RebillToProject { get; init; }

    /// <summary>
    /// Gets the URI reference to the invoice item this bill was rebilled on.
    /// </summary>
    /// <value>
    /// The URI of the invoice item where this bill expense was rebilled to a client.
    /// This is a read-only field set automatically when a bill is rebilled to an invoice.
    /// </value>
    [JsonPropertyName("rebilled_on_invoice_item")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? RebilledOnInvoiceItem { get; init; }

    /// <summary>
    /// Gets internal comments or notes about this bill.
    /// </summary>
    /// <value>
    /// Free-text notes for internal reference.
    /// </value>
    [JsonPropertyName("comments")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Comments { get; init; }

    /// <summary>
    /// Gets the currency for this bill.
    /// </summary>
    /// <value>
    /// A three-letter ISO 4217 currency code (e.g., "GBP", "USD", "EUR").
    /// </value>
    [JsonPropertyName("currency")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Currency { get; init; }

    /// <summary>
    /// Gets the exchange rate applied when using foreign currency.
    /// </summary>
    /// <value>
    /// The exchange rate from the bill currency to the company's base currency.
    /// </value>
    [JsonPropertyName("exchange_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? ExchangeRate { get; init; }

    /// <summary>
    /// Gets a value indicating whether this bill is paid through hire purchase financing.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if this bill is being paid via a hire purchase agreement;
    /// otherwise, <see langword="false"/>. Defaults to <see langword="false"/>.
    /// </value>
    [JsonPropertyName("is_paid_by_hire_purchase")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsPaidByHirePurchase { get; init; }

    /// <summary>
    /// Gets the URI reference to the recurring bill template.
    /// </summary>
    /// <value>
    /// The URI of the recurring bill template that generated this bill, or <see langword="null"/> for one-time bills.
    /// This is a read-only field that is automatically set when a bill is created from a recurring template.
    /// </value>
    [JsonPropertyName("recurring_bill")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? RecurringBill { get; init; }

    /// <summary>
    /// Gets the recurring schedule for automatic bill creation.
    /// </summary>
    /// <value>
    /// The frequency of recurrence: "Weekly", "Two Weekly", "Four Weekly", "Two Monthly", "Quarterly",
    /// "Biannually", "Annually", or "2-Yearly". Set this to create a recurring bill schedule.
    /// </value>
    [JsonPropertyName("recurring")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Recurring { get; init; }

    /// <summary>
    /// Gets the date when the recurring bill schedule should end.
    /// </summary>
    /// <value>
    /// The end date for recurring bill generation in YYYY-MM-DD format. After this date,
    /// no new bills will be automatically created from this recurring schedule.
    /// </value>
    [JsonPropertyName("recurring_end_date")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? RecurringEndDate { get; init; }

    /// <summary>
    /// Gets the attachment for this bill.
    /// </summary>
    /// <value>
    /// A <see cref="Domain.Attachment"/> object containing the attached receipt or invoice document,
    /// or <see langword="null"/> if no attachment is present.
    /// When reading bills with attachments, this field may contain a reference. When creating
    /// or updating bills, provide the attachment data as a <see cref="Domain.Attachment"/> object.
    /// </value>
    [JsonPropertyName("attachment")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Attachment? Attachment { get; init; }

    /// <summary>
    /// Gets the Construction Industry Scheme (CIS) deduction band for this bill.
    /// </summary>
    /// <value>
    /// One of "cis_gross" (no deduction), "cis_standard" (20% deduction), or "cis_higher" (30% deduction).
    /// Used for UK Construction Industry Scheme tax deductions.
    /// </value>
    [JsonPropertyName("cis_deduction_band")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CisDeductionBand { get; init; }

    /// <summary>
    /// Gets the CIS deduction rate applied to this bill.
    /// </summary>
    /// <value>
    /// The percentage rate for CIS deductions (e.g., 0.20 for 20%, 0.30 for 30%).
    /// This is determined by the <see cref="CisDeductionBand"/>.
    /// </value>
    [JsonPropertyName("cis_deduction_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? CisDeductionRate { get; init; }

    /// <summary>
    /// Gets the total CIS deduction value for this bill.
    /// </summary>
    /// <value>
    /// The calculated amount deducted under the Construction Industry Scheme, based on
    /// <see cref="CisDeductionRate"/> applied to the relevant bill amount.
    /// </value>
    [JsonPropertyName("cis_deduction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? CisDeduction { get; init; }

    /// <summary>
    /// Gets the total CIS deduction suffered on payments made towards this bill.
    /// </summary>
    /// <value>
    /// The sum of CIS deductions that have been withheld from payments made on this bill.
    /// Used to track actual CIS amounts deducted as payments are processed.
    /// </value>
    [JsonPropertyName("cis_deduction_suffered")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? CisDeductionSuffered { get; init; }

    /// <summary>
    /// Gets the collection of line items that make up this bill.
    /// </summary>
    /// <value>
    /// An immutable array of <see cref="BillItem"/> objects detailing each product, service, or charge
    /// included in the bill. Bills support up to 40 line items per bill.
    /// </value>
    [JsonPropertyName("bill_items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ImmutableArray<BillItem> BillItems { get; init; } = [];

    /// <summary>
    /// Gets the date and time when this bill was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the creation timestamp in UTC.
    /// </value>
    [JsonPropertyName("created_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this bill was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the last modification timestamp in UTC.
    /// </value>
    [JsonPropertyName("updated_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? UpdatedAt { get; init; }
}