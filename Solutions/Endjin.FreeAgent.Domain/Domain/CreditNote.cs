// <copyright file="CreditNote.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a credit note in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Credit notes are financial documents issued to customers to cancel or reduce the amount owed on an invoice.
/// They can be created for full or partial invoice refunds, returned goods, overpayments, or invoice corrections.
/// Each credit note is associated with a <see cref="Domain.Contact"/> (customer) and optionally linked to the
/// original <see cref="Domain.Invoice"/> it's crediting.
/// </para>
/// <para>
/// Credit notes support various statuses including Draft, Sent, and Refunded. They can be issued in multiple
/// currencies with automatic exchange rate conversion, and can include multiple line items detailing what is
/// being credited. The credit note can reduce the customer's outstanding balance or be issued as a cash refund.
/// </para>
/// <para>
/// Credit notes can be linked to projects for accurate project profitability tracking and support attachment
/// of supporting documentation.
/// </para>
/// <para>
/// API Endpoint: /v2/credit_notes
/// </para>
/// <para>
/// Minimum Access Level: Estimates and Invoices
/// </para>
/// </remarks>
/// <seealso cref="Invoice"/>
/// <seealso cref="Contact"/>
/// <seealso cref="Project"/>
/// <seealso cref="CreditNoteItem"/>
[DebuggerDisplay("Reference = {" + nameof(Reference) + "}, Status = {" + nameof(Status) + "}")]
public record CreditNote
{
    /// <summary>
    /// Gets the unique URI identifier for this credit note.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this credit note in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the URI reference to the customer contact.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Contact"/> representing the customer receiving this credit note.
    /// This field is required when creating a credit note.
    /// </value>
    [JsonPropertyName("contact")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Contact { get; init; }

    /// <summary>
    /// Gets the URI reference to the associated project.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Project"/> to which this credit note relates.
    /// Used for project-based revenue and profitability tracking.
    /// </value>
    [JsonPropertyName("project")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Project { get; init; }

    /// <summary>
    /// Gets the URI reference to the property (for UK Unincorporated Landlord entities).
    /// </summary>
    /// <value>
    /// The URI of the property when the company type is UkUnincorporatedLandlord.
    /// </value>
    [JsonPropertyName("property")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Property { get; init; }

    /// <summary>
    /// Gets the credit note reference number.
    /// </summary>
    /// <value>
    /// The unique reference or identification number for this credit note, typically auto-generated
    /// according to the company's numbering scheme.
    /// </value>
    [JsonPropertyName("reference")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Reference { get; init; }

    /// <summary>
    /// Gets the date the credit note was issued.
    /// </summary>
    /// <value>
    /// The credit note issue date in YYYY-MM-DD format. This field is required when creating a credit note.
    /// </value>
    [JsonPropertyName("dated_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? DatedOn { get; init; }

    /// <summary>
    /// Gets the due date for the credit note.
    /// </summary>
    /// <value>
    /// The date by which the credit note should be settled.
    /// </value>
    [JsonPropertyName("due_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? DueOn { get; init; }

    /// <summary>
    /// Gets the payment terms in days.
    /// </summary>
    /// <value>
    /// The number of days from the issue date until payment is due. This field is required when creating
    /// a credit note. Set to 0 for "Due on Receipt".
    /// </value>
    [JsonPropertyName("payment_terms_in_days")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? PaymentTermsInDays { get; init; }

    /// <summary>
    /// Gets the date when the credit note was refunded to the customer.
    /// </summary>
    /// <value>
    /// The refund date, or <see langword="null"/> if not yet refunded.
    /// </value>
    [JsonPropertyName("refunded_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? RefundedOn { get; init; }

    /// <summary>
    /// Gets the current status of the credit note.
    /// </summary>
    /// <value>
    /// One of "Draft", "Open", "Overdue", "Refunded", or "Written-off", indicating the current processing stage.
    /// </value>
    [JsonPropertyName("status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Status { get; init; }

    /// <summary>
    /// Gets the detailed status description of the credit note.
    /// </summary>
    /// <value>
    /// A human-readable description of the current status with due date context (e.g., "Due in 14 days").
    /// This field is read-only and returned by the API.
    /// </value>
    [JsonPropertyName("long_status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LongStatus { get; init; }

    /// <summary>
    /// Gets the currency for this credit note.
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
    /// The exchange rate from the credit note currency to the company's base currency.
    /// </value>
    [JsonPropertyName("exchange_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? ExchangeRate { get; init; }

    /// <summary>
    /// Gets the net value of the credit note before tax.
    /// </summary>
    /// <value>
    /// The total credit amount excluding sales tax/VAT.
    /// </value>
    [JsonPropertyName("net_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? NetValue { get; init; }

    /// <summary>
    /// Gets the total value of the credit note including tax.
    /// </summary>
    /// <value>
    /// The total credit amount including all applicable sales tax/VAT.
    /// This field is read-only and calculated automatically by the API.
    /// </value>
    [JsonPropertyName("total_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? TotalValue { get; init; }

    /// <summary>
    /// Gets the amount of sales tax/VAT on the credit note.
    /// </summary>
    /// <value>
    /// The total sales tax amount.
    /// </value>
    [JsonPropertyName("sales_tax_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SalesTaxValue { get; init; }

    /// <summary>
    /// Gets the second sales tax amount (for jurisdictions with multiple tax rates).
    /// </summary>
    /// <value>
    /// The secondary sales tax amount, if applicable.
    /// </value>
    [JsonPropertyName("second_sales_tax_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SecondSalesTaxValue { get; init; }

    /// <summary>
    /// Gets the total refunded value of the credit note.
    /// </summary>
    /// <value>
    /// The amount that has been refunded.
    /// </value>
    [JsonPropertyName("refunded_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? RefundedValue { get; init; }

    /// <summary>
    /// Gets the outstanding value due on the credit note.
    /// </summary>
    /// <value>
    /// The amount still owed.
    /// </value>
    [JsonPropertyName("due_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? DueValue { get; init; }

    /// <summary>
    /// Gets the discount percentage applied to the credit note.
    /// </summary>
    /// <value>
    /// The percentage discount applied.
    /// </value>
    [JsonPropertyName("discount_percent")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? DiscountPercent { get; init; }

    /// <summary>
    /// Gets the CIS rate for Construction Industry Scheme.
    /// </summary>
    /// <value>
    /// The CIS rate identifier, or <see langword="null"/> if not applicable.
    /// </value>
    [JsonPropertyName("cis_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CisRate { get; init; }

    /// <summary>
    /// Gets the CIS deduction rate percentage.
    /// </summary>
    /// <value>
    /// The percentage rate for CIS deductions.
    /// </value>
    [JsonPropertyName("cis_deduction_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? CisDeductionRate { get; init; }

    /// <summary>
    /// Gets the CIS deduction amount.
    /// </summary>
    /// <value>
    /// The amount deducted under CIS.
    /// </value>
    [JsonPropertyName("cis_deduction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? CisDeduction { get; init; }

    /// <summary>
    /// Gets the CIS deduction suffered amount.
    /// </summary>
    /// <value>
    /// The CIS deduction amount that has been suffered.
    /// </value>
    [JsonPropertyName("cis_deduction_suffered")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? CisDeductionSuffered { get; init; }

    /// <summary>
    /// Gets the comments or additional notes on the credit note.
    /// </summary>
    /// <value>
    /// Free-text comments visible on the credit note.
    /// </value>
    [JsonPropertyName("comments")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Comments { get; init; }

    /// <summary>
    /// Gets the client contact name.
    /// </summary>
    /// <value>
    /// The name of the specific contact person at the client.
    /// </value>
    [JsonPropertyName("client_contact_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ClientContactName { get; init; }

    /// <summary>
    /// Gets the payment terms description.
    /// </summary>
    /// <value>
    /// A text description of the payment terms.
    /// </value>
    [JsonPropertyName("payment_terms")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? PaymentTerms { get; init; }

    /// <summary>
    /// Gets the purchase order reference.
    /// </summary>
    /// <value>
    /// The client's purchase order reference number.
    /// </value>
    [JsonPropertyName("po_reference")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? PoReference { get; init; }

    /// <summary>
    /// Gets the URI of the bank account for remittance advice.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="BankAccount"/> whose details are displayed for remittance advice.
    /// </value>
    [JsonPropertyName("bank_account")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? BankAccount { get; init; }

    /// <summary>
    /// Gets whether to omit the header on the credit note.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to omit the header; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("omit_header")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? OmitHeader { get; init; }

    /// <summary>
    /// Gets whether to show the project name on the credit note.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to display the project name; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("show_project_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? ShowProjectName { get; init; }

    /// <summary>
    /// Gets the EC Status for European Community transactions.
    /// </summary>
    /// <value>
    /// The EC status identifier for VAT purposes.
    /// </value>
    [JsonPropertyName("ec_status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EcStatus { get; init; }

    /// <summary>
    /// Gets the place of supply for tax purposes.
    /// </summary>
    /// <value>
    /// The location where the supply is deemed to take place for tax calculation.
    /// </value>
    [JsonPropertyName("place_of_supply")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? PlaceOfSupply { get; init; }

    /// <summary>
    /// Gets whether the credit note involves sales tax.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if sales tax applies; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("involves_sales_tax")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? InvolvesSalesTax { get; init; }

    /// <summary>
    /// Gets whether this is an interim UK VAT credit note.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if this is an interim VAT credit note; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("is_interim_uk_vat")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsInterimUkVat { get; init; }

    /// <summary>
    /// Gets the date when the credit note was written off.
    /// </summary>
    /// <value>
    /// The write-off date, or <see langword="null"/> if not written off.
    /// </value>
    [JsonPropertyName("written_off_date")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? WrittenOffDate { get; init; }

    /// <summary>
    /// Gets the collection of line items that make up this credit note.
    /// </summary>
    /// <value>
    /// A list of <see cref="CreditNoteItem"/> objects detailing each product or service being credited.
    /// </value>
    [JsonPropertyName("credit_note_items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<CreditNoteItem>? CreditNoteItems { get; init; }

    /// <summary>
    /// Gets the date and time when this credit note was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the creation timestamp in UTC.
    /// </value>
    [JsonPropertyName("created_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this credit note was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the last modification timestamp in UTC.
    /// </value>
    [JsonPropertyName("updated_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? UpdatedAt { get; init; }
}