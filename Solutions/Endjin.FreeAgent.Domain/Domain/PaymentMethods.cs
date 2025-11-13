// <copyright file="PaymentMethods.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the payment methods configuration for a company in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Payment methods specify how customers can pay invoices, including bank transfer details and PayPal email address.
/// This information appears on invoices sent to customers and can be configured at the company level.
/// </para>
/// <para>
/// Companies can provide traditional bank account details for direct bank transfers and/or a PayPal email address
/// for online payments. At least one payment method should be configured for customer convenience.
/// </para>
/// </remarks>
/// <seealso cref="Company"/>
/// <seealso cref="BankAccountDetails"/>
public record PaymentMethods
{
    /// <summary>
    /// Gets the bank account details for receiving payments via bank transfer.
    /// </summary>
    /// <value>
    /// The <see cref="BankAccountDetails"/> containing account number, sort code, and other banking information
    /// that customers need to make direct bank payments.
    /// </value>
    [JsonPropertyName("bank_account")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public BankAccountDetails? BankAccount { get; init; }

    /// <summary>
    /// Gets the PayPal email address for receiving payments via PayPal.
    /// </summary>
    /// <value>
    /// The email address associated with the company's PayPal account for online payment processing.
    /// </value>
    [JsonPropertyName("paypal_email")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? PaypalEmail { get; init; }
}