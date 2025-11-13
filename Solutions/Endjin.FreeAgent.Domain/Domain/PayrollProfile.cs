// <copyright file="PayrollProfile.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents an employee's payroll profile configuration in the FreeAgent payroll system.
/// </summary>
/// <remarks>
/// <para>
/// A payroll profile contains all the information needed to calculate an employee's pay, tax deductions,
/// National Insurance contributions, pension contributions, and student loan repayments. This profile is
/// used when creating <see cref="Payslip"/> records.
/// </para>
/// <para>
/// The profile includes UK-specific payroll elements:
/// <list type="bullet">
/// <item><b>Tax Code</b> - HMRC-issued code determining the employee's tax-free allowance (e.g., "1257L")</item>
/// <item><b>NI Letter</b> - National Insurance category letter determining contribution rates (e.g., "A", "B", "C")</item>
/// <item><b>Pension Scheme</b> - Auto-enrolment pension scheme configuration</item>
/// <item><b>Student Loan Type</b> - Repayment plan type if the employee has a student loan (Plan 1, Plan 2, Plan 4, Postgraduate)</item>
/// </list>
/// </para>
/// <para>
/// Each employee (represented by a <see cref="User"/>) can have only one active payroll profile. The profile
/// is used by FreeAgent's payroll features to:
/// <list type="number">
/// <item>Calculate PAYE income tax deductions based on the tax code and cumulative earnings</item>
/// <item>Calculate National Insurance contributions (both employee and employer)</item>
/// <item>Calculate and manage pension contributions for auto-enrolment compliance</item>
/// <item>Calculate student loan repayments based on salary thresholds</item>
/// <item>Generate payslips with all statutory deductions</item>
/// <item>Prepare RTI (Real Time Information) submissions to HMRC</item>
/// </list>
/// </para>
/// <para>
/// The annual salary is used to pro-rata deductions across pay periods. FreeAgent supports monthly, weekly,
/// and other pay frequencies.
/// </para>
/// <para>
/// API Access: Accessible via GET/POST/PUT /v2/payroll_profiles
/// Minimum Access Level: Payroll admin access
/// </para>
/// </remarks>
/// <seealso cref="Payslip"/>
/// <seealso cref="User"/>
public record PayrollProfile
{
    /// <summary>
    /// Gets the API URL for this payroll profile resource.
    /// </summary>
    /// <value>
    /// The unique API endpoint URL for accessing this specific payroll profile.
    /// </value>
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the unique identifier for this payroll profile.
    /// </summary>
    /// <value>
    /// The FreeAgent-assigned unique ID for this payroll profile.
    /// </value>
    [JsonPropertyName("id")]
    public long? Id { get; init; }

    /// <summary>
    /// Gets the API URL of the user (employee) this payroll profile belongs to.
    /// </summary>
    /// <value>
    /// A reference URL to the <see cref="User"/> resource representing the employee.
    /// Each user can have only one active payroll profile.
    /// </value>
    [JsonPropertyName("user")]
    public Uri? User { get; init; }

    /// <summary>
    /// Gets the employee identifier for payroll purposes.
    /// </summary>
    /// <value>
    /// An optional internal employee ID or payroll number used for referencing this employee
    /// in payroll reports and HMRC submissions. This is distinct from the FreeAgent user ID.
    /// </value>
    [JsonPropertyName("employee_id")]
    public string? EmployeeId { get; init; }

    /// <summary>
    /// Gets the HMRC tax code for this employee.
    /// </summary>
    /// <value>
    /// The tax code issued by HMRC (e.g., "1257L", "BR", "D0", "K497"). The tax code determines:
    /// <list type="bullet">
    /// <item>The employee's tax-free personal allowance</item>
    /// <item>Which tax bands and rates apply</item>
    /// <item>Whether emergency tax applies</item>
    /// <item>Any adjustments for benefits, underpaid tax, or multiple jobs</item>
    /// </list>
    /// Common codes: "1257L" (standard allowance for 2023/24), "BR" (basic rate on all income),
    /// "K" codes (negative allowance), "NT" (no tax).
    /// </value>
    [JsonPropertyName("tax_code")]
    public string? TaxCode { get; init; }

    /// <summary>
    /// Gets the National Insurance category letter for this employee.
    /// </summary>
    /// <value>
    /// The NI category letter (e.g., "A", "B", "C", "H", "M") which determines the rate of
    /// National Insurance contributions for both employee and employer. Common categories:
    /// <list type="bullet">
    /// <item><b>A</b> - Standard rate for most employees under state pension age</item>
    /// <item><b>B</b> - Married women/widows with reduced rate certificate</item>
    /// <item><b>C</b> - Employees over state pension age (no employee NIC, but employer still pays)</item>
    /// <item><b>H</b> - Apprentices under 25</item>
    /// <item><b>M</b> - Employees under 21</item>
    /// </list>
    /// </value>
    [JsonPropertyName("ni_letter")]
    public string? NiLetter { get; init; }

    /// <summary>
    /// Gets the employee's annual gross salary.
    /// </summary>
    /// <value>
    /// The gross annual salary before any deductions. This is used to:
    /// <list type="bullet">
    /// <item>Calculate pro-rata pay for each pay period (e.g., monthly = annual / 12)</item>
    /// <item>Determine cumulative tax and NI calculations</item>
    /// <item>Calculate pension contributions as a percentage</item>
    /// <item>Determine student loan repayment thresholds</item>
    /// </list>
    /// </value>
    [JsonPropertyName("annual_salary")]
    public decimal? AnnualSalary { get; init; }

    /// <summary>
    /// Gets the pension scheme configuration for this employee.
    /// </summary>
    /// <value>
    /// The pension scheme identifier or configuration. UK employers must offer auto-enrolment pensions
    /// to eligible employees. Common scheme types include workplace pensions, NEST (National Employment
    /// Savings Trust), and other qualifying schemes.
    /// </value>
    [JsonPropertyName("pension_scheme")]
    public string? PensionScheme { get; init; }

    /// <summary>
    /// Gets the employee's pension contribution percentage.
    /// </summary>
    /// <value>
    /// The percentage of qualifying earnings deducted for pension contributions (e.g., 5.0 for 5%).
    /// UK auto-enrolment minimum rates (2023): employee 5%, employer 3%, total 8% of qualifying earnings.
    /// This represents the employee's contribution percentage; employer contributions are separate.
    /// </value>
    [JsonPropertyName("pension_contribution_percentage")]
    public decimal? PensionContributionPercentage { get; init; }

    /// <summary>
    /// Gets the type of student loan repayment plan for this employee.
    /// </summary>
    /// <value>
    /// The student loan plan type if applicable (e.g., "Plan 1", "Plan 2", "Plan 4", "Postgraduate").
    /// Different plans have different repayment thresholds and rates:
    /// <list type="bullet">
    /// <item><b>Plan 1</b> - Pre-2012 English/Welsh students, Scottish students, NI students (threshold ~£22,015)</item>
    /// <item><b>Plan 2</b> - Post-2012 English/Welsh students (threshold ~£27,295)</item>
    /// <item><b>Plan 4</b> - Scottish students (threshold varies)</item>
    /// <item><b>Postgraduate</b> - Postgraduate loans (threshold ~£21,000)</item>
    /// </list>
    /// Null if the employee has no student loan.
    /// </value>
    [JsonPropertyName("student_loan_type")]
    public string? StudentLoanType { get; init; }

    /// <summary>
    /// Gets the date and time when this payroll profile was created.
    /// </summary>
    /// <value>
    /// The timestamp of when the payroll profile was first set up in FreeAgent.
    /// </value>
    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this payroll profile was last updated.
    /// </summary>
    /// <value>
    /// The timestamp of the most recent modification to the payroll profile.
    /// Tax codes, NI letters, and salaries may change during employment and require profile updates.
    /// </value>
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }
}