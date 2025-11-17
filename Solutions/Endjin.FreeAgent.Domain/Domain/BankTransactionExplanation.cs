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
    /// Gets the type of this bank transaction explanation.
    /// </summary>
    /// <value>
    /// A read-only string indicating the explanation type such as "Payment", "Invoice Receipt",
    /// "Bill Payment", "Bank Transfer", etc. This field is set by the system.
    /// </value>
    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Type { get; init; }

    /// <summary>
    /// Gets the calculated sales tax value based on the gross value and tax rate.
    /// </summary>
    /// <value>
    /// The automatically calculated tax amount. This is a read-only field computed by the system.
    /// </value>
    [JsonPropertyName("sales_tax_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SalesTaxValue { get; init; }

    /// <summary>
    /// Gets the sales tax status for this explanation.
    /// </summary>
    /// <value>
    /// One of "TAXABLE", "EXEMPT", or "OUT_OF_SCOPE" indicating the tax treatment.
    /// </value>
    [JsonPropertyName("sales_tax_status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SalesTaxStatus { get; init; }

    /// <summary>
    /// Gets the EC (European Community) status for VAT purposes.
    /// </summary>
    /// <value>
    /// VAT classification such as "UK/Non-EC", "EC Goods", "EC Services", "Reverse Charge", or "EC VAT MOSS".
    /// Note: EC Goods/Services are invalid for transactions dated 1/1/2021+ in Great Britain (non-Northern Ireland).
    /// Reverse Charge is only valid for transactions dated 1/1/2021 or later.
    /// </value>
    [JsonPropertyName("ec_status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EcStatus { get; init; }

    /// <summary>
    /// Gets the second sales tax rate for universal accounts.
    /// </summary>
    /// <value>
    /// An additional tax rate as a decimal for jurisdictions requiring multiple tax calculations.
    /// Only applicable to universal accounts.
    /// </value>
    [JsonPropertyName("second_sales_tax_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SecondSalesTaxRate { get; init; }

    /// <summary>
    /// Gets the second sales tax value for universal accounts.
    /// </summary>
    /// <value>
    /// The calculated amount for the second tax rate. Only applicable to universal accounts.
    /// </value>
    [JsonPropertyName("second_sales_tax_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SecondSalesTaxValue { get; init; }

    /// <summary>
    /// Gets a value indicating whether this explanation is marked for review.
    /// </summary>
    /// <value>
    /// <c>true</c> if the explanation requires approval or review; otherwise, <c>false</c>.
    /// </value>
    [JsonPropertyName("marked_for_review")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? MarkedForReview { get; init; }

    /// <summary>
    /// Gets a value indicating whether this explanation is locked from modification.
    /// </summary>
    /// <value>
    /// <c>true</c> if the explanation cannot be modified; otherwise, <c>false</c>.
    /// </value>
    [JsonPropertyName("is_locked")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsLocked { get; init; }

    /// <summary>
    /// Gets a value indicating whether this explanation can be deleted.
    /// </summary>
    /// <value>
    /// <c>true</c> if the explanation can be deleted; otherwise, <c>false</c>.
    /// </value>
    [JsonPropertyName("is_deletable")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsDeletable { get; init; }

    /// <summary>
    /// Gets a value indicating whether this is an incoming money transaction.
    /// </summary>
    /// <value>
    /// <c>true</c> if money is coming into the account; otherwise, <c>false</c>.
    /// </value>
    [JsonPropertyName("is_money_in")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsMoneyIn { get; init; }

    /// <summary>
    /// Gets a value indicating whether this is an outgoing money transaction.
    /// </summary>
    /// <value>
    /// <c>true</c> if money is going out of the account; otherwise, <c>false</c>.
    /// </value>
    [JsonPropertyName("is_money_out")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsMoneyOut { get; init; }

    /// <summary>
    /// Gets a value indicating whether this is a payment to a user.
    /// </summary>
    /// <value>
    /// <c>true</c> if this represents a payment to a user; otherwise, <c>false</c>.
    /// </value>
    [JsonPropertyName("is_money_paid_to_user")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsMoneyPaidToUser { get; init; }

    /// <summary>
    /// Gets the URI reference to the project for payment or refund rebilling.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Project"/> to which this expense should be rebilled.
    /// Used with <see cref="RebillType"/> and <see cref="RebillFactor"/>.
    /// </value>
    [JsonPropertyName("project")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Project { get; init; }

    /// <summary>
    /// Gets the URI reference to the paid invoice for Invoice Receipt or Credit Note Refund explanations.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Invoice"/> that this transaction payment relates to.
    /// Used for Invoice Receipt and Credit Note Refund explanation types.
    /// </value>
    [JsonPropertyName("paid_invoice")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? PaidInvoice { get; init; }

    /// <summary>
    /// Gets the URI reference to the paid bill for Bill Payment or Bill Refund explanations.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Bill"/> that this transaction payment relates to.
    /// Used for Bill Payment and Bill Refund explanation types.
    /// </value>
    [JsonPropertyName("paid_bill")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? PaidBill { get; init; }

    /// <summary>
    /// Gets the URI reference to the user receiving payment.
    /// </summary>
    /// <value>
    /// The URI of the user being paid. For smart user payments, specify this field without
    /// <see cref="Category"/> to have the system automatically determine the appropriate category.
    /// </value>
    [JsonPropertyName("paid_user")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? PaidUser { get; init; }

    /// <summary>
    /// Gets the URI reference to the target or source bank account for bank transfer explanations.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.BankAccount"/> that is the target (for outgoing transfers) or
    /// source (for incoming transfers) of the bank transfer. Used for Bank Transfer explanation types.
    /// </value>
    [JsonPropertyName("transfer_bank_account")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? TransferBankAccount { get; init; }

    /// <summary>
    /// Gets the URI reference to the stock item for stock transaction explanations.
    /// </summary>
    /// <value>
    /// The URI of the stock item being purchased or sold. Used for stock-related explanations.
    /// </value>
    [JsonPropertyName("stock_item")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? StockItem { get; init; }

    /// <summary>
    /// Gets the quantity change for stock transaction explanations.
    /// </summary>
    /// <value>
    /// An integer indicating the change in stock quantity (positive for purchases, negative for sales).
    /// Used with <see cref="StockItem"/>.
    /// </value>
    [JsonPropertyName("stock_altering_quantity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? StockAlteringQuantity { get; init; }

    /// <summary>
    /// Gets the URI reference to the capital asset (read-only).
    /// </summary>
    /// <value>
    /// The URI of the capital asset associated with this explanation. This is a read-only field
    /// set by the system for capital asset transactions.
    /// </value>
    [JsonPropertyName("capital_asset")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? CapitalAsset { get; init; }

    /// <summary>
    /// Gets the URI reference to the asset being disposed.
    /// </summary>
    /// <value>
    /// The URI of the capital asset being disposed. Required when creating an asset disposal explanation.
    /// </value>
    [JsonPropertyName("disposed_asset")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? DisposedAsset { get; init; }

    /// <summary>
    /// Gets the depreciation period in years for capital assets.
    /// </summary>
    /// <value>
    /// The number of years over which the asset should be depreciated.
    /// Note: This field is deprecated in the FreeAgent API.
    /// </value>
    [JsonPropertyName("asset_life_years")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Obsolete("This field is deprecated in the FreeAgent API.")]
    public int? AssetLifeYears { get; init; }

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