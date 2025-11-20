// <copyright file="Estimate.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents an estimate (quote/proposal) in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Estimates are financial quotes or proposals sent to potential clients before work begins. They outline
/// the expected costs, scope, and terms for a project or service. Each estimate is associated with a
/// <see cref="Domain.Contact"/> (potential customer) and can optionally be linked to a <see cref="Domain.Project"/>.
/// </para>
/// <para>
/// Estimates support various statuses including Draft, Sent, Approved, and Rejected. Once approved, an
/// estimate can be converted into an <see cref="Domain.Invoice"/> for actual billing. Estimates can include
/// multiple line items, discounts, and support multi-currency transactions.
/// </para>
/// <para>
/// Estimates can be configured for different VAT/GST scenarios including standard VAT, interim UK VAT accounting,
/// and tax-exempt quotes.
/// </para>
/// <para>
/// API Endpoint: /v2/estimates
/// </para>
/// <para>
/// Minimum Access Level: Estimates and Invoices
/// </para>
/// </remarks>
/// <seealso cref="Contact"/>
/// <seealso cref="Project"/>
/// <seealso cref="Invoice"/>
/// <seealso cref="EstimateItem"/>
[DebuggerDisplay("Ref = {" + nameof(Reference) + "}, Status = {" + nameof(Status) + "}")]
public record Estimate
{
    /// <summary>
    /// Gets the unique URI identifier for this estimate.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this estimate in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the URI reference to the associated project.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Project"/> that this estimate relates to, if applicable.
    /// </value>
    [JsonPropertyName("project")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Project { get; init; }

    /// <summary>
    /// Gets the expanded project object when the estimate includes embedded project data.
    /// </summary>
    /// <value>
    /// The full <see cref="Domain.Project"/> object, populated when the API response includes expanded relationships.
    /// This property is not serialized to JSON.
    /// </value>
    [JsonIgnore]
    public Project? ProjectEntry { get; init; }

    /// <summary>
    /// Gets the URI reference to the customer contact.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Contact"/> representing the potential customer.
    /// This field is required when creating an estimate.
    /// </value>
    [JsonPropertyName("contact")]
    public Uri? Contact { get; init; }

    /// <summary>
    /// Gets the expanded contact object when the estimate includes embedded contact data.
    /// </summary>
    /// <value>
    /// The full <see cref="Domain.Contact"/> object, populated when the API response includes expanded relationships.
    /// This property is not serialized to JSON.
    /// </value>
    [JsonIgnore]
    public Contact? ContactEntry { get; init; }

    /// <summary>
    /// Gets the URI reference to the invoice created from this estimate.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Invoice"/> that was generated from this estimate, if it has been converted.
    /// </value>
    [JsonPropertyName("invoice")]
    public Uri? Invoice { get; init; }

    /// <summary>
    /// Gets the estimate reference number.
    /// </summary>
    /// <value>
    /// The unique reference or identification number for this estimate, typically auto-generated
    /// according to the company's numbering scheme.
    /// </value>
    [JsonPropertyName("reference")]
    public string? Reference { get; init; }

    /// <summary>
    /// Gets the discount percentage applied to the estimate total.
    /// </summary>
    /// <value>
    /// The discount rate as a percentage (e.g., 10 for 10% discount). Applied to the subtotal
    /// before tax calculations.
    /// </value>
    [JsonPropertyName("discount_percent")]
    public decimal? DiscountPercent { get; init; }

    /// <summary>
    /// Gets the type classification of this estimate.
    /// </summary>
    /// <value>
    /// The estimate type identifier, such as "Quote", "Proposal", or other custom type classifications.
    /// </value>
    [JsonPropertyName("estimate_type")]
    public string? EstimateType { get; init; }

    /// <summary>
    /// Gets the date when the estimate was issued.
    /// </summary>
    /// <value>
    /// The estimate issue date in YYYY-MM-DD format. This field is required when creating an estimate.
    /// </value>
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; init; }

    /// <summary>
    /// Gets the current status of the estimate.
    /// </summary>
    /// <value>
    /// One of "Draft", "Sent", "Open", "Approved", "Rejected", or "Invoiced", indicating the current stage of the estimate.
    /// </value>
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    /// <summary>
    /// Gets the currency for this estimate.
    /// </summary>
    /// <value>
    /// A three-letter ISO 4217 currency code (e.g., "GBP", "USD", "EUR").
    /// </value>
    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    /// <summary>
    /// Gets the net value of the estimate before tax.
    /// </summary>
    /// <value>
    /// The total estimate amount excluding sales tax/VAT, after applying any discount.
    /// </value>
    [JsonPropertyName("net_value")]
    public decimal? NetValue { get; init; }

    /// <summary>
    /// Gets the total value of the estimate including tax.
    /// </summary>
    /// <value>
    /// The total estimate amount including all applicable sales tax/VAT and after discount.
    /// </value>
    [JsonPropertyName("total_value")]
    public decimal? TotalValue { get; init; }

    /// <summary>
    /// Gets a value indicating whether this estimate includes sales tax calculations.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if VAT/GST is calculated and included in the estimate; otherwise,
    /// <see langword="false"/> for tax-exempt estimates.
    /// </value>
    [JsonPropertyName("involves_sales_tax")]
    public bool? InvolvesSalesTax { get; init; }

    /// <summary>
    /// Gets a value indicating whether this estimate uses interim UK VAT accounting.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the estimate is configured for interim UK VAT rules where VAT
    /// is accounted for when cash is received rather than when invoiced; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("is_interim_uk_vat")]
    public bool? IsInterimUkVat { get; init; }

    /// <summary>
    /// Gets the date and time when this estimate was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the last modification timestamp with timezone information.
    /// </value>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this estimate was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the creation timestamp with timezone information.
    /// </value>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>
    /// Gets the total sales tax value for this estimate.
    /// </summary>
    /// <value>
    /// The calculated total VAT/GST amount. The total tax across all line items.
    /// </value>
    [JsonPropertyName("sales_tax_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SalesTaxValue { get; init; }

    /// <summary>
    /// Gets internal notes or comments about this estimate.
    /// </summary>
    /// <value>
    /// Free-text notes for internal reference, not typically shown to the customer.
    /// </value>
    [JsonPropertyName("notes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Notes { get; init; }

    /// <summary>
    /// Gets the EC (European Community) VAT status for this estimate.
    /// </summary>
    /// <value>
    /// One of "UK/Non-EC", "EC Goods", "EC Services", "Reverse Charge", or "EC VAT MOSS".
    /// Used for VAT reporting and compliance within the European Community.
    /// </value>
    [JsonPropertyName("ec_status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EcStatus { get; init; }

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
    /// Gets a value indicating whether sales tax should be included in the total value.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the <see cref="TotalValue"/> includes sales tax; otherwise,
    /// <see langword="false"/> if sales tax is shown separately.
    /// </value>
    [JsonPropertyName("include_sales_tax_on_total_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IncludeSalesTaxOnTotalValue { get; init; }

    /// <summary>
    /// Gets the override contact name to display on this estimate.
    /// </summary>
    /// <value>
    /// A custom contact name to display instead of the contact's name from their record. Useful for
    /// addressing estimates to specific individuals within an organization.
    /// </value>
    [JsonPropertyName("client_contact_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ClientContactName { get; init; }

    /// <summary>
    /// Gets the collection of line items that make up this estimate.
    /// </summary>
    /// <value>
    /// An immutable array of <see cref="EstimateItem"/> objects detailing each product or service included in the estimate.
    /// </value>
    [JsonPropertyName("estimate_items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ImmutableArray<EstimateItem> EstimateItems { get; init; } = [];
}