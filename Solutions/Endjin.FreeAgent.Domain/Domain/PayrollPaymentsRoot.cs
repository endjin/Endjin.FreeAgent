// <copyright file="PayrollPaymentsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="Domain.PayrollPayment"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple payroll payment records.
/// </remarks>
/// <seealso cref="PayrollPayment"/>
public record PayrollPaymentsRoot
{
    /// <summary>
    /// Gets the collection of payroll payments from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="Domain.PayrollPayment"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("payroll_payments")]
    public List<PayrollPayment>? PayrollPayments { get; init; }
}