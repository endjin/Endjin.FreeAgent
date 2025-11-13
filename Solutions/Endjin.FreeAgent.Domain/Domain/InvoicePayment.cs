// <copyright file="InvoicePayment.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a payment applied to an invoice in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Invoice payments record when and how customers pay their invoices. Payments are associated with
/// specific bank accounts and dates, allowing proper cash flow tracking and invoice reconciliation.
/// Multiple payments can be applied to a single invoice for partial payment scenarios.
/// </para>
/// <para>
/// API Endpoint: /v2/invoice_payments
/// </para>
/// <para>
/// Minimum Access Level: Invoices
/// </para>
/// </remarks>
/// <seealso cref="Invoice"/>
/// <seealso cref="BankAccount"/>
public record InvoicePayment
{
    /// <summary>
    /// Gets the date when the payment was received.
    /// </summary>
    /// <value>
    /// The payment date in YYYY-MM-DD format. This field is required when recording a payment.
    /// </value>
    [JsonPropertyName("paid_on")]
    public string? PaidOn { get; init; }

    /// <summary>
    /// Gets the URI reference to the bank account that received the payment.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.BankAccount"/> into which the payment was deposited.
    /// This field is required when recording a payment.
    /// </value>
    [JsonPropertyName("paid_into_bank_account")]
    public string? PaidIntoBankAccount { get; init; }
}