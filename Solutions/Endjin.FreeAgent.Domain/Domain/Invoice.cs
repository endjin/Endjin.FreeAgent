// <copyright file="Invoice.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents an invoice in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Invoices are financial documents issued to clients requesting payment for goods or services.
/// Each invoice is associated with a <see cref="Contact"/> and optionally with a <see cref="Project"/>.
/// Invoices contain line items, tax calculations, payment tracking, and support various status transitions.
/// </para>
/// <para>
/// Invoice statuses include: Draft, Scheduled To Email, Open, Zero Value, Overdue, Paid, Overpaid,
/// Refunded, Written-off, and Part written-off. Invoices are created as drafts and must be transitioned
/// to other states using the appropriate API endpoints.
/// </para>
/// <para>
/// The API supports comprehensive invoice management including PDF generation, email sending, payment tracking,
/// and integration with online payment providers (PayPal, GoCardless, Stripe, Tyl).
/// </para>
/// <para>
/// API Endpoint: /v2/invoices
/// </para>
/// <para>
/// Minimum Access Level: Estimates and Invoices
/// </para>
/// </remarks>
/// <seealso cref="Contact"/>
/// <seealso cref="Project"/>
/// <seealso cref="InvoiceItem"/>
[DebuggerDisplay("Reference = {" + nameof(Reference) + "}, Status = {" + nameof(Status) + "}")]
public record Invoice
{
    /// <summary>
    /// Gets the unique URI identifier for this invoice.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this invoice in the FreeAgent system.
    /// This value is assigned by the API upon creation and is used to reference the invoice in other resources.
    /// </value>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the URI reference to the <see cref="Domain.Contact"/> associated with this invoice.
    /// </summary>
    /// <value>
    /// The URI of the client to whom this invoice is issued. This field is required when creating an invoice.
    /// </value>
    /// <seealso cref="Domain.Contact"/>
    [JsonPropertyName("contact")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Contact { get; init; }

    /// <summary>
    /// Gets the URI reference to the <see cref="Domain.Project"/> associated with this invoice.
    /// </summary>
    /// <value>
    /// The URI of the project this invoice is billed against, if applicable.
    /// </value>
    /// <seealso cref="Domain.Project"/>
    [JsonPropertyName("project")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Project { get; init; }

    /// <summary>
    /// Gets the invoice reference number.
    /// </summary>
    /// <value>
    /// The unique invoice number used for identification and tracking.
    /// This is automatically generated based on the invoice numbering sequence.
    /// </value>
    [JsonPropertyName("reference")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Reference { get; init; }

    /// <summary>
    /// Gets the date the invoice was issued.
    /// </summary>
    /// <value>
    /// The invoice date in YYYY-MM-DD format.
    /// </value>
    [JsonPropertyName("dated_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? DatedOn { get; init; }

    /// <summary>
    /// Gets the date when payment is due.
    /// </summary>
    /// <value>
    /// The payment due date in YYYY-MM-DD format, typically calculated based on <see cref="DatedOn"/> plus <see cref="PaymentTermsInDays"/>.
    /// </value>
    [JsonPropertyName("due_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? DueOn { get; init; }

    /// <summary>
    /// Gets the date when the invoice was fully paid.
    /// </summary>
    /// <value>
    /// The date when the invoice balance was settled in full, or <see langword="null"/> if not yet paid.
    /// </value>
    [JsonPropertyName("paid_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? PaidOn { get; init; }

    /// <summary>
    /// Gets the current status of the invoice.
    /// </summary>
    /// <value>
    /// One of "Draft", "Scheduled To Email", "Open", "Zero Value", "Overdue", "Paid", "Overpaid",
    /// "Refunded", "Written-off", or "Part written-off".
    /// </value>
    [JsonPropertyName("status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Status { get; init; }

    /// <summary>
    /// Gets the human-readable status with relative due date information.
    /// </summary>
    /// <value>
    /// A descriptive status string that includes relative timing information (e.g., "Open – due in 1 day",
    /// "Overdue by 3 days"). Provides more context than the basic <see cref="Status"/> field.
    /// </value>
    [JsonPropertyName("long_status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LongStatus { get; init; }

    /// <summary>
    /// Gets the currency for this invoice.
    /// </summary>
    /// <value>
    /// A three-letter ISO 4217 currency code (e.g., "GBP", "USD", "EUR").
    /// </value>
    [JsonPropertyName("currency")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Currency { get; init; }

    /// <summary>
    /// Gets the exchange rate applied to this invoice when using a foreign currency.
    /// </summary>
    /// <value>
    /// The exchange rate from the invoice currency to the company's base currency.
    /// </value>
    [JsonPropertyName("exchange_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? ExchangeRate { get; init; }

    /// <summary>
    /// Gets the net value of the invoice before tax.
    /// </summary>
    /// <value>
    /// The total of all line items excluding sales tax.
    /// </value>
    [JsonPropertyName("net_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? NetValue { get; init; }

    /// <summary>
    /// Gets the total value of the invoice including tax.
    /// </summary>
    /// <value>
    /// The final amount to be paid, including all line items and applicable sales tax.
    /// </value>
    [JsonPropertyName("total_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? TotalValue { get; init; }

    /// <summary>
    /// Gets the amount that has been paid towards this invoice.
    /// </summary>
    /// <value>
    /// The sum of all payments received for this invoice.
    /// </value>
    [JsonPropertyName("paid_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? PaidValue { get; init; }

    /// <summary>
    /// Gets the outstanding amount still due on this invoice.
    /// </summary>
    /// <value>
    /// The remaining balance to be paid, calculated as <see cref="TotalValue"/> minus <see cref="PaidValue"/>.
    /// </value>
    [JsonPropertyName("due_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? DueValue { get; init; }

    /// <summary>
    /// Gets the discount amount applied to this invoice.
    /// </summary>
    /// <value>
    /// The monetary discount value.
    /// </value>
    /// <seealso cref="DiscountPercent"/>
    [JsonPropertyName("discount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Discount { get; init; }

    /// <summary>
    /// Gets the discount percentage applied to this invoice.
    /// </summary>
    /// <value>
    /// The percentage discount (e.g., 10 for 10% discount).
    /// </value>
    /// <seealso cref="Discount"/>
    [JsonPropertyName("discount_percent")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? DiscountPercent { get; init; }

    /// <summary>
    /// Gets the number of days allowed for payment from the invoice date.
    /// </summary>
    /// <value>
    /// The payment term period in days, used to calculate <see cref="DueOn"/> from <see cref="DatedOn"/>.
    /// </value>
    [JsonPropertyName("payment_terms_in_days")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? PaymentTermsInDays { get; init; }

    /// <summary>
    /// Gets the descriptive payment terms text.
    /// </summary>
    /// <value>
    /// A human-readable description of the payment terms (e.g., "Net 30 days").
    /// </value>
    [JsonPropertyName("payment_terms")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? PaymentTerms { get; init; }

    /// <summary>
    /// Gets the comments displayed on the invoice.
    /// </summary>
    /// <value>
    /// Additional notes or comments that appear on the invoice document sent to the client.
    /// </value>
    [JsonPropertyName("comments")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Comments { get; init; }

    /// <summary>
    /// Gets internal notes about this invoice.
    /// </summary>
    /// <value>
    /// Private notes for internal use that are not displayed to the client.
    /// </value>
    [JsonPropertyName("notes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Notes { get; init; }

    /// <summary>
    /// Gets a value indicating whether the invoice header should be omitted from the printed document.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to omit the standard header; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("omit_header")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? OmitHeader { get; init; }

    /// <summary>
    /// Gets a value indicating whether BIC and IBAN codes should always be displayed on the invoice.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to always show international bank codes; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("always_show_bic_and_iban")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? AlwaysShowBicAndIban { get; init; }

    /// <summary>
    /// Gets a value indicating whether automatic thank you emails should be sent when the invoice is paid.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to automatically send thank you emails upon payment; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("send_thank_you_emails")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? SendThankYouEmails { get; init; }

    /// <summary>
    /// Gets a value indicating whether automatic reminder emails should be sent for overdue invoices.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to automatically send payment reminders; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("send_reminder_emails")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? SendReminderEmails { get; init; }

    /// <summary>
    /// Gets a value indicating whether an email should be sent when the invoice is first created.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to automatically email the invoice to the client upon creation; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("send_new_invoice_emails")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? SendNewInvoiceEmails { get; init; }

    /// <summary>
    /// Gets the URI reference to the bank account where payments should be sent.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="BankAccount"/> for receiving payment.
    /// </value>
    /// <seealso cref="BankAccount"/>
    [JsonPropertyName("bank_account")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? BankAccount { get; init; }

    /// <summary>
    /// Gets the URI reference to the recurring invoice template if this invoice was generated from one.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="RecurringInvoice"/> that generated this invoice, or <see langword="null"/> for one-time invoices.
    /// </value>
    /// <seealso cref="RecurringInvoice"/>
    [JsonPropertyName("recurring_invoice")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? RecurringInvoice { get; init; }

    /// <summary>
    /// Gets the collection of line items included in this invoice.
    /// </summary>
    /// <value>
    /// A list of <see cref="InvoiceItem"/> objects representing individual goods or services billed.
    /// </value>
    /// <seealso cref="InvoiceItem"/>
    [JsonPropertyName("invoice_items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<InvoiceItem>? InvoiceItems { get; init; }

    /// <summary>
    /// Gets the available online payment methods for this invoice.
    /// </summary>
    /// <value>
    /// A <see cref="Domain.PaymentMethods"/> object indicating which online payment options (PayPal, GoCardless, Stripe, Tyl) are available.
    /// </value>
    /// <seealso cref="Domain.PaymentMethods"/>
    [JsonPropertyName("payment_methods")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public PaymentMethods? PaymentMethods { get; init; }

    /// <summary>
    /// Gets the date and time when this invoice was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing the creation timestamp in UTC.
    /// </value>
    [JsonPropertyName("created_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this invoice was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing the last modification timestamp in UTC.
    /// </value>
    [JsonPropertyName("updated_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? UpdatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this invoice was sent to the client.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing when the invoice was emailed, or <see langword="null"/> if not yet sent.
    /// </value>
    [JsonPropertyName("sent_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? SentAt { get; init; }

    /// <summary>
    /// Gets the collection of dates when reminder emails were sent for this invoice.
    /// </summary>
    /// <value>
    /// A list of <see cref="DateTime"/> values representing each reminder email timestamp.
    /// </value>
    [JsonPropertyName("reminders_sent")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<DateTime>? RemindersSent { get; init; }

    /// <summary>
    /// Gets the date when this invoice was written off.
    /// </summary>
    /// <value>
    /// The date when the outstanding balance was written off as uncollectible, or <see langword="null"/> if not written off.
    /// </value>
    [JsonPropertyName("written_off_date")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? WrittenOffDate { get; init; }

    /// <summary>
    /// Gets a value indicating whether this invoice involves sales tax (VAT/GST).
    /// </summary>
    /// <value>
    /// <see langword="true"/> if sales tax is applicable to this invoice; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("involves_sales_tax")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? InvolvesSalesTax { get; init; }

    /// <summary>
    /// Gets the total sales tax (VAT/GST) amount on this invoice.
    /// </summary>
    /// <value>
    /// The calculated total of all sales tax charged on the invoice line items.
    /// </value>
    [JsonPropertyName("sales_tax_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SalesTaxValue { get; init; }

    /// <summary>
    /// Gets the EC (European Community) VAT status for this invoice.
    /// </summary>
    /// <value>
    /// One of "UK/Non-EC", "EC Goods", "EC Services", or other EC-related status values.
    /// Used for VAT reporting and compliance within the European Community.
    /// </value>
    [JsonPropertyName("ec_status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EcStatus { get; init; }

    /// <summary>
    /// Gets the online payment URL for this invoice.
    /// </summary>
    /// <value>
    /// A URL that clients can use to pay the invoice online through integrated payment providers
    /// such as PayPal, Stripe, GoCardless, or Tyl. Returns <see langword="null"/> if no online payment is configured.
    /// </value>
    [JsonPropertyName("payment_url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? PaymentUrl { get; init; }

    /// <summary>
    /// Gets the URI reference to the property associated with this invoice.
    /// </summary>
    /// <value>
    /// The URI of the property for UkUnincorporatedLandlord company types. Required for landlord invoicing.
    /// </value>
    [JsonPropertyName("property")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Property { get; init; }

    /// <summary>
    /// Gets the timeslip grouping option for this invoice.
    /// </summary>
    /// <value>
    /// Specifies how timeslips should be grouped when included in the invoice (e.g., "individual", "grouped", "summary").
    /// </value>
    [JsonPropertyName("include_timeslips")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? IncludeTimeslips { get; init; }

    /// <summary>
    /// Gets the expense grouping option for this invoice.
    /// </summary>
    /// <value>
    /// Specifies how expenses should be grouped when included in the invoice (e.g., "individual", "grouped", "summary").
    /// </value>
    [JsonPropertyName("include_expenses")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? IncludeExpenses { get; init; }

    /// <summary>
    /// Gets the estimate grouping option for this invoice.
    /// </summary>
    /// <value>
    /// Specifies how estimates should be included when converting to an invoice.
    /// </value>
    [JsonPropertyName("include_estimates")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? IncludeEstimates { get; init; }

    /// <summary>
    /// Gets the Construction Industry Scheme (CIS) deduction rate band for this invoice.
    /// </summary>
    /// <value>
    /// The CIS deduction band (e.g., "standard", "higher", "gross") for UK construction industry invoices.
    /// </value>
    [JsonPropertyName("cis_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CisRate { get; init; }

    /// <summary>
    /// Gets the total CIS deduction amount for this invoice.
    /// </summary>
    /// <value>
    /// The total Construction Industry Scheme deduction withheld from this invoice under UK tax regulations.
    /// </value>
    [JsonPropertyName("cis_deduction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? CisDeduction { get; init; }

    /// <summary>
    /// Gets the CIS deduction rate percentage for this invoice.
    /// </summary>
    /// <value>
    /// The percentage rate at which CIS deductions are calculated (e.g., 0.20 for 20%, 0.30 for 30%).
    /// Used in Construction Industry Scheme tax calculations.
    /// </value>
    [JsonPropertyName("cis_deduction_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? CisDeductionRate { get; init; }

    /// <summary>
    /// Gets the prepaid CIS deduction amount for this invoice.
    /// </summary>
    /// <value>
    /// The amount of CIS deduction that has already been paid or suffered prior to this invoice.
    /// </value>
    [JsonPropertyName("cis_deduction_suffered")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? CisDeductionSuffered { get; init; }

    /// <summary>
    /// Gets the override contact name to display on this invoice.
    /// </summary>
    /// <value>
    /// A custom contact name to display instead of the contact's name from their record. Useful for
    /// addressing invoices to specific individuals within an organization.
    /// </value>
    [JsonPropertyName("client_contact_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ClientContactName { get; init; }

    /// <summary>
    /// Gets the display name from the associated contact record.
    /// </summary>
    /// <value>
    /// The contact's name as stored in their contact record. This is a read-only field derived from
    /// the associated <see cref="Contact"/>.
    /// </value>
    [JsonPropertyName("contact_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ContactName { get; init; }

    /// <summary>
    /// Gets the purchase order reference override for this invoice.
    /// </summary>
    /// <value>
    /// A custom PO reference to display on this invoice, overriding the project's PO reference if present.
    /// </value>
    [JsonPropertyName("po_reference")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? PoReference { get; init; }

    /// <summary>
    /// Gets a value indicating whether the project name should be displayed in the Other Information section.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to display the project name on the invoice; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("show_project_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? ShowProjectName { get; init; }

    /// <summary>
    /// Gets the second sales tax amount for jurisdictions with dual tax systems.
    /// </summary>
    /// <value>
    /// The calculated secondary tax amount (e.g., PST in Canadian provinces) for universal accounts
    /// that operate in multi-tax jurisdictions.
    /// </value>
    [JsonPropertyName("second_sales_tax_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SecondSalesTaxValue { get; init; }

    /// <summary>
    /// Gets the place of supply for EC VAT MOSS (Mini One Stop Shop) purposes.
    /// </summary>
    /// <value>
    /// The country or jurisdiction code determining where VAT is due for digital services
    /// sold to EU customers under the MOSS scheme.
    /// </value>
    [JsonPropertyName("place_of_supply")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? PlaceOfSupply { get; init; }

    /// <summary>
    /// Gets a value indicating whether this invoice uses interim UK VAT accounting.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if using provisional VAT registration where VAT is accounted for when cash
    /// is received rather than when invoiced; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("is_interim_uk_vat")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsInterimUkVat { get; init; }
}