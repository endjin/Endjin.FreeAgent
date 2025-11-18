// <copyright file="Contact.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a contact in the FreeAgent accounting system, which can be a client, supplier, or other business entity.
/// </summary>
/// <remarks>
/// <para>
/// Contacts are fundamental entities in FreeAgent used to manage relationships with clients and suppliers.
/// Each contact can have associated projects, invoices, bills, and other financial documents.
/// </para>
/// <para>
/// When creating a contact, you must provide either BOTH <see cref="FirstName"/> AND <see cref="LastName"/> together,
/// OR <see cref="OrganisationName"/> alone. Contacts can be filtered by status (Active or Hidden) and
/// categorized as clients or suppliers.
/// </para>
/// <para>
/// API Endpoint: /v2/contacts
/// </para>
/// <para>
/// Minimum Access Level: Time (additional attributes require "Contacts &amp; Projects" permission).
/// </para>
/// </remarks>
/// <seealso cref="Project"/>
/// <seealso cref="Invoice"/>
[DebuggerDisplay("OrganisationName = {" + nameof(OrganisationName) + "}")]
public record Contact
{
    /// <summary>
    /// Gets the unique URI identifier for this contact.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this contact in the FreeAgent system.
    /// This value is assigned by the API upon creation and is used to reference the contact in other resources.
    /// </value>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the first name of the individual contact.
    /// </summary>
    /// <value>
    /// The first name of the contact person. At least one of <see cref="FirstName"/>, <see cref="LastName"/>,
    /// or <see cref="OrganisationName"/> must be specified when creating a contact.
    /// </value>
    [JsonPropertyName("first_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? FirstName { get; init; }

    /// <summary>
    /// Gets the last name of the individual contact.
    /// </summary>
    /// <value>
    /// The last name of the contact person. At least one of <see cref="FirstName"/>, <see cref="LastName"/>,
    /// or <see cref="OrganisationName"/> must be specified when creating a contact.
    /// </value>
    [JsonPropertyName("last_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LastName { get; init; }

    /// <summary>
    /// Gets the organisation or company name for this contact.
    /// </summary>
    /// <value>
    /// The name of the organisation or business entity. At least one of <see cref="FirstName"/>, <see cref="LastName"/>,
    /// or <see cref="OrganisationName"/> must be specified when creating a contact.
    /// </value>
    [JsonPropertyName("organisation_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? OrganisationName { get; init; }

    /// <summary>
    /// Gets the primary email address for this contact.
    /// </summary>
    /// <value>
    /// The email address used for general contact communications. Free trial accounts are limited to a maximum
    /// of 2 unique email addresses across all contacts.
    /// </value>
    [JsonPropertyName("email")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Email { get; init; }

    /// <summary>
    /// Gets the billing-specific email address for this contact.
    /// </summary>
    /// <value>
    /// An optional separate email address to be used specifically for billing and invoice communications.
    /// Free trial accounts are limited to a maximum of 2 unique email addresses across all contacts.
    /// </value>
    [JsonPropertyName("billing_email")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? BillingEmail { get; init; }

    /// <summary>
    /// Gets the primary phone number for this contact.
    /// </summary>
    /// <value>
    /// The contact's main telephone number.
    /// </value>
    [JsonPropertyName("phone_number")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? PhoneNumber { get; init; }

    /// <summary>
    /// Gets the mobile phone number for this contact.
    /// </summary>
    /// <value>
    /// The contact's mobile or cellular telephone number.
    /// </value>
    [JsonPropertyName("mobile")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Mobile { get; init; }

    /// <summary>
    /// Gets the first line of the contact's address.
    /// </summary>
    /// <value>
    /// The primary address line, typically containing the building number and street name.
    /// </value>
    [JsonPropertyName("address1")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Address1 { get; init; }

    /// <summary>
    /// Gets the second line of the contact's address.
    /// </summary>
    /// <value>
    /// Additional address information such as suite or apartment number.
    /// </value>
    [JsonPropertyName("address2")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Address2 { get; init; }

    /// <summary>
    /// Gets the third line of the contact's address.
    /// </summary>
    /// <value>
    /// Additional address information if needed for complex addresses.
    /// </value>
    [JsonPropertyName("address3")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Address3 { get; init; }

    /// <summary>
    /// Gets the town or city of the contact's address.
    /// </summary>
    /// <value>
    /// The town or city name.
    /// </value>
    [JsonPropertyName("town")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Town { get; init; }

    /// <summary>
    /// Gets the region, state, or county of the contact's address.
    /// </summary>
    /// <value>
    /// The region, state, province, or county name.
    /// </value>
    [JsonPropertyName("region")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Region { get; init; }

    /// <summary>
    /// Gets the postal code or ZIP code of the contact's address.
    /// </summary>
    /// <value>
    /// The postcode or ZIP code.
    /// </value>
    [JsonPropertyName("postcode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Postcode { get; init; }

    /// <summary>
    /// Gets the country of the contact's address.
    /// </summary>
    /// <value>
    /// The country name.
    /// </value>
    [JsonPropertyName("country")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Country { get; init; }

    /// <summary>
    /// Gets a value indicating whether the contact name should be displayed on invoices alongside the organisation name.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the contact name should be displayed on invoices; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("contact_name_on_invoices")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? ContactNameOnInvoices { get; init; }

    /// <summary>
    /// Gets the locale code for invoice language preferences.
    /// </summary>
    /// <value>
    /// A locale code (e.g., "en", "es", "fr", "de") determining the language used on invoices sent to this contact.
    /// FreeAgent supports 30+ languages including English, Spanish, French, German, and others.
    /// </value>
    [JsonPropertyName("locale")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Locale { get; init; }

    /// <summary>
    /// Gets a value indicating whether this contact uses a separate invoice numbering sequence.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if this contact has its own invoice numbering sequence; otherwise, <see langword="false"/>
    /// to use the global invoice numbering sequence.
    /// </value>
    [JsonPropertyName("uses_contact_invoice_sequence")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? UsesContactInvoiceSequence { get; init; }

    /// <summary>
    /// Gets the sales tax charging behavior for this contact.
    /// </summary>
    /// <value>
    /// One of "Auto", "Always", or "Never", determining when sales tax (VAT/GST) should be applied to invoices
    /// for this contact.
    /// </value>
    [JsonPropertyName("charge_sales_tax")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ChargeSalesTax { get; init; }

    /// <summary>
    /// Gets the sales tax registration number for this contact.
    /// </summary>
    /// <value>
    /// The contact's VAT, GST, or other sales tax registration number for tax reporting purposes.
    /// </value>
    [JsonPropertyName("sales_tax_registration_number")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SalesTaxRegistrationNumber { get; init; }

    /// <summary>
    /// Gets the number of active projects associated with this contact.
    /// </summary>
    /// <value>
    /// The count of projects with "Active" status linked to this contact (returned as a string by the API).
    /// </value>
    /// <seealso cref="Project"/>
    [JsonPropertyName("active_projects_count")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ActiveProjectsCount { get; init; }

    /// <summary>
    /// Gets the current account balance for this contact.
    /// </summary>
    /// <value>
    /// The account balance showing the amount owed by or to this contact.
    /// A positive value indicates money owed to you, a negative value indicates money you owe.
    /// </value>
    /// <remarks>
    /// Requires "Contacts &amp; Projects" permission level.
    /// </remarks>
    [JsonPropertyName("account_balance")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? AccountBalance { get; init; }

    /// <summary>
    /// Gets the status of this contact.
    /// </summary>
    /// <value>
    /// Either "Active" or "Hidden". Hidden contacts are archived but remain in the system for historical records.
    /// </value>
    [JsonPropertyName("status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Status { get; init; }

    /// <summary>
    /// Gets the date and time when this contact was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the creation timestamp in UTC.
    /// </value>
    [JsonPropertyName("created_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this contact was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the last modification timestamp in UTC.
    /// </value>
    [JsonPropertyName("updated_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? UpdatedAt { get; init; }

    /// <summary>
    /// Gets the state of the direct debit mandate for this contact.
    /// </summary>
    /// <value>
    /// One of "setup", "pending", "inactive", "active", or "failed", indicating the current status
    /// of the direct debit arrangement with this contact.
    /// </value>
    [JsonPropertyName("direct_debit_mandate_state")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DirectDebitMandateState { get; init; }

    /// <summary>
    /// Gets the default payment terms in days for invoices sent to this contact.
    /// </summary>
    /// <value>
    /// The number of days after invoice date that payment is due. This value is used as the default
    /// when creating new invoices for this contact.
    /// </value>
    /// <remarks>
    /// Requires "Contacts &amp; Projects" permission level.
    /// </remarks>
    [JsonPropertyName("default_payment_terms_in_days")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? DefaultPaymentTermsInDays { get; init; }

    /// <summary>
    /// Gets a value indicating whether this contact is a CIS (Construction Industry Scheme) subcontractor.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if this contact is registered as a CIS subcontractor; otherwise, <see langword="false"/>.
    /// This property may not appear in responses if CIS is not enabled for the company.
    /// </value>
    /// <remarks>
    /// CIS is a UK tax scheme where contractors deduct money from subcontractors' payments and pass it to HMRC.
    /// </remarks>
    [JsonPropertyName("is_cis_subcontractor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsCisSubcontractor { get; init; }

    /// <summary>
    /// Gets the CIS (Construction Industry Scheme) deduction rate for this contact.
    /// </summary>
    /// <value>
    /// One of "cis_gross" (0% deduction), "cis_standard" (20% deduction), or "cis_higher" (30% deduction),
    /// indicating the tax deduction rate to apply to payments made to this subcontractor.
    /// </value>
    /// <remarks>
    /// Only applicable when <see cref="IsCisSubcontractor"/> is <see langword="true"/>.
    /// </remarks>
    [JsonPropertyName("cis_deduction_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CisDeductionRate { get; init; }

    /// <summary>
    /// Gets the Unique Taxpayer Reference (UTR) for this contact.
    /// </summary>
    /// <value>
    /// A 10-digit reference number used in the UK tax system. Format should be 10 digits without spaces.
    /// </value>
    /// <remarks>
    /// The UTR is issued by HMRC and is used to identify taxpayers in the UK tax system.
    /// Required for CIS subcontractors.
    /// </remarks>
    [JsonPropertyName("unique_tax_reference")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? UniqueTaxReference { get; init; }

    /// <summary>
    /// Gets the subcontractor verification number for CIS purposes.
    /// </summary>
    /// <value>
    /// A verification number in the format "V" followed by 10 digits and optionally 2 letters (e.g., "V1234567890AB").
    /// This number is provided by HMRC when a subcontractor is verified for CIS.
    /// </value>
    /// <remarks>
    /// This verification number confirms the subcontractor's registration with HMRC and determines their deduction rate.
    /// </remarks>
    [JsonPropertyName("subcontractor_verification_number")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SubcontractorVerificationNumber { get; init; }
}