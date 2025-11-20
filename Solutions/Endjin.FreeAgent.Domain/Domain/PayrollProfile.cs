// <copyright file="PayrollProfile.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents an employee's payroll profile for a specific tax year in the FreeAgent system.
/// </summary>
/// <remarks>
/// <para>
/// A payroll profile contains personal information about an employee that is relevant for payroll
/// processing and RTI (Real Time Information) reporting to HMRC. This includes address details,
/// demographic information, and previous employment earnings data.
/// </para>
/// <para>
/// The API uses UK tax years (April to March). When specifying a year parameter, use the
/// tax year end (e.g., 2026 for the tax year April 2025 - March 2026).
/// </para>
/// <para>
/// API Endpoint: /v2/payroll_profiles/:year
/// </para>
/// <para>
/// Minimum Access Level: Tax and Limited Accounting. Only available for UK companies.
/// This API is read-only.
/// </para>
/// </remarks>
/// <seealso cref="User"/>
public record PayrollProfile
{
    /// <summary>
    /// Gets the API URL of the user (employee) this payroll profile belongs to.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies the user in the FreeAgent system.
    /// This value links the payroll profile to its associated user.
    /// </value>
    [JsonPropertyName("user")]
    public Uri? User { get; init; }

    /// <summary>
    /// Gets the first line of the employee's address.
    /// </summary>
    /// <value>
    /// The first address line, typically the building number and street name.
    /// </value>
    [JsonPropertyName("address_line_1")]
    public string? AddressLine1 { get; init; }

    /// <summary>
    /// Gets the second line of the employee's address.
    /// </summary>
    /// <value>
    /// The second address line, if applicable.
    /// </value>
    [JsonPropertyName("address_line_2")]
    public string? AddressLine2 { get; init; }

    /// <summary>
    /// Gets the third line of the employee's address.
    /// </summary>
    /// <value>
    /// The third address line, if applicable.
    /// </value>
    [JsonPropertyName("address_line_3")]
    public string? AddressLine3 { get; init; }

    /// <summary>
    /// Gets the fourth line of the employee's address.
    /// </summary>
    /// <value>
    /// The fourth address line, if applicable.
    /// </value>
    [JsonPropertyName("address_line_4")]
    public string? AddressLine4 { get; init; }

    /// <summary>
    /// Gets the employee's postal code.
    /// </summary>
    /// <value>
    /// The postal code for the employee's address.
    /// Only present for users in the UK.
    /// </value>
    [JsonPropertyName("postcode")]
    public string? Postcode { get; init; }

    /// <summary>
    /// Gets the employee's country.
    /// </summary>
    /// <value>
    /// The country for the employee's address.
    /// Only present for users outside the UK, Isle of Man, and Channel Islands.
    /// </value>
    [JsonPropertyName("country")]
    public string? Country { get; init; }

    /// <summary>
    /// Gets the employee's title or honorific.
    /// </summary>
    /// <value>
    /// The title such as Mr, Mrs, Ms, Dr, etc.
    /// </value>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// Gets the employee's gender as defined by HMRC.
    /// </summary>
    /// <value>
    /// The gender code as required for RTI reporting to HMRC.
    /// </value>
    [JsonPropertyName("gender")]
    public string? Gender { get; init; }

    /// <summary>
    /// Gets the employee's date of birth.
    /// </summary>
    /// <value>
    /// The date of birth used for tax and payroll calculations.
    /// </value>
    [JsonPropertyName("date_of_birth")]
    public DateOnly? DateOfBirth { get; init; }

    /// <summary>
    /// Gets the total pay from previous employment in this tax year.
    /// </summary>
    /// <value>
    /// The gross earnings from any previous employment within the current tax year,
    /// used for cumulative tax calculations.
    /// </value>
    [JsonPropertyName("total_pay_in_previous_employment")]
    public decimal? TotalPayInPreviousEmployment { get; init; }

    /// <summary>
    /// Gets the total tax from previous employment in this tax year.
    /// </summary>
    /// <value>
    /// The tax deducted from any previous employment within the current tax year,
    /// used for cumulative tax calculations.
    /// </value>
    [JsonPropertyName("total_tax_in_previous_employment")]
    public decimal? TotalTaxInPreviousEmployment { get; init; }

    /// <summary>
    /// Gets the date when employment started.
    /// </summary>
    /// <value>
    /// The employment start date, only present if the employee started during this tax year.
    /// </value>
    [JsonPropertyName("employment_starts_on")]
    public DateOnly? EmploymentStartsOn { get; init; }

    /// <summary>
    /// Gets the date and time when this payroll profile was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the creation timestamp in UTC.
    /// </value>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this payroll profile was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the last modification timestamp in UTC.
    /// </value>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; init; }
}
