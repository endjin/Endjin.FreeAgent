// <copyright file="PayrollPaymentRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON request/response wrapper for a <see cref="Domain.PayrollPayment"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON serialization and deserialization of FreeAgent API operations with payroll payment records.
/// </remarks>
/// <seealso cref="PayrollPayment"/>
public record PayrollPaymentRoot
{
    /// <summary>
    /// Gets the payroll payment from the API request/response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.PayrollPayment"/> object for the API transaction.
    /// </value>
    [JsonPropertyName("payroll_payment")]
    public PayrollPayment? PayrollPayment { get; init; }
}