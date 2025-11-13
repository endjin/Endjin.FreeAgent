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
/// At least one name field (<see cref="FirstName"/>, <see cref="LastName"/>, or <see cref="OrganisationName"/>)
/// must be provided when creating a contact. Contacts can be filtered by status (Active or Hidden) and
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
    /// FreeAgent supports over 30 languages including English, Spanish, French, German, and others.
    /// </value>
    [JsonPropertyName("locale")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Locale { get; init; }

    /// <summary>
    /// Gets the current account balance for this contact.
    /// </summary>
    /// <value>
    /// The current balance representing the net amount owed to or by this contact.
    /// </value>
    [JsonPropertyName("account_balance")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? AccountBalance { get; init; }

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
    /// The count of projects with "Active" status linked to this contact.
    /// </value>
    /// <seealso cref="Project"/>
    [JsonPropertyName("active_projects_count")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ActiveProjectsCount { get; init; }

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
}