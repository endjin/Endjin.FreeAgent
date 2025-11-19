// <copyright file="VatReturn.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a VAT (Value Added Tax) return in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// VAT returns are periodic submissions to HM Revenue &amp; Customs (HMRC) reporting the VAT collected on sales
/// and the VAT paid on purchases. UK VAT-registered businesses must file returns quarterly, monthly, or annually
/// depending on their registration type.
/// </para>
/// <para>
/// Returns can be filed electronically through FreeAgent's integration with HMRC's Making Tax Digital (MTD) service
/// or manually marked as filed if submitted through other means.
/// </para>
/// <para>
/// API Endpoint: /v2/vat_returns
/// </para>
/// <para>
/// Minimum Access Level: Tax, Accounting &amp; Users (GET operations), Full Access (mark_as_filed/mark_as_unfiled)
/// </para>
/// </remarks>
/// <seealso cref="VatReturnPayment"/>
/// <seealso cref="Invoice"/>
/// <seealso cref="Bill"/>
public record VatReturn
{
    /// <summary>
    /// Gets the unique URI identifier for this VAT return.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this VAT return in the FreeAgent system.
    /// This is a read-only property set by the API.
    /// </value>
    [JsonPropertyName("url")]
    public required Uri Url { get; init; }

    /// <summary>
    /// Gets the start date of the VAT return period.
    /// </summary>
    /// <value>
    /// The first date of the accounting period covered by this VAT return.
    /// </value>
    [JsonPropertyName("period_starts_on")]
    public required DateOnly PeriodStartsOn { get; init; }

    /// <summary>
    /// Gets the end date of the VAT return period.
    /// </summary>
    /// <value>
    /// The last date of the accounting period covered by this VAT return.
    /// </value>
    [JsonPropertyName("period_ends_on")]
    public required DateOnly PeriodEndsOn { get; init; }

    /// <summary>
    /// Gets the filing due date for this VAT return.
    /// </summary>
    /// <value>
    /// The deadline by which this VAT return must be submitted to HMRC.
    /// Typically 1 month and 7 days after the period end date.
    /// </value>
    [JsonPropertyName("filing_due_on")]
    public required DateOnly FilingDueOn { get; init; }

    /// <summary>
    /// Gets the filing status of this VAT return.
    /// </summary>
    /// <value>
    /// The current filing status. Valid values are:
    /// <list type="bullet">
    /// <item><c>unfiled</c> - The return has not been filed</item>
    /// <item><c>pending</c> - The return is pending submission</item>
    /// <item><c>rejected</c> - The return was rejected by HMRC</item>
    /// <item><c>filed</c> - The return was filed online</item>
    /// <item><c>marked_as_filed</c> - The return was manually marked as filed</item>
    /// </list>
    /// </value>
    [JsonPropertyName("filing_status")]
    public required string FilingStatus { get; init; }

    /// <summary>
    /// Gets the date and time when this VAT return was filed online.
    /// </summary>
    /// <value>
    /// The date and time the return was submitted to HMRC through the MTD service.
    /// Only populated for returns filed online. This is a read-only property set by the API.
    /// </value>
    [JsonPropertyName("filed_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? FiledAt { get; init; }

    /// <summary>
    /// Gets the HMRC form bundle reference number for this filed VAT return.
    /// </summary>
    /// <value>
    /// The unique form bundle number provided by HMRC upon successful online submission,
    /// used for tracking and confirmation. This is a read-only property set by the API.
    /// </value>
    [JsonPropertyName("filed_reference")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? FiledReference { get; init; }

    /// <summary>
    /// Gets the collection of payments associated with this VAT return.
    /// </summary>
    /// <value>
    /// A list of payment obligations for this VAT period, including payment amounts and due dates.
    /// Negative amounts indicate refunds due from HMRC.
    /// </value>
    [JsonPropertyName("payments")]
    public required IList<VatReturnPayment> Payments { get; init; }
}
