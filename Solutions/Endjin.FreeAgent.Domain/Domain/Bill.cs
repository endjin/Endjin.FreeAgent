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
    /// One of "Draft", "Open", "Overdue", "Paid", or "Part Paid".
    /// </value>
    [JsonPropertyName("status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Status { get; init; }

    /// <summary>
    /// Gets the URI reference to the accounting category.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Category"/> for classifying this expense.
    /// </value>
    [JsonPropertyName("category")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Category { get; init; }

    /// <summary>
    /// Gets the total value of the bill including tax.
    /// </summary>
    /// <value>
    /// The total amount to be paid. This field is required when creating a bill.
    /// </value>
    [JsonPropertyName("total_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? TotalValue { get; init; }

    /// <summary>
    /// Gets the sales tax rate applied to this bill.
    /// </summary>
    /// <value>
    /// The VAT/GST rate as a decimal (e.g., 0.20 for 20% tax).
    /// </value>
    [JsonPropertyName("sales_tax_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SalesTaxRate { get; init; }

    /// <summary>
    /// Gets the manually specified sales tax amount.
    /// </summary>
    /// <value>
    /// A manually entered tax amount when automatic calculation is not applicable.
    /// </value>
    [JsonPropertyName("manual_sales_tax_amount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? ManualSalesTaxAmount { get; init; }

    /// <summary>
    /// Gets the rebilling method when rebilling to a project.
    /// </summary>
    /// <value>
    /// One of "Cost" (at actual cost), "Markup" (with percentage markup), or "Price" (fixed rebill price).
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
    /// Gets the URI reference to the recurring bill template.
    /// </summary>
    /// <value>
    /// The URI of the recurring bill that generated this bill, or <see langword="null"/> for one-time bills.
    /// </value>
    [JsonPropertyName("recurring_bill")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? RecurringBill { get; init; }

    /// <summary>
    /// Gets the URI reference to the attached document.
    /// </summary>
    /// <value>
    /// The URI of the receipt or invoice document attachment.
    /// </value>
    [JsonPropertyName("attachment")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Attachment { get; init; }

    /// <summary>
    /// Gets the date and time when this bill was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing the creation timestamp in UTC.
    /// </value>
    [JsonPropertyName("created_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this bill was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing the last modification timestamp in UTC.
    /// </value>
    [JsonPropertyName("updated_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? UpdatedAt { get; init; }
}