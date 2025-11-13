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
/// <seealso cref="InvoicePayment"/>
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
}