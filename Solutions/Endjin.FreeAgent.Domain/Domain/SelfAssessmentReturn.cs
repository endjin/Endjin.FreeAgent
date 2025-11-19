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
/// </para>
/// <list type="bullet">
/// <item>Filed annually for the tax year (6 April to 5 April)</item>
/// <item>Must be filed by 31 October (paper) or 31 January (online) following the tax year end</item>
/// <item>Tax must be paid by 31 January following the tax year end</item>
/// <item>May include payments on account for the following year</item>
/// </list>
/// <para>
/// The return tracks payment deadlines and filing status for compliance monitoring.
/// </para>
/// <para>
/// API Endpoint: /v2/users/:user_id/self_assessment_returns
/// </para>
/// <para>
/// Minimum Access Level: Tax, Accounting &amp; Users
/// </para>
/// </remarks>
/// <seealso cref="SelfAssessmentPayment"/>
/// <seealso cref="User"/>
public record SelfAssessmentReturn
{
    /// <summary>
    /// Gets the unique URI identifier for this Self Assessment return.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this Self Assessment return in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    public required Uri Url { get; init; }

    /// <summary>
    /// Gets the start date of the tax year for this Self Assessment return.
    /// </summary>
    /// <value>
    /// The first date of the UK tax year covered by this return, typically 6 April.
    /// </value>
    [JsonPropertyName("period_starts_on")]
    public required DateOnly PeriodStartsOn { get; init; }

    /// <summary>
    /// Gets the end date of the tax year for this Self Assessment return.
    /// </summary>
    /// <value>
    /// The last date of the UK tax year covered by this return, typically 5 April.
    /// This date is used to identify specific returns in API calls.
    /// </value>
    [JsonPropertyName("period_ends_on")]
    public required DateOnly PeriodEndsOn { get; init; }

    /// <summary>
    /// Gets the collection of payment obligations for this Self Assessment return.
    /// </summary>
    /// <value>
    /// A list of <see cref="SelfAssessmentPayment"/> objects representing the tax payments due,
    /// including balancing payments and payments on account. Each payment has its own due date and status.
    /// </value>
    [JsonPropertyName("payments")]
    public required List<SelfAssessmentPayment> Payments { get; init; }

    /// <summary>
    /// Gets the deadline date for filing this Self Assessment return with HMRC.
    /// </summary>
    /// <value>
    /// The date by which the SA100 return must be submitted to HMRC to avoid late filing penalties.
    /// Typically 31 January following the end of the tax year for online submissions.
    /// </value>
    [JsonPropertyName("filing_due_on")]
    public required DateOnly FilingDueOn { get; init; }

    /// <summary>
    /// Gets the filing status of this Self Assessment return.
    /// </summary>
    /// <value>
    /// The current filing status. Valid values are:
    /// <list type="bullet">
    /// <item><c>unfiled</c> - Return has not been submitted</item>
    /// <item><c>pending</c> - Submission is being processed</item>
    /// <item><c>rejected</c> - Submission was rejected by HMRC</item>
    /// <item><c>provisionally_filed</c> - Provisionally filed pending final confirmation</item>
    /// <item><c>filed</c> - Successfully filed with HMRC online</item>
    /// <item><c>marked_as_filed</c> - Manually marked as filed (e.g., paper submission)</item>
    /// </list>
    /// </value>
    [JsonPropertyName("filing_status")]
    public required string FilingStatus { get; init; }

    /// <summary>
    /// Gets the date and time when this Self Assessment return was filed online with HMRC.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing when the return was submitted to HMRC via online filing.
    /// This property is only populated for returns filed online through the FreeAgent system.
    /// </value>
    [JsonPropertyName("filed_at")]
    public DateTimeOffset? FiledAt { get; init; }

    /// <summary>
    /// Gets the IRMark reference returned by HMRC upon successful online filing.
    /// </summary>
    /// <value>
    /// The unique reference number (IRMark) provided by HMRC as confirmation of successful online submission.
    /// This property is only populated for returns filed online through the FreeAgent system.
    /// </value>
    [JsonPropertyName("filed_reference")]
    public string? FiledReference { get; init; }
}
