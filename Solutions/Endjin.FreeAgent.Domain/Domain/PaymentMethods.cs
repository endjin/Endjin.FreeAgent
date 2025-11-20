// <copyright file="PaymentMethods.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the available online payment methods for an invoice in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// PaymentMethods indicates which online payment options are available to clients for paying an invoice.
/// This includes PayPal, GoCardless (Direct Debit and Instant Bank Pay), Stripe, and Tyl payment gateways.
/// The boolean flags show which payment methods have been configured and are available for this invoice.
/// </para>
/// <para>
/// These payment methods are configured at the company level and appear on invoices when available,
/// allowing clients to pay through their preferred online payment method.
/// </para>
/// </remarks>
/// <seealso cref="Invoice"/>
public record PaymentMethods
{
    /// <summary>
    /// Gets a value indicating whether PayPal payment is available for this invoice.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the invoice can be paid via PayPal; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("paypal")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Paypal { get; init; }

    /// <summary>
    /// Gets a value indicating whether GoCardless Direct Debit pre-authorization is available for this invoice.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the invoice can be paid via GoCardless Direct Debit with pre-authorization; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("gocardless_preauth")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? GocardlessPreauth { get; init; }

    /// <summary>
    /// Gets a value indicating whether GoCardless Instant Bank Pay is available for this invoice.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the invoice can be paid via GoCardless Instant Bank Pay; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("gocardless_instant_bank_pay")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? GocardlessInstantBankPay { get; init; }

    /// <summary>
    /// Gets a value indicating whether Stripe payment is available for this invoice.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the invoice can be paid via Stripe; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("stripe")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Stripe { get; init; }

    /// <summary>
    /// Gets a value indicating whether Tyl payment is available for this invoice.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the invoice can be paid via Tyl by NatWest; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("tyl")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Tyl { get; init; }
}