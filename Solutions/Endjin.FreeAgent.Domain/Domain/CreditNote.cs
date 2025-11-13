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
/// Minimum Access Level: Invoices
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
    /// Gets the URI reference to the original invoice being credited.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Invoice"/> that this credit note relates to, if applicable.
    /// Used to track which invoice is being partially or fully refunded.
    /// </value>
    [JsonPropertyName("invoice")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Invoice { get; init; }

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
    /// One of "Draft", "Sent", or "Refunded", indicating the current processing stage.
    /// </value>
    [JsonPropertyName("status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Status { get; init; }

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
    /// This field is required when creating a credit note.
    /// </value>
    [JsonPropertyName("total_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? TotalValue { get; init; }

    /// <summary>
    /// Gets the reason or explanation for issuing this credit note.
    /// </summary>
    /// <value>
    /// A text description explaining why the credit note was issued, such as "Returned goods",
    /// "Invoice correction", or "Customer overpayment".
    /// </value>
    [JsonPropertyName("reason")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Reason { get; init; }

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
    /// A <see cref="DateTime"/> representing the creation timestamp in UTC.
    /// </value>
    [JsonPropertyName("created_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this credit note was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing the last modification timestamp in UTC.
    /// </value>
    [JsonPropertyName("updated_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? UpdatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this credit note was sent to the customer.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing when the credit note was emailed or marked as sent,
    /// or <see langword="null"/> if still in draft status.
    /// </value>
    [JsonPropertyName("sent_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? SentAt { get; init; }
}