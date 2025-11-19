// <copyright file="VatReturnPayment.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a payment obligation within a VAT return in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// VAT return payments represent the individual payment obligations that arise from a VAT return.
/// These can include the main VAT payment or refund due, and potentially additional payments
/// depending on the VAT scheme and period.
/// </para>
/// <para>
/// Each payment has its own due date and can be tracked as paid or unpaid through the FreeAgent API.
/// The payment endpoints allow marking payments as paid or unpaid:
/// </para>
/// <list type="bullet">
/// <item>PUT /v2/vat_returns/:period_ends_on/payments/:payment_date/mark_as_paid</item>
/// <item>PUT /v2/vat_returns/:period_ends_on/payments/:payment_date/mark_as_unpaid</item>
/// </list>
/// <para>
/// Minimum Access Level: Tax, Accounting &amp; Users
/// </para>
/// </remarks>
/// <seealso cref="VatReturn"/>
public record VatReturnPayment
{
    /// <summary>
    /// Gets the descriptive label for this payment obligation.
    /// </summary>
    /// <value>
    /// A human-readable description of the payment, such as "VAT Payment" or "VAT Refund".
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
    /// A negative value indicates a refund is due from HMRC.
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
