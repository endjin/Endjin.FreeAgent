// <copyright file="BankTransactionExplanation.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents an explanation (categorization) of a bank transaction in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Bank transaction explanations classify and categorize bank transactions to properly reconcile accounts and
/// generate accurate financial reports. Each explanation links a transaction (or portion of a transaction) to
/// an accounting category, invoice, bill, or other financial entity.
/// </para>
/// <para>
/// A single transaction can have multiple explanations to split the amount across different categories or
/// entities. Explanations support multi-currency transactions, tax calculations, rebilling to clients, and
/// attachment of supporting documentation such as receipts.
/// </para>
/// <para>
/// Explanations can be linked to invoices, credit notes, or bills to automatically reconcile payments and
/// track cash flow against outstanding receivables and payables.
/// </para>
/// <para>
/// API Endpoint: /v2/bank_transaction_explanations
/// </para>
/// <para>
/// Minimum Access Level: Banking
/// </para>
/// </remarks>
/// <seealso cref="BankTransaction"/>
/// <seealso cref="BankAccount"/>
/// <seealso cref="Category"/>
/// <seealso cref="Invoice"/>
/// <seealso cref="Bill"/>
/// <seealso cref="CreditNote"/>
public record BankTransactionExplanation
{
    /// <summary>
    /// Gets the unique URI identifier for this bank transaction explanation.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this explanation in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the URI reference to the bank transaction being explained.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.BankTransaction"/> that this explanation categorizes.
    /// This field is required when creating an explanation.
    /// </value>
    [JsonPropertyName("bank_transaction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? BankTransaction { get; init; }

    /// <summary>
    /// Gets the URI reference to the bank account containing the transaction.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.BankAccount"/> where the transaction occurred.
    /// </value>
    [JsonPropertyName("bank_account")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? BankAccount { get; init; }

    /// <summary>
    /// Gets the date of the transaction being explained.
    /// </summary>
    /// <value>
    /// The transaction date in YYYY-MM-DD format.
    /// </value>
    [JsonPropertyName("dated_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? DatedOn { get; init; }

    /// <summary>
    /// Gets the URI reference to the accounting category for this explanation.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Category"/> used to classify this transaction in the chart of accounts.
    /// Required unless the explanation is linked to an invoice, bill, or credit note.
    /// </value>
    [JsonPropertyName("category")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Category { get; init; }

    /// <summary>
    /// Gets the gross value of this explanation including tax.
    /// </summary>
    /// <value>
    /// The total amount being explained, including any applicable sales tax.
    /// This field is required when creating an explanation.
    /// </value>
    [JsonPropertyName("gross_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? GrossValue { get; init; }

    /// <summary>
    /// Gets the sales tax rate applied to this explanation.
    /// </summary>
    /// <value>
    /// The VAT/GST rate as a decimal (e.g., 0.20 for 20% tax). Used to calculate the net and tax
    /// components of <see cref="GrossValue"/>.
    /// </value>
    [JsonPropertyName("sales_tax_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SalesTaxRate { get; init; }

    /// <summary>
    /// Gets the manually specified sales tax amount.
    /// </summary>
    /// <value>
    /// A manually entered tax amount when automatic calculation based on <see cref="SalesTaxRate"/>
    /// is not applicable or needs to be overridden.
    /// </value>
    [JsonPropertyName("manual_sales_tax_amount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? ManualSalesTaxAmount { get; init; }

    /// <summary>
    /// Gets the description or notes for this explanation.
    /// </summary>
    /// <value>
    /// Free-text description providing additional context about this transaction explanation.
    /// </value>
    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the URI reference to an attached supporting document.
    /// </summary>
    /// <value>
    /// The URI of an attachment such as a receipt or invoice document supporting this explanation.
    /// </value>
    [JsonPropertyName("attachment")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Attachment { get; init; }

    /// <summary>
    /// Gets the rebilling method when rebilling this expense to a client.
    /// </summary>
    /// <value>
    /// One of "Cost" (at actual cost), "Markup" (with percentage markup), or "Price" (fixed rebill price).
    /// </value>
    /// <seealso cref="RebillFactor"/>
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
    /// Gets the transaction amount in the original foreign currency.
    /// </summary>
    /// <value>
    /// The amount in the foreign currency before conversion to the company's base currency.
    /// Used for multi-currency transaction tracking.
    /// </value>
    /// <seealso cref="Currency"/>
    [JsonPropertyName("foreign_currency_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? ForeignCurrencyValue { get; init; }

    /// <summary>
    /// Gets the currency code for foreign currency transactions.
    /// </summary>
    /// <value>
    /// A three-letter ISO 4217 currency code (e.g., "GBP", "USD", "EUR") indicating the original
    /// transaction currency when different from the company's base currency.
    /// </value>
    [JsonPropertyName("currency")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Currency { get; init; }

    /// <summary>
    /// Gets the URI reference to the linked invoice for automatic payment reconciliation.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Invoice"/> that this transaction payment relates to.
    /// Used to automatically mark invoices as paid.
    /// </value>
    [JsonPropertyName("linked_invoice")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? LinkedInvoice { get; init; }

    /// <summary>
    /// Gets the URI reference to the linked credit note for automatic refund reconciliation.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.CreditNote"/> that this transaction refund relates to.
    /// Used to automatically mark credit notes as refunded.
    /// </value>
    [JsonPropertyName("linked_credit_note")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? LinkedCreditNote { get; init; }

    /// <summary>
    /// Gets the URI reference to the linked bill for automatic payment reconciliation.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Bill"/> that this transaction payment relates to.
    /// Used to automatically mark bills as paid.
    /// </value>
    [JsonPropertyName("linked_bill")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? LinkedBill { get; init; }

    /// <summary>
    /// Gets the date and time when this explanation was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing the creation timestamp in UTC.
    /// </value>
    [JsonPropertyName("created_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this explanation was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing the last modification timestamp in UTC.
    /// </value>
    [JsonPropertyName("updated_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? UpdatedAt { get; init; }
}