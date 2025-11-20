// <copyright file="Company.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Converters;

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
    /// Gets the unique identifier for this company.
    /// </summary>
    /// <value>
    /// An integer uniquely identifying this company in the FreeAgent system.
    /// </value>
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Id { get; init; }

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
    /// The company classification such as <see cref="CompanyType.UkLimitedCompany"/>, <see cref="CompanyType.UsSoleProprietor"/>,
    /// <see cref="CompanyType.UniversalCompany"/>, etc. The type determines applicable tax and regulatory requirements.
    /// </value>
    [JsonPropertyName("type")]
    [JsonConverter(typeof(CompanyTypeJsonConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public CompanyType? Type { get; init; }

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
    /// Either "miles" or "kilometers" (lowercase), determining how mileage expenses are recorded and calculated.
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
    /// Gets the date when the company started trading.
    /// </summary>
    /// <value>
    /// The date when the company commenced trading operations, which may differ from the company start date.
    /// </value>
    [JsonPropertyName("trading_start_date")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? TradingStartDate { get; init; }

    /// <summary>
    /// Gets the FreeAgent start date for transaction calculations.
    /// </summary>
    /// <value>
    /// The date from which FreeAgent begins calculating transactions and financial reports for this company.
    /// </value>
    [JsonPropertyName("freeagent_start_date")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? FreeagentStartDate { get; init; }

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
    /// Gets the first line of the company's address.
    /// </summary>
    /// <value>
    /// The first line of the street address.
    /// </value>
    [JsonPropertyName("address1")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Address1 { get; init; }

    /// <summary>
    /// Gets the second line of the company's address.
    /// </summary>
    /// <value>
    /// The second line of the street address.
    /// </value>
    [JsonPropertyName("address2")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Address2 { get; init; }

    /// <summary>
    /// Gets the third line of the company's address.
    /// </summary>
    /// <value>
    /// The third line of the street address.
    /// </value>
    [JsonPropertyName("address3")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Address3 { get; init; }

    /// <summary>
    /// Gets the town or city of the company's address.
    /// </summary>
    /// <value>
    /// The town or city name.
    /// </value>
    [JsonPropertyName("town")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Town { get; init; }

    /// <summary>
    /// Gets the region, state, or county of the company's address.
    /// </summary>
    /// <value>
    /// The region, state, or county name.
    /// </value>
    [JsonPropertyName("region")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Region { get; init; }

    /// <summary>
    /// Gets the postal code or ZIP code of the company's address.
    /// </summary>
    /// <value>
    /// The postal code or ZIP code.
    /// </value>
    [JsonPropertyName("postcode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Postcode { get; init; }

    /// <summary>
    /// Gets the country of the company's address.
    /// </summary>
    /// <value>
    /// The country name.
    /// </value>
    [JsonPropertyName("country")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Country { get; init; }

    /// <summary>
    /// Gets the contact email address for the company.
    /// </summary>
    /// <value>
    /// The primary contact email address.
    /// </value>
    [JsonPropertyName("contact_email")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ContactEmail { get; init; }

    /// <summary>
    /// Gets the contact phone number for the company.
    /// </summary>
    /// <value>
    /// The primary contact phone number.
    /// </value>
    [JsonPropertyName("contact_phone")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ContactPhone { get; init; }

    /// <summary>
    /// Gets the company's website URL.
    /// </summary>
    /// <value>
    /// The company website address.
    /// </value>
    [JsonPropertyName("website")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Website { get; init; }

    /// <summary>
    /// Gets the free-text description of the business type.
    /// </summary>
    /// <value>
    /// A description of what the business does.
    /// </value>
    [JsonPropertyName("business_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? BusinessType { get; init; }

    /// <summary>
    /// Gets the standardized business category.
    /// </summary>
    /// <value>
    /// A standardized business category value from the predefined list of business categories.
    /// </value>
    [JsonPropertyName("business_category")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? BusinessCategory { get; init; }

    /// <summary>
    /// Gets the short date format preference for this company.
    /// </summary>
    /// <value>
    /// The date format string used for displaying short dates in the FreeAgent interface.
    /// Valid values are defined in <see cref="Domain.ShortDateFormat"/>:
    /// <see cref="Domain.ShortDateFormat.AbbreviatedMonth"/> ("dd mmm yy"),
    /// <see cref="Domain.ShortDateFormat.European"/> ("dd-mm-yyyy"),
    /// <see cref="Domain.ShortDateFormat.US"/> ("mm/dd/yyyy"),
    /// <see cref="Domain.ShortDateFormat.ISO"/> ("yyyy-mm-dd").
    /// Use <see cref="Domain.ShortDateFormat.IsValid(string?)"/> to validate values.
    /// </value>
    [JsonPropertyName("short_date_format")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ShortDateFormat { get; init; }

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
    /// A list of sales tax rates (percentages) applicable to this company's transactions.
    /// </value>
    [JsonPropertyName("sales_tax_rates")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<decimal>? SalesTaxRates { get; init; }

    /// <summary>
    /// Gets the name of the sales tax applicable to this company.
    /// </summary>
    /// <value>
    /// The tax name such as "VAT", "GST", or "Sales Tax".
    /// </value>
    [JsonPropertyName("sales_tax_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SalesTaxName { get; init; }

    /// <summary>
    /// Gets the effective date of the sales tax registration.
    /// </summary>
    /// <value>
    /// The date when sales tax registration became effective.
    /// </value>
    [JsonPropertyName("sales_tax_effective_date")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? SalesTaxEffectiveDate { get; init; }

    /// <summary>
    /// Gets a value indicating whether the sales tax is value-added (reclaimable).
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the sales tax can be reclaimed (like VAT/GST); <see langword="false"/> if it cannot be reclaimed (like US sales tax).
    /// </value>
    [JsonPropertyName("sales_tax_is_value_added")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? SalesTaxIsValueAdded { get; init; }

    /// <summary>
    /// Gets the sales tax registration status.
    /// </summary>
    /// <value>
    /// The current registration status for sales tax: <see cref="Domain.SalesTaxRegistrationStatus.Registered"/>
    /// or <see cref="Domain.SalesTaxRegistrationStatus.NotRegistered"/>.
    /// </value>
    [JsonPropertyName("sales_tax_registration_status")]
    [JsonConverter(typeof(SalesTaxRegistrationStatusJsonConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SalesTaxRegistrationStatus? SalesTaxRegistrationStatus { get; init; }

    /// <summary>
    /// Gets the effective date of sales tax deregistration.
    /// </summary>
    /// <value>
    /// The date when sales tax deregistration became effective.
    /// </value>
    [JsonPropertyName("sales_tax_deregistration_effective_date")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? SalesTaxDeregistrationEffectiveDate { get; init; }

    /// <summary>
    /// Gets the name of the second sales tax (for US and Universal companies).
    /// </summary>
    /// <value>
    /// The second tax name for jurisdictions with multiple sales taxes.
    /// </value>
    [JsonPropertyName("second_sales_tax_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SecondSalesTaxName { get; init; }

    /// <summary>
    /// Gets the collection of second sales tax rates (for US and Universal companies).
    /// </summary>
    /// <value>
    /// A list of second sales tax rates (percentages) for jurisdictions with multiple taxes.
    /// </value>
    [JsonPropertyName("second_sales_tax_rates")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<decimal>? SecondSalesTaxRates { get; init; }

    /// <summary>
    /// Gets a value indicating whether the second sales tax is compound (for US and Universal companies).
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the second sales tax is calculated on top of the first; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("second_sales_tax_is_compound")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? SecondSalesTaxIsCompound { get; init; }

    /// <summary>
    /// Gets the end date of the first VAT return period (UK companies only).
    /// </summary>
    /// <value>
    /// The date when the first VAT return period ends.
    /// </value>
    [JsonPropertyName("vat_first_return_period_ends_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? VatFirstReturnPeriodEndsOn { get; init; }

    /// <summary>
    /// Gets the initial VAT accounting basis (UK companies only).
    /// </summary>
    /// <value>
    /// The VAT accounting method: <see cref="VatBasis.Invoice"/> ("Invoice") for accrual accounting
    /// or <see cref="VatBasis.Cash"/> ("Cash") for cash accounting.
    /// Use <see cref="VatBasis.IsValid(string?)"/> to validate values.
    /// </value>
    [JsonPropertyName("initial_vat_basis")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? InitialVatBasis { get; init; }

    /// <summary>
    /// Gets a value indicating whether the company was initially on the VAT Flat Rate Scheme (UK companies only).
    /// </summary>
    /// <value>
    /// <see langword="true"/> if initially on the Flat Rate Scheme; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("initially_on_frs")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? InitiallyOnFrs { get; init; }

    /// <summary>
    /// Gets the initial VAT Flat Rate Scheme type (UK companies only).
    /// </summary>
    /// <value>
    /// The Flat Rate Scheme category if applicable.
    /// </value>
    [JsonPropertyName("initial_vat_frs_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? InitialVatFrsType { get; init; }

    /// <summary>
    /// Gets a value indicating whether CIS (Construction Industry Scheme) is enabled.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if CIS is enabled for this company; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("cis_enabled")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? CisEnabled { get; init; }

    /// <summary>
    /// Gets a value indicating whether the company is a CIS subcontractor.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the company is registered as a CIS subcontractor; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("cis_subcontractor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? CisSubcontractor { get; init; }

    /// <summary>
    /// Gets a value indicating whether the company is a CIS contractor.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the company is registered as a CIS contractor; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("cis_contractor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? CisContractor { get; init; }

    /// <summary>
    /// Gets the collection of annual accounting periods.
    /// </summary>
    /// <value>
    /// A list of <see cref="AnnualAccountingPeriod"/> objects representing the fiscal years for this company.
    /// </value>
    /// <seealso cref="AnnualAccountingPeriod"/>
    [JsonPropertyName("annual_accounting_periods")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<AnnualAccountingPeriod>? AnnualAccountingPeriods { get; init; }

    /// <summary>
    /// Gets the collection of locked attributes that cannot be modified.
    /// </summary>
    /// <value>
    /// A list of field names that are locked and cannot be changed.
    /// </value>
    [JsonPropertyName("locked_attributes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? LockedAttributes { get; init; }

    /// <summary>
    /// Gets the timestamp when this company record was created.
    /// </summary>
    /// <value>
    /// The UTC timestamp of record creation.
    /// </value>
    [JsonPropertyName("created_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>
    /// Gets the timestamp when this company record was last updated.
    /// </summary>
    /// <value>
    /// The UTC timestamp of the last update.
    /// </value>
    [JsonPropertyName("updated_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? UpdatedAt { get; init; }
}