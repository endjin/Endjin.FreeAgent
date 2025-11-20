// <copyright file="CorporationTaxReturn.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Converters;

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
/// Minimum Access Level: Tax, Accounting &amp; Users
/// </para>
/// </remarks>
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
    public required Uri Url { get; init; }

    /// <summary>
    /// Gets the start date of the accounting period for this Corporation Tax return.
    /// </summary>
    /// <value>
    /// The first date of the company's accounting period covered by this tax return.
    /// </value>
    [JsonPropertyName("period_starts_on")]
    public required DateOnly PeriodStartsOn { get; init; }

    /// <summary>
    /// Gets the end date of the accounting period for this Corporation Tax return.
    /// </summary>
    /// <value>
    /// The last date of the company's accounting period covered by this tax return.
    /// This date determines filing and payment deadlines.
    /// </value>
    [JsonPropertyName("period_ends_on")]
    public required DateOnly PeriodEndsOn { get; init; }

    /// <summary>
    /// Gets the filing status of this Corporation Tax return.
    /// </summary>
    /// <value>
    /// The current filing status: draft, unfiled, pending, rejected, filed, or marked_as_filed.
    /// </value>
    [JsonPropertyName("filing_status")]
    [JsonConverter(typeof(CorporationTaxFilingStatusNonNullableJsonConverter))]
    public required CorporationTaxFilingStatus FilingStatus { get; init; }

    /// <summary>
    /// Gets the payment status of this Corporation Tax return.
    /// </summary>
    /// <value>
    /// The payment status: unpaid or marked_as_paid. This field is omitted if no payment is required.
    /// </value>
    [JsonPropertyName("payment_status")]
    [JsonConverter(typeof(CorporationTaxPaymentStatusJsonConverter))]
    public CorporationTaxPaymentStatus? PaymentStatus { get; init; }

    /// <summary>
    /// Gets the amount of Corporation Tax due for payment.
    /// </summary>
    /// <value>
    /// The payment amount required for this Corporation Tax return. Read-only.
    /// This field is omitted if no payment is required.
    /// </value>
    [JsonPropertyName("amount_due")]
    public decimal? AmountDue { get; init; }

    /// <summary>
    /// Gets the date and time when this Corporation Tax return was filed with HMRC.
    /// </summary>
    /// <value>
    /// The timestamp when the CT600 return was submitted to HMRC. Read-only, set automatically when filed online.
    /// </value>
    [JsonPropertyName("filed_at")]
    public DateTimeOffset? FiledAt { get; init; }

    /// <summary>
    /// Gets the reference number for this filed Corporation Tax return.
    /// </summary>
    /// <value>
    /// The IRMark (HMRC's digital receipt/signature) for this filed return.
    /// Read-only, set automatically when filed online.
    /// </value>
    [JsonPropertyName("filed_reference")]
    public string? FiledReference { get; init; }

    /// <summary>
    /// Gets the deadline date for paying the Corporation Tax due.
    /// </summary>
    /// <value>
    /// The date by which the tax payment must be received by HMRC, typically 9 months and 1 day
    /// after the accounting period end date. Late payment incurs interest charges.
    /// This field is omitted if no payment is required.
    /// </value>
    [JsonPropertyName("payment_due_on")]
    public DateOnly? PaymentDueOn { get; init; }

    /// <summary>
    /// Gets the deadline date for filing the Corporation Tax return.
    /// </summary>
    /// <value>
    /// The submission deadline for the Corporation Tax return. Read-only.
    /// </value>
    [JsonPropertyName("filing_due_on")]
    public required DateOnly FilingDueOn { get; init; }
}