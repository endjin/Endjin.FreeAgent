// <copyright file="InvoicePaymentRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON request wrapper for an <see cref="Domain.InvoicePayment"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON serialization when recording invoice payment transactions via the FreeAgent API.
/// </remarks>
/// <seealso cref="InvoicePayment"/>
/// <seealso cref="Invoice"/>
public record InvoicePaymentRoot
{
    /// <summary>
    /// Gets the invoice payment data for the API request.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.InvoicePayment"/> object containing payment details.
    /// </value>
    [JsonPropertyName("invoice")]
    public InvoicePayment? Invoice { get; init; }
}