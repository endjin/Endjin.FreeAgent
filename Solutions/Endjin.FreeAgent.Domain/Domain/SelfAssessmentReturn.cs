// <copyright file="SelfAssessmentReturn.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a Self Assessment tax return for an individual in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Self Assessment tax returns (SA100) are annual submissions to HM Revenue &amp; Customs (HMRC) reporting an
/// individual's income, gains, and tax liability. Self-employed individuals, partners, company directors taking
/// dividends, and those with significant untaxed income must complete a Self Assessment return.
/// </para>
/// <para>
/// Key aspects of Self Assessment returns:
/// - Filed annually for the tax year (6 April to 5 April)
/// - Must be filed by 31 October (paper) or 31 January (online) following the tax year end
/// - Tax must be paid by 31 January following the tax year end
/// - May include payments on account for the following year
/// - Covers income tax, National Insurance, Capital Gains Tax, and Student Loan repayments
/// </para>
/// <para>
/// The return tracks various tax components including income tax, National Insurance contributions (Class 2 and Class 4
/// for self-employed), Capital Gains Tax, and Student Loan repayments, along with payments on account for the next tax year.
/// </para>
/// <para>
/// API Endpoint: /v2/self_assessment_returns
/// </para>
/// <para>
/// Minimum Access Level: Full Access
/// </para>
/// </remarks>
/// <seealso cref="SelfAssessmentReturnFiling"/>
/// <seealso cref="User"/>
/// <seealso cref="Expense"/>
public record SelfAssessmentReturn
{
    /// <summary>
    /// Gets the unique URI identifier for this Self Assessment return.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this Self Assessment return in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the URI reference to the user (individual) for whom this Self Assessment return is filed.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.User"/> who is the taxpayer for this Self Assessment return.
    /// </value>
    [JsonPropertyName("user")]
    public Uri? User { get; init; }

    /// <summary>
    /// Gets the start date of the tax year for this Self Assessment return.
    /// </summary>
    /// <value>
    /// The first date of the UK tax year covered by this return, typically 6 April.
    /// </value>
    [JsonPropertyName("period_starts_on")]
    public DateOnly? PeriodStartsOn { get; init; }

    /// <summary>
    /// Gets the end date of the tax year for this Self Assessment return.
    /// </summary>
    /// <value>
    /// The last date of the UK tax year covered by this return, typically 5 April.
    /// This date determines the tax year (e.g., 2023/24).
    /// </value>
    [JsonPropertyName("period_ends_on")]
    public DateOnly? PeriodEndsOn { get; init; }

    /// <summary>
    /// Gets the status of this Self Assessment return.
    /// </summary>
    /// <value>
    /// The current status, such as "Draft", "Filed", "Overdue", or "Paid".
    /// </value>
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    /// <summary>
    /// Gets the amount of Income Tax due for this tax year.
    /// </summary>
    /// <value>
    /// The calculated Income Tax liability on employment income, self-employment profits, rental income,
    /// dividends, and other taxable income after personal allowances and reliefs.
    /// </value>
    [JsonPropertyName("income_tax_due")]
    public decimal? IncomeTaxDue { get; init; }

    /// <summary>
    /// Gets the amount of National Insurance contributions due for this tax year.
    /// </summary>
    /// <value>
    /// The calculated National Insurance liability including Class 2 (flat rate for self-employed)
    /// and Class 4 (profit-related for self-employed) contributions.
    /// </value>
    [JsonPropertyName("national_insurance_due")]
    public decimal? NationalInsuranceDue { get; init; }

    /// <summary>
    /// Gets the amount of Capital Gains Tax due for this tax year.
    /// </summary>
    /// <value>
    /// The tax due on profits from selling assets such as property (excluding main residence),
    /// shares, business assets, or valuable personal possessions above the annual CGT allowance.
    /// </value>
    [JsonPropertyName("capital_gains_tax_due")]
    public decimal? CapitalGainsTaxDue { get; init; }

    /// <summary>
    /// Gets the amount of Student Loan repayment due for this tax year.
    /// </summary>
    /// <value>
    /// The calculated Student Loan repayment based on income above the repayment threshold.
    /// Applies to Plan 1, Plan 2, Plan 4, or Postgraduate Loan holders.
    /// </value>
    [JsonPropertyName("student_loan_repayment_due")]
    public decimal? StudentLoanRepaymentDue { get; init; }

    /// <summary>
    /// Gets the total tax and contributions due for this tax year.
    /// </summary>
    /// <value>
    /// The sum of Income Tax, National Insurance, Capital Gains Tax, and Student Loan repayments
    /// due for this tax year. This is the total amount owed to HMRC.
    /// </value>
    [JsonPropertyName("total_tax_due")]
    public decimal? TotalTaxDue { get; init; }

    /// <summary>
    /// Gets the payments on account due for the following tax year.
    /// </summary>
    /// <value>
    /// Advance payments toward next year's tax bill, typically required if the previous year's tax bill
    /// exceeded £1,000. Paid in two installments (31 January and 31 July).
    /// </value>
    [JsonPropertyName("payments_on_account_due")]
    public decimal? PaymentsOnAccountDue { get; init; }

    /// <summary>
    /// Gets the date when this Self Assessment return was filed with HMRC.
    /// </summary>
    /// <value>
    /// The date the SA100 return was submitted to HMRC, either electronically or on paper.
    /// Must be by 31 October (paper) or 31 January (online) following the tax year end.
    /// </value>
    [JsonPropertyName("filed_on")]
    public DateOnly? FiledOn { get; init; }

    /// <summary>
    /// Gets a value indicating whether this Self Assessment return was filed online.
    /// </summary>
    /// <value>
    /// <c>true</c> if the return was submitted electronically through HMRC's online services;
    /// <c>false</c> if it was filed on paper or marked as manually filed.
    /// </value>
    [JsonPropertyName("filed_online")]
    public bool? FiledOnline { get; init; }

    /// <summary>
    /// Gets the Unique Taxpayer Reference (UTR) number for this individual.
    /// </summary>
    /// <value>
    /// The 10-digit UTR number issued by HMRC to uniquely identify this taxpayer for Self Assessment purposes.
    /// </value>
    [JsonPropertyName("utr_number")]
    public string? UtrNumber { get; init; }

    /// <summary>
    /// Gets the date and time when this Self Assessment return record was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing when this return was first created in the FreeAgent system.
    /// </value>
    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this Self Assessment return record was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing the last modification timestamp for this return.
    /// </value>
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }
}