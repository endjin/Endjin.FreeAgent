// <copyright file="Company.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a company in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// The Company resource contains general information about the authenticated company, including business structure,
/// financial configuration, accounting periods, and tax settings. This is a read-only resource representing the
/// currently authenticated company's details.
/// </para>
/// <para>
/// Company types include various business structures such as UkLimitedCompany, UsSoleProprietor, UniversalCompany,
/// and others, each with specific tax and regulatory requirements.
/// </para>
/// <para>
/// API Endpoint: /v2/company
/// </para>
/// <para>
/// Minimum Access Level: Time
/// </para>
/// </remarks>
/// <seealso cref="SalesTaxRate"/>
public record Company
{
    /// <summary>
    /// Gets the unique URI identifier for this company.
    /// </summary>
    /// <value>
    /// A URI representing the API endpoint for the authenticated company.
    /// </value>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the name of the company.
    /// </summary>
    /// <value>
    /// The registered or trading name of the business entity.
    /// </value>
    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; init; }

    /// <summary>
    /// Gets the FreeAgent subdomain for this company.
    /// </summary>
    /// <value>
    /// The unique subdomain identifier used to access this company's FreeAgent account (e.g., "companyname" in companyname.freeagent.com).
    /// </value>
    [JsonPropertyName("subdomain")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Subdomain { get; init; }

    /// <summary>
    /// Gets the type of business structure for this company.
    /// </summary>
    /// <value>
    /// A string representing the company classification such as "UkLimitedCompany", "UsSoleProprietor", "UniversalCompany", etc.
    /// The type determines applicable tax and regulatory requirements.
    /// </value>
    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Type { get; init; }

    /// <summary>
    /// Gets the base accounting currency for this company.
    /// </summary>
    /// <value>
    /// A three-letter ISO 4217 currency code (e.g., "GBP", "USD", "EUR") representing the company's native currency.
    /// </value>
    [JsonPropertyName("currency")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Currency { get; init; }

    /// <summary>
    /// Gets the units used for mileage tracking.
    /// </summary>
    /// <value>
    /// Either "Miles" or "Kilometers", determining how mileage expenses are recorded and calculated.
    /// </value>
    [JsonPropertyName("mileage_units")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? MileageUnits { get; init; }

    /// <summary>
    /// Gets the date when the company commenced business operations.
    /// </summary>
    /// <value>
    /// The official start date of the business entity.
    /// </value>
    [JsonPropertyName("company_start_date")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? CompanyStartDate { get; init; }

    /// <summary>
    /// Gets the end date of the company's first accounting year.
    /// </summary>
    /// <value>
    /// The date when the initial accounting period ends, used to establish the annual accounting cycle.
    /// </value>
    [JsonPropertyName("first_accounting_year_end")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? FirstAccountingYearEnd { get; init; }

    /// <summary>
    /// Gets the official registration number assigned to the company by regulatory authorities.
    /// </summary>
    /// <value>
    /// The company registration number (e.g., Companies House number in the UK, EIN in the US).
    /// </value>
    [JsonPropertyName("company_registration_number")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CompanyRegistrationNumber { get; init; }

    /// <summary>
    /// Gets the current sales tax registration status of the company.
    /// </summary>
    /// <value>
    /// A string indicating whether the company is registered for sales tax (VAT/GST) and the current status.
    /// </value>
    [JsonPropertyName("sales_tax_registration_status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SalesTaxRegistrationStatus { get; init; }

    /// <summary>
    /// Gets the sales tax registration number for this company.
    /// </summary>
    /// <value>
    /// The current VAT, GST, or sales tax registration number used for tax reporting and compliance.
    /// </value>
    [JsonPropertyName("sales_tax_registration_number")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SalesTaxRegistrationNumber { get; init; }

    /// <summary>
    /// Gets the collection of applicable sales tax rates for this company.
    /// </summary>
    /// <value>
    /// A list of <see cref="SalesTaxRate"/> objects representing the various tax rates that apply to this company's transactions.
    /// </value>
    /// <seealso cref="SalesTaxRate"/>
    [JsonPropertyName("sales_tax_rates")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<SalesTaxRate>? SalesTaxRates { get; init; }

    /// <summary>
    /// Gets a value indicating whether automatic sales tax calculation is supported for purchases.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the system can automatically calculate sales tax on purchase transactions; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("supports_auto_sales_tax_on_purchases")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? SupportsAutoSalesTaxOnPurchases { get; init; }

    /// <summary>
    /// Gets a value indicating whether EC (European Community) VAT reporting is enabled for this company.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if EC VAT reporting features are enabled; otherwise, <see langword="false"/>.
    /// This setting is relevant for companies conducting cross-border trade within the European Union.
    /// </value>
    [JsonPropertyName("ec_vat_reporting_enabled")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? EcVatReportingEnabled { get; init; }
}