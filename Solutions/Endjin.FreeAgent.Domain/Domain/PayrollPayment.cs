// <copyright file="PayrollPayment.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a payroll payment record in the FreeAgent payroll system.
/// </summary>
/// <remarks>
/// <para>
/// Payroll payments record when employees are paid, tracking gross pay, deductions (tax, NI, pension, student loans),
/// and net pay. Each payment is associated with a specific employee and payment date, providing a complete audit
/// trail of payroll transactions.
/// </para>
/// <para>
/// While <see cref="Payslip"/> represents the detailed breakdown of a pay period (showing all earnings components
/// and statutory deductions), PayrollPayment represents the actual monetary transaction when the employee is paid.
/// A payslip might cover earnings from April 1-30, while the payment records the money transfer on May 5th.
/// </para>
/// <para>
/// UK payroll components tracked:
/// <list type="bullet">
/// <item><b>Gross Pay</b> - Total earnings before deductions</item>
/// <item><b>Income Tax</b> - PAYE tax deducted based on tax code</item>
/// <item><b>Employee NI</b> - Employee's National Insurance (Class 1)</item>
/// <item><b>Employer NI</b> - Employer's National Insurance contribution (additional cost)</item>
/// <item><b>Employee Pension</b> - Employee's pension contribution (reduces net pay)</item>
/// <item><b>Employer Pension</b> - Employer's pension contribution (additional cost)</item>
/// <item><b>Student Loan</b> - Student loan repayments deducted at source</item>
/// <item><b>Net Pay</b> - Amount actually paid to employee (take-home pay)</item>
/// <item><b>Total Cost</b> - Complete employment cost (gross + employer NI + employer pension)</item>
/// </list>
/// </para>
/// <para>
/// Payroll payments are used for:
/// <list type="bullet">
/// <item>Recording when employees are paid</item>
/// <item>Tracking payroll cash flow and timing</item>
/// <item>Reconciling bank transactions with employee payments</item>
/// <item>Preparing HMRC submissions (RTI - Real Time Information)</item>
/// <item>Calculating total employment costs</item>
/// <item>Year-end payroll reporting (P60s)</item>
/// </list>
/// </para>
/// <para>
/// API Access: Accessible via GET/POST /v2/payroll_payments
/// Minimum Access Level: Payroll admin access
/// </para>
/// </remarks>
/// <seealso cref="Payslip"/>
/// <seealso cref="PayrollProfile"/>
/// <seealso cref="User"/>
public record PayrollPayment
{
    /// <summary>
    /// Gets the API URL for this payroll payment resource.
    /// </summary>
    /// <value>
    /// The unique API endpoint URL for accessing this specific payroll payment record.
    /// </value>
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the API URL of the employee (user) receiving this payment.
    /// </summary>
    /// <value>
    /// A reference URL to the <see cref="User"/> resource representing the employee being paid.
    /// </value>
    [JsonPropertyName("user")]
    public Uri? User { get; init; }

    /// <summary>
    /// Gets the date when the payment was made to the employee.
    /// </summary>
    /// <value>
    /// The payment date, typically the pay date specified in the employment contract (e.g., last working day
    /// of the month, 25th of each month). This is when money is transferred to the employee's bank account.
    /// </value>
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; init; }

    /// <summary>
    /// Gets the gross pay amount before any deductions.
    /// </summary>
    /// <value>
    /// The total earnings for the pay period before tax, NI, pension, or student loan deductions.
    /// This is the contractual salary amount or sum of all pay components.
    /// </value>
    [JsonPropertyName("gross_pay")]
    public decimal? GrossPay { get; init; }

    /// <summary>
    /// Gets the income tax (PAYE) deducted from gross pay.
    /// </summary>
    /// <value>
    /// The PAYE income tax amount withheld and paid to HMRC, calculated based on the employee's tax code
    /// and cumulative earnings for the tax year.
    /// </value>
    [JsonPropertyName("income_tax")]
    public decimal? IncomeTax { get; init; }

    /// <summary>
    /// Gets the employee's National Insurance contribution deducted from gross pay.
    /// </summary>
    /// <value>
    /// The employee's Class 1 NIC deduction, calculated based on earnings and NI category letter.
    /// This reduces the employee's net pay.
    /// </value>
    [JsonPropertyName("employee_ni")]
    public decimal? EmployeeNi { get; init; }

    /// <summary>
    /// Gets the employer's National Insurance contribution.
    /// </summary>
    /// <value>
    /// The employer's Class 1 NIC contribution paid on top of gross salary. This is an employer cost,
    /// not deducted from the employee's pay, and increases the total cost of employment.
    /// </value>
    [JsonPropertyName("employer_ni")]
    public decimal? EmployerNi { get; init; }

    /// <summary>
    /// Gets the employee's pension contribution deducted from gross pay.
    /// </summary>
    /// <value>
    /// The employee's pension contribution (typically 5% of qualifying earnings for auto-enrolment minimum).
    /// This is deducted from pay and reduces net pay.
    /// </value>
    [JsonPropertyName("employee_pension")]
    public decimal? EmployeePension { get; init; }

    /// <summary>
    /// Gets the employer's pension contribution.
    /// </summary>
    /// <value>
    /// The employer's pension contribution (typically 3% of qualifying earnings for auto-enrolment minimum).
    /// This is an employer cost paid on top of salary, not deducted from the employee's pay.
    /// </value>
    [JsonPropertyName("employer_pension")]
    public decimal? EmployerPension { get; init; }

    /// <summary>
    /// Gets the student loan repayment deducted from gross pay.
    /// </summary>
    /// <value>
    /// The student loan repayment amount deducted based on the employee's loan plan type and salary threshold.
    /// Only applies if the employee has a student loan and earnings exceed the threshold. This reduces net pay.
    /// </value>
    [JsonPropertyName("student_loan")]
    public decimal? StudentLoan { get; init; }

    /// <summary>
    /// Gets the net pay amount paid to the employee.
    /// </summary>
    /// <value>
    /// The take-home pay after all deductions. Calculated as:
    /// Gross Pay - Income Tax - Employee NI - Employee Pension - Student Loan.
    /// This is the amount actually transferred to the employee's bank account.
    /// </value>
    [JsonPropertyName("net_pay")]
    public decimal? NetPay { get; init; }

    /// <summary>
    /// Gets the total cost of employment for this payment.
    /// </summary>
    /// <value>
    /// The complete employment cost to the business. Calculated as:
    /// Gross Pay + Employer NI + Employer Pension.
    /// This represents the true cost of employing the person for this pay period.
    /// </value>
    [JsonPropertyName("total_cost")]
    public decimal? TotalCost { get; init; }

    /// <summary>
    /// Gets the date and time when this payroll payment record was created.
    /// </summary>
    /// <value>
    /// The timestamp of when the payment was recorded in FreeAgent.
    /// </value>
    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this payroll payment record was last updated.
    /// </summary>
    /// <value>
    /// The timestamp of the most recent modification to this payment record.
    /// </value>
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }
}