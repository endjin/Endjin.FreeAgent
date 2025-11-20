// <copyright file="RecurringInvoice.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a recurring invoice profile in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Recurring invoices automate the creation of regular invoices for ongoing services or subscriptions.
/// They eliminate the need to manually create repetitive invoices by automatically generating new invoices
/// based on a defined schedule and template.
/// </para>
/// <para>
/// Key features of recurring invoices:
/// - Automated invoice generation on a scheduled frequency (weekly, monthly, quarterly, annually, etc.)
/// - Consistent invoice content using a template (items, quantities, prices)
/// - Defined start and optional end dates for the recurring schedule
/// - Tracks next scheduled invoice creation date
/// - Supports all standard invoice features (multi-currency, tax, payment terms)
/// </para>
/// <para>
/// Typical use cases include monthly retainers, subscription services, rent payments, ongoing support contracts,
/// and any other regularly billed services where the invoice content remains consistent.
/// </para>
/// <para>
/// API Endpoint: /v2/recurring_invoices
/// </para>
/// <para>
/// Minimum Access Level: Estimates and Invoices
/// </para>
/// </remarks>
/// <seealso cref="Invoice"/>
/// <seealso cref="InvoiceItem"/>
/// <seealso cref="Contact"/>
[DebuggerDisplay("ProfileName = {" + nameof(ProfileName) + "}, Status = {" + nameof(RecurringStatus) + "}")]
public record RecurringInvoice
{
    /// <summary>
    /// Gets the unique URI identifier for this recurring invoice profile.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this recurring invoice profile in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the URI reference to the contact (customer) for this recurring invoice.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Contact"/> who will receive the automatically generated invoices.
    /// </value>
    [JsonPropertyName("contact")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Contact { get; init; }

    /// <summary>
    /// Gets the URI reference to the <see cref="Domain.Project"/> associated with this recurring invoice.
    /// </summary>
    /// <value>
    /// The URI of the project that invoices generated from this profile will be billed against, if applicable.
    /// </value>
    /// <seealso cref="Domain.Project"/>
    [JsonPropertyName("project")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Project { get; init; }

    /// <summary>
    /// Gets the display name of the contact associated with this recurring invoice.
    /// </summary>
    /// <value>
    /// The name of the contact as displayed in FreeAgent, typically the company or individual name.
    /// </value>
    [JsonPropertyName("contact_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ContactName { get; init; }

    /// <summary>
    /// Gets the override contact name to display on invoices generated from this profile.
    /// </summary>
    /// <value>
    /// A custom contact name to display instead of the contact's name from their record. Useful for
    /// addressing invoices to specific individuals within an organization.
    /// </value>
    [JsonPropertyName("client_contact_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ClientContactName { get; init; }

    /// <summary>
    /// Gets the date that will be used for invoices generated from this profile.
    /// </summary>
    /// <value>
    /// The date in YYYY-MM-DD format that will be set as the invoice date on generated invoices.
    /// </value>
    [JsonPropertyName("dated_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? DatedOn { get; init; }

    /// <summary>
    /// Gets the frequency at which invoices are automatically generated.
    /// </summary>
    /// <value>
    /// The recurrence pattern. Valid values are: "Weekly", "Two Weekly", "Four Weekly", "Monthly",
    /// "Two Monthly", "Quarterly", "Biannually", "Annually", or "2-Yearly".
    /// </value>
    [JsonPropertyName("frequency")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Frequency { get; init; }

    /// <summary>
    /// Gets the date when this recurring invoice schedule begins.
    /// </summary>
    /// <value>
    /// The first date from which invoices will be generated according to the frequency.
    /// The first invoice will be created on or after this date.
    /// </value>
    [JsonPropertyName("recurring_starts_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? RecurringStartsOn { get; init; }

    /// <summary>
    /// Gets the date when this recurring invoice schedule ends.
    /// </summary>
    /// <value>
    /// The last date for generating invoices. No invoices will be created after this date.
    /// If null or blank, the recurring invoice continues indefinitely until manually stopped.
    /// </value>
    [JsonPropertyName("recurring_end_date")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? RecurringEndDate { get; init; }

    /// <summary>
    /// Gets the date when the next invoice is scheduled to be automatically created.
    /// </summary>
    /// <value>
    /// The upcoming date when FreeAgent will automatically generate the next invoice from this profile.
    /// This date advances after each invoice is created.
    /// </value>
    [JsonPropertyName("next_recurs_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? NextRecursOn { get; init; }

    /// <summary>
    /// Gets the descriptive name for this recurring invoice profile.
    /// </summary>
    /// <value>
    /// A human-readable name identifying this recurring invoice template, such as "Monthly Retainer - ABC Ltd"
    /// or "Quarterly Subscription - XYZ Corp".
    /// </value>
    [JsonPropertyName("profile_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ProfileName { get; init; }

    /// <summary>
    /// Gets the reference number or identifier that will appear on generated invoices.
    /// </summary>
    /// <value>
    /// An optional reference string that helps identify the service or contract, which will be
    /// included on all automatically generated invoices.
    /// </value>
    [JsonPropertyName("reference")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Reference { get; init; }

    /// <summary>
    /// Gets the purchase order reference for invoices generated from this profile.
    /// </summary>
    /// <value>
    /// A custom PO reference to display on generated invoices, overriding the project's PO reference if present.
    /// </value>
    [JsonPropertyName("po_reference")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? PoReference { get; init; }

    /// <summary>
    /// Gets the currency code for invoices generated from this profile.
    /// </summary>
    /// <value>
    /// The three-letter ISO 4217 currency code (e.g., "GBP", "USD", "EUR") that will be used
    /// for all invoices created from this recurring profile.
    /// </value>
    [JsonPropertyName("currency")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Currency { get; init; }

    /// <summary>
    /// Gets the exchange rate for converting the invoice currency to the company's native currency.
    /// </summary>
    /// <value>
    /// The exchange rate used for currency conversion when the invoice currency differs from
    /// the company's native currency.
    /// </value>
    [JsonPropertyName("exchange_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? ExchangeRate { get; init; }

    /// <summary>
    /// Gets the discount percentage applied to invoices generated from this profile.
    /// </summary>
    /// <value>
    /// The percentage discount (e.g., 10 for 10% discount) applied to all generated invoices.
    /// </value>
    [JsonPropertyName("discount_percent")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? DiscountPercent { get; init; }

    /// <summary>
    /// Gets the net value (excluding tax) of each generated invoice.
    /// </summary>
    /// <value>
    /// The total value before sales tax is added, calculated from the invoice items.
    /// </value>
    [JsonPropertyName("net_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? NetValue { get; init; }

    /// <summary>
    /// Gets the sales tax (VAT) value for each generated invoice.
    /// </summary>
    /// <value>
    /// The total sales tax amount calculated from the invoice items.
    /// </value>
    [JsonPropertyName("sales_tax_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SalesTaxValue { get; init; }

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
    /// Gets the total value (including tax) of each generated invoice.
    /// </summary>
    /// <value>
    /// The final amount including all applicable sales taxes that will appear on each automatically generated invoice.
    /// </value>
    [JsonPropertyName("total_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? TotalValue { get; init; }

    /// <summary>
    /// Gets a value indicating whether invoices generated from this profile involve sales tax (VAT/GST).
    /// </summary>
    /// <value>
    /// <see langword="true"/> if sales tax is applicable to generated invoices; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("involves_sales_tax")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? InvolvesSalesTax { get; init; }

    /// <summary>
    /// Gets the recurring status of this recurring invoice profile.
    /// </summary>
    /// <value>
    /// The current status: "Draft" (not yet active) or "Active" (generating invoices on schedule).
    /// Note: The API may also return "Inactive" for deactivated profiles, though this is not documented.
    /// </value>
    [JsonPropertyName("recurring_status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? RecurringStatus { get; init; }

    /// <summary>
    /// Gets a value indicating whether the header should be omitted from generated invoices.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to hide the invoice header; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("omit_header")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? OmitHeader { get; init; }

    /// <summary>
    /// Gets a value indicating whether the project name should be displayed in the Other Information section of generated invoices.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to display the project name on generated invoices; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("show_project_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? ShowProjectName { get; init; }

    /// <summary>
    /// Gets a value indicating whether BIC and IBAN bank details should always be displayed on generated invoices.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to always show BIC and IBAN details; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("always_show_bic_and_iban")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? AlwaysShowBicAndIban { get; init; }

    /// <summary>
    /// Gets the available online payment methods for invoices generated from this profile.
    /// </summary>
    /// <value>
    /// A <see cref="PaymentMethods"/> object indicating which online payment options (PayPal, Stripe, etc.)
    /// are available for paying the generated invoices.
    /// </value>
    [JsonPropertyName("payment_methods")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public PaymentMethods? PaymentMethods { get; init; }

    /// <summary>
    /// Gets the payment terms in days for invoices generated from this profile.
    /// </summary>
    /// <value>
    /// The number of days from the invoice date until payment is due.
    /// Common values are 0 (due on receipt), 7, 14, 30, or 60 days.
    /// </value>
    [JsonPropertyName("payment_terms_in_days")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? PaymentTermsInDays { get; init; }

    /// <summary>
    /// Gets the URI reference to the bank account where payments should be sent for invoices generated from this profile.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="BankAccount"/> for receiving payment on generated invoices.
    /// </value>
    /// <seealso cref="BankAccount"/>
    [JsonPropertyName("bank_account")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? BankAccount { get; init; }

    /// <summary>
    /// Gets the European Community (EC) sales status for invoices generated from this profile.
    /// </summary>
    /// <value>
    /// The EC VAT status such as "EC Services", "EC Goods", or "Non-EC" for cross-border EU transactions.
    /// Affects VAT treatment and reporting requirements.
    /// </value>
    [JsonPropertyName("ec_status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EcStatus { get; init; }

    /// <summary>
    /// Gets the place of supply for EC VAT MOSS (Mini One Stop Shop) purposes.
    /// </summary>
    /// <value>
    /// The country or jurisdiction code determining where VAT is due for digital services
    /// sold to EU customers under the MOSS scheme for invoices generated from this profile.
    /// </value>
    [JsonPropertyName("place_of_supply")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? PlaceOfSupply { get; init; }

    /// <summary>
    /// Gets the Construction Industry Scheme (CIS) deduction rate band for invoices generated from this profile.
    /// </summary>
    /// <value>
    /// The CIS deduction band (e.g., "standard", "higher", "gross") for UK construction industry invoices.
    /// </value>
    [JsonPropertyName("cis_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CisRate { get; init; }

    /// <summary>
    /// Gets the CIS deduction rate percentage for invoices generated from this profile.
    /// </summary>
    /// <value>
    /// The percentage rate at which CIS deductions are calculated as a decimal (e.g., 20 for 20%, 30 for 30%).
    /// Used in Construction Industry Scheme tax calculations.
    /// </value>
    [JsonPropertyName("cis_deduction_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? CisDeductionRate { get; init; }

    /// <summary>
    /// Gets a value indicating whether an email should be sent when invoices are generated from this profile.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to automatically email generated invoices to the client; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("send_new_invoice_emails")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? SendNewInvoiceEmails { get; init; }

    /// <summary>
    /// Gets a value indicating whether automatic reminder emails should be sent for overdue invoices generated from this profile.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to automatically send payment reminders for generated invoices; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("send_reminder_emails")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? SendReminderEmails { get; init; }

    /// <summary>
    /// Gets a value indicating whether automatic thank you emails should be sent when invoices generated from this profile are paid.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to automatically send thank you emails upon payment; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("send_thank_you_emails")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? SendThankYouEmails { get; init; }

    /// <summary>
    /// Gets the list of line items that will appear on each generated invoice.
    /// </summary>
    /// <value>
    /// A list of <see cref="InvoiceItem"/> objects defining the products or services, quantities,
    /// prices, and tax rates that will be used as the template for all automatically created invoices.
    /// </value>
    [JsonPropertyName("invoice_items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<InvoiceItem>? InvoiceItems { get; init; }

    /// <summary>
    /// Gets the comments displayed on invoices generated from this profile.
    /// </summary>
    /// <value>
    /// Additional notes or comments that appear on the invoice document sent to the client.
    /// </value>
    [JsonPropertyName("comments")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Comments { get; init; }

    /// <summary>
    /// Gets the URI reference to the property associated with this recurring invoice.
    /// </summary>
    /// <value>
    /// The URI of the property for UkUnincorporatedLandlord company types.
    /// Required for landlord invoicing on generated invoices.
    /// </value>
    [JsonPropertyName("property")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Property { get; init; }

    /// <summary>
    /// Gets the date and time when this recurring invoice profile was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing when this recurring invoice profile was first set up.
    /// </value>
    [JsonPropertyName("created_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this recurring invoice profile was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the last time this profile was modified (e.g., items changed,
    /// frequency adjusted, or status updated).
    /// </value>
    [JsonPropertyName("updated_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? UpdatedAt { get; init; }
}