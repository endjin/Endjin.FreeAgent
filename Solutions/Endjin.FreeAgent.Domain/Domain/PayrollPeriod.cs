// <copyright file="PayrollPeriod.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a payroll period in the FreeAgent RTI (Real Time Information) payroll system.
/// </summary>
/// <remarks>
/// <para>
/// Payroll periods represent the pay periods within a UK tax year (April to March). Each period contains
/// information about the filing status for RTI submissions to HMRC, employment allowance claims, and
/// construction industry scheme deductions.
/// </para>
/// <para>
/// The period number is 0-indexed:
/// <list type="bullet">
/// <item>Period 0 = April (first month of UK tax year)</item>
/// <item>Period 1 = May</item>
/// <item>Period 11 = March (last month of UK tax year)</item>
/// </list>
/// </para>
/// <para>
/// Filing status indicates the RTI submission state:
/// <list type="bullet">
/// <item><b>unfiled</b> - No submission made yet</item>
/// <item><b>pending</b> - Submission in progress</item>
/// <item><b>rejected</b> - Submission rejected by HMRC</item>
/// <item><b>partially_filed</b> - Some employees filed</item>
/// <item><b>filed</b> - Successfully submitted to HMRC</item>
/// </list>
/// </para>
/// <para>
/// API Access: Read-only via GET /v2/payroll/:year and GET /v2/payroll/:year/:period
/// Minimum Access Level: Tax and Limited Accounting. Only available for UK companies.
/// </para>
/// </remarks>
/// <seealso cref="Payslip"/>
/// <seealso cref="PayrollPayment"/>
public record PayrollPeriod
{
    /// <summary>
    /// Gets the API URL for this payroll period resource.
    /// </summary>
    /// <value>
    /// The unique API endpoint URL for accessing this specific payroll period.
    /// </value>
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the period number within the tax year.
    /// </summary>
    /// <value>
    /// The pay period number (0-11), where 0 represents April and 11 represents March.
    /// </value>
    [JsonPropertyName("period")]
    public int? Period { get; init; }

    /// <summary>
    /// Gets the pay frequency for this period.
    /// </summary>
    /// <value>
    /// The frequency of pay, typically "Monthly" or "Weekly".
    /// </value>
    [JsonPropertyName("frequency")]
    public string? Frequency { get; init; }

    /// <summary>
    /// Gets the date for this payroll period.
    /// </summary>
    /// <value>
    /// The pay date for this period in YYYY-MM-DD format.
    /// </value>
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; init; }

    /// <summary>
    /// Gets the RTI filing status for this period.
    /// </summary>
    /// <value>
    /// The submission status: "unfiled", "pending", "rejected", "partially_filed", or "filed".
    /// </value>
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    /// <summary>
    /// Gets whether Employment Allowance was claimed for this period.
    /// </summary>
    /// <value>
    /// True if Employment Allowance was claimed, false otherwise.
    /// </value>
    [JsonPropertyName("employment_allowance_claimed")]
    public bool? EmploymentAllowanceClaimed { get; init; }

    /// <summary>
    /// Gets the Employment Allowance amount claimed for this period.
    /// </summary>
    /// <value>
    /// The amount of Employment Allowance claimed to offset employer National Insurance liability.
    /// </value>
    [JsonPropertyName("employment_allowance_amount")]
    public decimal? EmploymentAllowanceAmount { get; init; }

    /// <summary>
    /// Gets the Construction Industry Scheme (CIS) deduction for this period.
    /// </summary>
    /// <value>
    /// The CIS liability withheld during this period for subcontractors.
    /// </value>
    [JsonPropertyName("construction_industry_scheme_deduction")]
    public decimal? ConstructionIndustrySchemeDeduction { get; init; }

    /// <summary>
    /// Gets the payslips for this period.
    /// </summary>
    /// <value>
    /// A list of <see cref="Payslip"/> objects for employees paid in this period.
    /// This property is only populated when retrieving a specific period via GET /v2/payroll/:year/:period.
    /// </value>
    [JsonPropertyName("payslips")]
    public List<Payslip>? Payslips { get; init; }

    /// <summary>
    /// Gets the date and time when this payroll period record was created.
    /// </summary>
    /// <value>
    /// The timestamp of when the period was created in FreeAgent.
    /// </value>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this payroll period record was last updated.
    /// </summary>
    /// <value>
    /// The timestamp of the most recent modification to this period record.
    /// </value>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; init; }
}
