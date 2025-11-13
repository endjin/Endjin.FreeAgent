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
/// Minimum Access Level: Standard
/// </para>
/// </remarks>
/// <seealso cref="Invoice"/>
/// <seealso cref="InvoiceItem"/>
/// <seealso cref="Contact"/>
public record RecurringInvoice
{
    /// <summary>
    /// Gets the unique URI identifier for this recurring invoice profile.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this recurring invoice profile in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the URI reference to the contact (customer) for this recurring invoice.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Contact"/> who will receive the automatically generated invoices.
    /// </value>
    [JsonPropertyName("contact")]
    public Uri? Contact { get; init; }

    /// <summary>
    /// Gets the frequency at which invoices are automatically generated.
    /// </summary>
    /// <value>
    /// The recurrence pattern such as "Weekly", "Monthly", "Quarterly", "Annually", "Bi-Annually",
    /// or custom frequencies like "Every 2 Weeks" or "Every 3 Months".
    /// </value>
    [JsonPropertyName("frequency")]
    public string? Frequency { get; init; }

    /// <summary>
    /// Gets the date when this recurring invoice schedule begins.
    /// </summary>
    /// <value>
    /// The first date from which invoices will be generated according to the frequency.
    /// The first invoice will be created on or after this date.
    /// </value>
    [JsonPropertyName("recurring_starts_on")]
    public DateOnly? RecurringStartsOn { get; init; }

    /// <summary>
    /// Gets the date when this recurring invoice schedule ends.
    /// </summary>
    /// <value>
    /// The last date for generating invoices. No invoices will be created after this date.
    /// If null, the recurring invoice continues indefinitely until manually stopped.
    /// </value>
    [JsonPropertyName("recurring_ends_on")]
    public DateOnly? RecurringEndsOn { get; init; }

    /// <summary>
    /// Gets the date when the next invoice is scheduled to be automatically created.
    /// </summary>
    /// <value>
    /// The upcoming date when FreeAgent will automatically generate the next invoice from this profile.
    /// This date advances after each invoice is created.
    /// </value>
    [JsonPropertyName("next_recurs_on")]
    public DateOnly? NextRecursOn { get; init; }

    /// <summary>
    /// Gets the descriptive name for this recurring invoice profile.
    /// </summary>
    /// <value>
    /// A human-readable name identifying this recurring invoice template, such as "Monthly Retainer - ABC Ltd"
    /// or "Quarterly Subscription - XYZ Corp".
    /// </value>
    [JsonPropertyName("profile_name")]
    public string? ProfileName { get; init; }

    /// <summary>
    /// Gets the reference number or identifier that will appear on generated invoices.
    /// </summary>
    /// <value>
    /// An optional reference string that helps identify the service or contract, which will be
    /// included on all automatically generated invoices.
    /// </value>
    [JsonPropertyName("reference")]
    public string? Reference { get; init; }

    /// <summary>
    /// Gets the currency code for invoices generated from this profile.
    /// </summary>
    /// <value>
    /// The three-letter ISO 4217 currency code (e.g., "GBP", "USD", "EUR") that will be used
    /// for all invoices created from this recurring profile.
    /// </value>
    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    /// <summary>
    /// Gets the net value (excluding tax) of each generated invoice.
    /// </summary>
    /// <value>
    /// The total value before sales tax is added, calculated from the invoice items.
    /// </value>
    [JsonPropertyName("net_value")]
    public decimal? NetValue { get; init; }

    /// <summary>
    /// Gets the total value (including tax) of each generated invoice.
    /// </summary>
    /// <value>
    /// The final amount including all applicable sales taxes that will appear on each automatically generated invoice.
    /// </value>
    [JsonPropertyName("total_value")]
    public decimal? TotalValue { get; init; }

    /// <summary>
    /// Gets the status of this recurring invoice profile.
    /// </summary>
    /// <value>
    /// The current status such as "Active", "Paused", or "Cancelled" indicating whether invoices
    /// are currently being generated from this profile.
    /// </value>
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    /// <summary>
    /// Gets the payment terms in days for invoices generated from this profile.
    /// </summary>
    /// <value>
    /// The number of days from the invoice date until payment is due.
    /// Common values are 0 (due on receipt), 7, 14, 30, or 60 days.
    /// </value>
    [JsonPropertyName("payment_terms_in_days")]
    public int? PaymentTermsInDays { get; init; }

    /// <summary>
    /// Gets the European Community (EC) sales status for invoices generated from this profile.
    /// </summary>
    /// <value>
    /// The EC VAT status such as "EC Services", "EC Goods", or "Non-EC" for cross-border EU transactions.
    /// Affects VAT treatment and reporting requirements.
    /// </value>
    [JsonPropertyName("ec_status")]
    public string? EcStatus { get; init; }

    /// <summary>
    /// Gets the list of line items that will appear on each generated invoice.
    /// </summary>
    /// <value>
    /// A list of <see cref="InvoiceItem"/> objects defining the products or services, quantities,
    /// prices, and tax rates that will be used as the template for all automatically created invoices.
    /// </value>
    [JsonPropertyName("invoice_items")]
    public List<InvoiceItem>? InvoiceItems { get; init; }

    /// <summary>
    /// Gets the date and time when this recurring invoice profile was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing when this recurring invoice profile was first set up.
    /// </value>
    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this recurring invoice profile was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing the last time this profile was modified (e.g., items changed,
    /// frequency adjusted, or status updated).
    /// </value>
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }
}