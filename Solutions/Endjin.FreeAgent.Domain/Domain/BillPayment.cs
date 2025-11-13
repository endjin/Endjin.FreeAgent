// <copyright file="BillPayment.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a payment made against a bill in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Bill payments record when and how the company pays its supplier bills. Payments are associated with
/// specific bank accounts and dates, allowing proper expense tracking and bill reconciliation.
/// Multiple payments can be applied to a single bill for partial payment scenarios.
/// </para>
/// <para>
/// API Endpoint: /v2/bill_payments
/// </para>
/// <para>
/// Minimum Access Level: Bills
/// </para>
/// </remarks>
/// <seealso cref="Bill"/>
/// <seealso cref="BankAccount"/>
public record BillPayment
{
    /// <summary>
    /// Gets the date when the payment was made.
    /// </summary>
    /// <value>
    /// The payment date in YYYY-MM-DD format. This field is required when recording a payment.
    /// </value>
    [JsonPropertyName("paid_on")]
    public string? PaidOn { get; init; }

    /// <summary>
    /// Gets the URI reference to the bank account from which the payment was made.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.BankAccount"/> from which the payment was withdrawn.
    /// This field is required when recording a payment.
    /// </value>
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; init; }
}