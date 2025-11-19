// <copyright file="SelfAssessmentPayment.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a tax payment obligation within a Self Assessment return in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Self Assessment payments represent the individual tax payment obligations that arise from a tax return.
/// These typically include the balancing payment for the tax year and payments on account for the following year.
/// </para>
/// <para>
/// Common payment types include:
/// </para>
/// <list type="bullet">
/// <item>Balancing Payment - The remaining tax due for the tax year after accounting for PAYE and payments on account</item>
/// <item>First Payment on Account - 50% advance payment toward next year's tax, due 31 January</item>
/// <item>Second Payment on Account - 50% advance payment toward next year's tax, due 31 July</item>
/// </list>
/// <para>
/// Each payment has its own due date and can be tracked as paid or unpaid through the FreeAgent API.
/// </para>
/// </remarks>
/// <seealso cref="SelfAssessmentReturn"/>
public record SelfAssessmentPayment
{
    /// <summary>
    /// Gets the descriptive label for this payment obligation.
    /// </summary>
    /// <value>
    /// A description of the payment type, such as "Balancing Payment", "First Payment on Account",
    /// or "Second Payment on Account".
    /// </value>
    [JsonPropertyName("label")]
    public required string Label { get; init; }

    /// <summary>
    /// Gets the due date for this payment.
    /// </summary>
    /// <value>
    /// The date by which this payment must be made to HMRC to avoid interest charges and penalties.
    /// </value>
    [JsonPropertyName("due_on")]
    public required DateOnly DueOn { get; init; }

    /// <summary>
    /// Gets the amount due for this payment.
    /// </summary>
    /// <value>
    /// The monetary amount that must be paid to HMRC for this payment obligation.
    /// </value>
    [JsonPropertyName("amount_due")]
    public required decimal AmountDue { get; init; }

    /// <summary>
    /// Gets the payment status.
    /// </summary>
    /// <value>
    /// The current status of this payment. Valid values are:
    /// <list type="bullet">
    /// <item><c>unpaid</c> - Payment has not been made</item>
    /// <item><c>marked_as_paid</c> - Payment has been marked as paid in the system</item>
    /// </list>
    /// This property is omitted from the API response when <see cref="AmountDue"/> is zero or negative.
    /// </value>
    [JsonPropertyName("status")]
    public string? Status { get; init; }
}
