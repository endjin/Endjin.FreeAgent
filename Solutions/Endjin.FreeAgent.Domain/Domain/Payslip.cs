// <copyright file="Payslip.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents an employee payslip in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Payslips record employee salary payments including gross pay, deductions, and net pay. They track all
/// statutory deductions required by UK employment law including Income Tax (PAYE), National Insurance
/// contributions, pension contributions, and Student Loan repayments.
/// </para>
/// <para>
/// Each payslip shows:
/// - Gross Salary: Total pay before deductions
/// - Employee Deductions: Income Tax, Employee NIC, pension contributions, Student Loan repayments
/// - Employer Costs: Employer NIC and pension contributions (not deducted from employee)
/// - Net Salary: Take-home pay after all employee deductions
/// </para>
/// <para>
/// Payslips are essential for payroll compliance, providing employees with records of their earnings and
/// deductions, and giving employers data for RTI (Real Time Information) submissions to HMRC, pension
/// contributions, and accurate expense recording.
/// </para>
/// <para>
/// API Endpoint: /v2/payslips
/// </para>
/// <para>
/// Minimum Access Level: Payroll
/// </para>
/// </remarks>
/// <seealso cref="User"/>
/// <seealso cref="PayrollPayment"/>
public record Payslip
{
    /// <summary>
    /// Gets the unique URI identifier for this payslip.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this payslip in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the URI reference to the employee (user) for this payslip.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.User"/> who received this salary payment.
    /// </value>
    [JsonPropertyName("user")]
    public Uri? User { get; init; }

    /// <summary>
    /// Gets the gross salary before any deductions.
    /// </summary>
    /// <value>
    /// The total salary amount before Income Tax, National Insurance, pension contributions,
    /// and Student Loan repayments are deducted.
    /// </value>
    [JsonPropertyName("gross_salary")]
    public decimal? GrossSalary { get; init; }

    /// <summary>
    /// Gets the net salary (take-home pay) after all deductions.
    /// </summary>
    /// <value>
    /// The amount actually paid to the employee after Income Tax, National Insurance, pension contributions,
    /// and Student Loan repayments have been deducted from gross salary.
    /// </value>
    [JsonPropertyName("net_salary")]
    public decimal? NetSalary { get; init; }

    /// <summary>
    /// Gets the Income Tax (PAYE) deducted from this payslip.
    /// </summary>
    /// <value>
    /// The amount of Income Tax withheld under the Pay As You Earn (PAYE) system,
    /// calculated based on the employee's tax code and earnings.
    /// </value>
    [JsonPropertyName("income_tax")]
    public decimal? IncomeTax { get; init; }

    /// <summary>
    /// Gets the employee National Insurance contribution deducted from this payslip.
    /// </summary>
    /// <value>
    /// The employee's National Insurance contribution (Class 1 employee NIC) deducted from gross salary,
    /// calculated based on earnings thresholds and NIC rates.
    /// </value>
    [JsonPropertyName("employee_nic")]
    public decimal? EmployeeNic { get; init; }

    /// <summary>
    /// Gets the employee pension contribution deducted from this payslip.
    /// </summary>
    /// <value>
    /// The employee's contribution to the workplace pension scheme, deducted from gross salary.
    /// This may include auto-enrolment pension contributions.
    /// </value>
    [JsonPropertyName("employee_pension")]
    public decimal? EmployeePension { get; init; }

    /// <summary>
    /// Gets the employer National Insurance contribution for this payslip.
    /// </summary>
    /// <value>
    /// The employer's National Insurance contribution (Class 1 employer NIC) paid on top of gross salary.
    /// This is an employer cost, not deducted from the employee's pay.
    /// </value>
    [JsonPropertyName("employer_nic")]
    public decimal? EmployerNic { get; init; }

    /// <summary>
    /// Gets the employer pension contribution for this payslip.
    /// </summary>
    /// <value>
    /// The employer's contribution to the workplace pension scheme, paid in addition to the employee's gross salary.
    /// This is an employer cost, not deducted from the employee's pay.
    /// </value>
    [JsonPropertyName("employer_pension")]
    public decimal? EmployerPension { get; init; }

    /// <summary>
    /// Gets the Student Loan repayment deducted from this payslip.
    /// </summary>
    /// <value>
    /// The Student Loan repayment deducted from gross salary when earnings exceed the repayment threshold.
    /// Applies to Plan 1, Plan 2, Plan 4, or Postgraduate Loan holders.
    /// </value>
    [JsonPropertyName("student_loan")]
    public decimal? StudentLoan { get; init; }

    /// <summary>
    /// Gets the date this payslip is for (the pay period end date).
    /// </summary>
    /// <value>
    /// The date of the salary payment or the end date of the pay period covered by this payslip.
    /// </value>
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; init; }
}