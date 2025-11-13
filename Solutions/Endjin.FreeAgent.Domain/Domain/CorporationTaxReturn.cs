// <copyright file="CorporationTaxReturn.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a Corporation Tax return for a limited company in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Corporation Tax returns (CT600) are annual submissions to HM Revenue &amp; Customs (HMRC) reporting the taxable
/// profits and tax liability of a UK limited company. All UK limited companies must file a Corporation Tax return
/// and pay any tax due within specific deadlines.
/// </para>
/// <para>
/// Key aspects of Corporation Tax returns:
/// - Filed annually based on the company's accounting period (typically 12 months)
/// - Tax is calculated on company profits (revenue minus allowable expenses)
/// - Returns must be filed within 12 months of the accounting period end date
/// - Tax must be paid within 9 months and 1 day of the accounting period end date
/// - Can be filed electronically through HMRC's online services
/// </para>
/// <para>
/// The return tracks the filing status, tax due, payment deadline, and HMRC references for audit purposes.
/// </para>
/// <para>
/// API Endpoint: /v2/corporation_tax_returns
/// </para>
/// <para>
/// Minimum Access Level: Full Access
/// </para>
/// </remarks>
/// <seealso cref="CorporationTaxReturnFiling"/>
/// <seealso cref="Company"/>
/// <seealso cref="ProfitAndLoss"/>
public record CorporationTaxReturn
{
    /// <summary>
    /// Gets the unique URI identifier for this Corporation Tax return.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this Corporation Tax return in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the start date of the accounting period for this Corporation Tax return.
    /// </summary>
    /// <value>
    /// The first date of the company's accounting period covered by this tax return.
    /// </value>
    [JsonPropertyName("period_starts_on")]
    public DateOnly? PeriodStartsOn { get; init; }

    /// <summary>
    /// Gets the end date of the accounting period for this Corporation Tax return.
    /// </summary>
    /// <value>
    /// The last date of the company's accounting period covered by this tax return.
    /// This date determines filing and payment deadlines.
    /// </value>
    [JsonPropertyName("period_ends_on")]
    public DateOnly? PeriodEndsOn { get; init; }

    /// <summary>
    /// Gets the status of this Corporation Tax return.
    /// </summary>
    /// <value>
    /// The current status, such as "Draft", "Filed", "Overdue", or "Paid".
    /// </value>
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    /// <summary>
    /// Gets the amount of Corporation Tax due for this period.
    /// </summary>
    /// <value>
    /// The calculated tax liability based on the company's taxable profits for the accounting period.
    /// This amount must be paid to HMRC by the payment due date.
    /// </value>
    [JsonPropertyName("tax_due")]
    public decimal? TaxDue { get; init; }

    /// <summary>
    /// Gets the date when this Corporation Tax return was filed with HMRC.
    /// </summary>
    /// <value>
    /// The date the CT600 return was submitted to HMRC, either electronically or manually.
    /// Must be within 12 months of the accounting period end date.
    /// </value>
    [JsonPropertyName("filed_on")]
    public DateOnly? FiledOn { get; init; }

    /// <summary>
    /// Gets a value indicating whether this Corporation Tax return was filed online.
    /// </summary>
    /// <value>
    /// <c>true</c> if the return was submitted electronically through HMRC's online services;
    /// <c>false</c> if it was marked as manually filed through other means.
    /// </value>
    [JsonPropertyName("filed_online")]
    public bool? FiledOnline { get; init; }

    /// <summary>
    /// Gets the HMRC reference number for this filed Corporation Tax return.
    /// </summary>
    /// <value>
    /// The unique reference number provided by HMRC upon successful submission, used for tracking and confirmation.
    /// </value>
    [JsonPropertyName("hmrc_reference")]
    public string? HmrcReference { get; init; }

    /// <summary>
    /// Gets the deadline date for paying the Corporation Tax due.
    /// </summary>
    /// <value>
    /// The date by which the tax payment must be received by HMRC, typically 9 months and 1 day
    /// after the accounting period end date. Late payment incurs interest charges.
    /// </value>
    [JsonPropertyName("payment_due_on")]
    public DateOnly? PaymentDueOn { get; init; }

    /// <summary>
    /// Gets the date and time when this Corporation Tax return record was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing when this return was first created in the FreeAgent system.
    /// </value>
    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this Corporation Tax return record was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing the last modification timestamp for this return.
    /// </value>
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }
}