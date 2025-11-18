// <copyright file="Payslip.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents an employee payslip in the FreeAgent RTI (Real Time Information) payroll system.
/// </summary>
/// <remarks>
/// <para>
/// Payslips contain detailed salary information for employees including basic pay, all types of statutory
/// payments, deductions, and pension contributions. This data is used for UK RTI submissions to HMRC.
/// </para>
/// <para>
/// Payslips are accessed through payroll periods via GET /v2/payroll/:year/:period and contain comprehensive
/// UK payroll information including:
/// <list type="bullet">
/// <item>Basic pay and additional earnings (overtime, commission, bonus, allowance)</item>
/// <item>Statutory payments (SSP, SMP, SPP, SAP, SPBP, SNCP)</item>
/// <item>Tax deductions (PAYE, student loans, postgraduate loans)</item>
/// <item>National Insurance contributions (employee and employer)</item>
/// <item>Pension contributions (employee and employer)</item>
/// <item>Other deductions and salary sacrifice arrangements</item>
/// </list>
/// </para>
/// <para>
/// API Access: Read-only via GET /v2/payroll/:year/:period
/// Minimum Access Level: Tax and Limited Accounting. Only available for UK companies.
/// </para>
/// </remarks>
/// <seealso cref="PayrollPeriod"/>
/// <seealso cref="PayrollPayment"/>
public record Payslip
{
    /// <summary>
    /// Gets the URI reference to the employee (user) for this payslip.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="User"/> who received this salary payment.
    /// </value>
    [JsonPropertyName("user")]
    public Uri? User { get; init; }

    /// <summary>
    /// Gets the employee's tax code.
    /// </summary>
    /// <value>
    /// The HMRC tax code (e.g., "1257L", "BR", "D0") determining the tax-free allowance and tax bands.
    /// </value>
    [JsonPropertyName("tax_code")]
    public string? TaxCode { get; init; }

    /// <summary>
    /// Gets the date for this payslip.
    /// </summary>
    /// <value>
    /// The pay date in YYYY-MM-DD format.
    /// </value>
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; init; }

    /// <summary>
    /// Gets the basic pay amount.
    /// </summary>
    /// <value>
    /// The employee's basic salary for this pay period before any additions or deductions.
    /// </value>
    [JsonPropertyName("basic_pay")]
    public decimal? BasicPay { get; init; }

    /// <summary>
    /// Gets the tax deducted from this payslip.
    /// </summary>
    /// <value>
    /// The PAYE income tax amount withheld based on the employee's tax code and earnings.
    /// </value>
    [JsonPropertyName("tax_deducted")]
    public decimal? TaxDeducted { get; init; }

    /// <summary>
    /// Gets the employee's National Insurance contribution.
    /// </summary>
    /// <value>
    /// The employee's Class 1 NIC deduction calculated based on earnings and NI category letter.
    /// </value>
    [JsonPropertyName("employee_ni")]
    public decimal? EmployeeNi { get; init; }

    /// <summary>
    /// Gets the employer's National Insurance contribution.
    /// </summary>
    /// <value>
    /// The employer's Class 1 NIC paid on top of gross salary.
    /// </value>
    [JsonPropertyName("employer_ni")]
    public decimal? EmployerNi { get; init; }

    /// <summary>
    /// Gets other deductions from gross pay.
    /// </summary>
    /// <value>
    /// Additional deductions from gross pay not covered by other specific fields.
    /// </value>
    [JsonPropertyName("other_deductions")]
    public decimal? OtherDeductions { get; init; }

    /// <summary>
    /// Gets the student loan deduction.
    /// </summary>
    /// <value>
    /// The student loan repayment deducted based on the employee's loan plan and salary threshold.
    /// </value>
    [JsonPropertyName("student_loan_deduction")]
    public decimal? StudentLoanDeduction { get; init; }

    /// <summary>
    /// Gets the postgraduate loan deduction.
    /// </summary>
    /// <value>
    /// The postgraduate loan repayment deducted from this payslip.
    /// </value>
    [JsonPropertyName("postgrad_loan_deduction")]
    public decimal? PostgradLoanDeduction { get; init; }

    /// <summary>
    /// Gets the overtime payment.
    /// </summary>
    /// <value>
    /// Payment for overtime hours worked.
    /// </value>
    [JsonPropertyName("overtime")]
    public decimal? Overtime { get; init; }

    /// <summary>
    /// Gets the commission payment.
    /// </summary>
    /// <value>
    /// Commission earned for this pay period.
    /// </value>
    [JsonPropertyName("commission")]
    public decimal? Commission { get; init; }

    /// <summary>
    /// Gets the bonus payment.
    /// </summary>
    /// <value>
    /// Bonus payment for this pay period.
    /// </value>
    [JsonPropertyName("bonus")]
    public decimal? Bonus { get; init; }

    /// <summary>
    /// Gets the allowance payment.
    /// </summary>
    /// <value>
    /// Any allowances paid (e.g., travel, equipment).
    /// </value>
    [JsonPropertyName("allowance")]
    public decimal? Allowance { get; init; }

    /// <summary>
    /// Gets the Statutory Sick Pay (SSP) amount.
    /// </summary>
    /// <value>
    /// SSP paid to the employee for qualifying sick days.
    /// </value>
    [JsonPropertyName("statutory_sick_pay")]
    public decimal? StatutorySickPay { get; init; }

    /// <summary>
    /// Gets the Statutory Maternity Pay (SMP) amount.
    /// </summary>
    /// <value>
    /// SMP paid to eligible employees during maternity leave.
    /// </value>
    [JsonPropertyName("statutory_maternity_pay")]
    public decimal? StatutoryMaternityPay { get; init; }

    /// <summary>
    /// Gets the Statutory Paternity Pay (SPP) amount.
    /// </summary>
    /// <value>
    /// SPP paid to eligible employees during paternity leave.
    /// </value>
    [JsonPropertyName("statutory_paternity_pay")]
    public decimal? StatutoryPaternityPay { get; init; }

    /// <summary>
    /// Gets the Statutory Adoption Pay (SAP) amount.
    /// </summary>
    /// <value>
    /// SAP paid to eligible employees during adoption leave.
    /// </value>
    [JsonPropertyName("statutory_adoption_pay")]
    public decimal? StatutoryAdoptionPay { get; init; }

    /// <summary>
    /// Gets the Statutory Parental Bereavement Pay (SPBP) amount.
    /// </summary>
    /// <value>
    /// SPBP paid to eligible employees following the death of a child.
    /// </value>
    [JsonPropertyName("statutory_parental_bereavement_pay")]
    public decimal? StatutoryParentalBereavementPay { get; init; }

    /// <summary>
    /// Gets the Statutory Neonatal Care Pay (SNCP) amount.
    /// </summary>
    /// <value>
    /// SNCP paid to eligible employees when their baby requires neonatal care.
    /// </value>
    [JsonPropertyName("statutory_neonatal_care_pay")]
    public decimal? StatutoryNeonatalCarePay { get; init; }

    /// <summary>
    /// Gets the absence payments amount.
    /// </summary>
    /// <value>
    /// Payments for other types of absence.
    /// </value>
    [JsonPropertyName("absence_payments")]
    public decimal? AbsencePayments { get; init; }

    /// <summary>
    /// Gets other payments not covered by specific fields.
    /// </summary>
    /// <value>
    /// Miscellaneous payments added to gross pay.
    /// </value>
    [JsonPropertyName("other_payments")]
    public decimal? OtherPayments { get; init; }

    /// <summary>
    /// Gets the employee's pension contribution.
    /// </summary>
    /// <value>
    /// The employee's pension contribution deducted from pay.
    /// </value>
    [JsonPropertyName("employee_pension")]
    public decimal? EmployeePension { get; init; }

    /// <summary>
    /// Gets the employer's pension contribution.
    /// </summary>
    /// <value>
    /// The employer's pension contribution paid on behalf of the employee.
    /// </value>
    [JsonPropertyName("employer_pension")]
    public decimal? EmployerPension { get; init; }

    /// <summary>
    /// Gets the attachment of earnings deduction.
    /// </summary>
    /// <value>
    /// Deductions made under an attachment of earnings order (e.g., court-ordered repayments).
    /// </value>
    [JsonPropertyName("attachments")]
    public decimal? Attachments { get; init; }

    /// <summary>
    /// Gets the payroll giving donation.
    /// </summary>
    /// <value>
    /// Charitable donations made through payroll giving (Give As You Earn).
    /// </value>
    [JsonPropertyName("payroll_giving")]
    public decimal? PayrollGiving { get; init; }

    /// <summary>
    /// Gets the NI calculation type.
    /// </summary>
    /// <value>
    /// The NI calculation method: "Director" or "Employee". Directors may have annual NI calculations.
    /// </value>
    [JsonPropertyName("ni_calc_type")]
    public string? NiCalcType { get; init; }

    /// <summary>
    /// Gets the pay frequency.
    /// </summary>
    /// <value>
    /// The frequency of payment (e.g., "Monthly", "Weekly").
    /// </value>
    [JsonPropertyName("frequency")]
    public string? Frequency { get; init; }

    /// <summary>
    /// Gets the Additional Statutory Paternity Pay (ASPP) amount.
    /// </summary>
    /// <value>
    /// ASPP paid when the mother/primary adopter returns to work early.
    /// </value>
    [JsonPropertyName("additional_statutory_paternity_pay")]
    public decimal? AdditionalStatutoryPaternityPay { get; init; }

    /// <summary>
    /// Gets deductions subject to NIC but not PAYE.
    /// </summary>
    /// <value>
    /// Deductions that reduce taxable pay but not NICable pay.
    /// </value>
    [JsonPropertyName("deductions_subject_to_nic_but_not_paye")]
    public decimal? DeductionsSubjectToNicButNotPaye { get; init; }

    /// <summary>
    /// Gets other deductions from net pay.
    /// </summary>
    /// <value>
    /// Deductions taken after tax and NI have been calculated.
    /// </value>
    [JsonPropertyName("other_deductions_from_net_pay")]
    public decimal? OtherDeductionsFromNetPay { get; init; }

    /// <summary>
    /// Gets employee pension contributions not under Net Pay Arrangement.
    /// </summary>
    /// <value>
    /// Pension contributions made outside of salary sacrifice or net pay arrangements.
    /// </value>
    [JsonPropertyName("employee_pension_not_under_net_pay")]
    public decimal? EmployeePensionNotUnderNetPay { get; init; }

    /// <summary>
    /// Gets other salary sacrifice deductions.
    /// </summary>
    /// <value>
    /// Salary sacrifice arrangements other than pension (e.g., cycle to work, childcare vouchers).
    /// </value>
    [JsonPropertyName("other_salary_sacrifice_deductions")]
    public decimal? OtherSalarySacrificeDeductions { get; init; }

    /// <summary>
    /// Gets employee pension via salary sacrifice.
    /// </summary>
    /// <value>
    /// Pension contributions made through salary sacrifice arrangement.
    /// </value>
    [JsonPropertyName("employee_pension_salary_sacrifice")]
    public decimal? EmployeePensionSalarySacrifice { get; init; }

    /// <summary>
    /// Gets the National Insurance category letter.
    /// </summary>
    /// <value>
    /// The NI category letter (e.g., "A", "B", "C", "H", "M") determining contribution rates.
    /// </value>
    [JsonPropertyName("ni_letter")]
    public string? NiLetter { get; init; }

    /// <summary>
    /// Gets whether student loan deductions apply.
    /// </summary>
    /// <value>
    /// True if student loan deductions should be made from this employee's pay.
    /// </value>
    [JsonPropertyName("deduct_student_loan")]
    public bool? DeductStudentLoan { get; init; }

    /// <summary>
    /// Gets the student loan deduction plan.
    /// </summary>
    /// <value>
    /// The student loan plan type: "Plan 1" or "Plan 2". Only present when <see cref="DeductStudentLoan"/> is true.
    /// </value>
    [JsonPropertyName("student_loan_deductions_plan")]
    public string? StudentLoanDeductionsPlan { get; init; }

    /// <summary>
    /// Gets whether postgraduate loan deductions apply.
    /// </summary>
    /// <value>
    /// True if postgraduate loan deductions should be made from this employee's pay.
    /// </value>
    [JsonPropertyName("deduct_postgrad_loan")]
    public bool? DeductPostgradLoan { get; init; }

    /// <summary>
    /// Gets whether Week 1/Month 1 basis applies.
    /// </summary>
    /// <value>
    /// True if tax is calculated on a non-cumulative (Week 1/Month 1) basis rather than cumulatively.
    /// This is sometimes used when an employee's tax code is uncertain.
    /// </value>
    [JsonPropertyName("week_1_month_1_basis")]
    public bool? Week1Month1Basis { get; init; }

    /// <summary>
    /// Gets the tax-free pay (deduction free pay) for this period.
    /// </summary>
    /// <value>
    /// The amount of pay that is free from tax deduction for this period.
    /// </value>
    [JsonPropertyName("deduction_free_pay")]
    public decimal? DeductionFreePay { get; init; }

    /// <summary>
    /// Gets the hours worked in this pay period.
    /// </summary>
    /// <value>
    /// The number of hours worked, used for National Minimum Wage compliance.
    /// </value>
    [JsonPropertyName("hours_worked")]
    public decimal? HoursWorked { get; init; }

    /// <summary>
    /// Gets the date and time when this payslip was created.
    /// </summary>
    /// <value>
    /// The timestamp of when the payslip was created in FreeAgent.
    /// </value>
    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this payslip was last updated.
    /// </summary>
    /// <value>
    /// The timestamp of the most recent modification to this payslip.
    /// </value>
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }
}
