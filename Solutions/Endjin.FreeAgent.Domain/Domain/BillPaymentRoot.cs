// <copyright file="BillPaymentRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON request/response wrapper for a <see cref="Domain.BillPayment"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON serialization and deserialization of FreeAgent API requests and responses
/// that work with bill payment records.
/// </remarks>
/// <seealso cref="BillPayment"/>
/// <seealso cref="Bill"/>
public record BillPaymentRoot
{
    /// <summary>
    /// Gets the bill payment from the API request/response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.BillPayment"/> object for the API transaction.
    /// </value>
    [JsonPropertyName("bill")]
    public BillPayment? Bill { get; init; }
}