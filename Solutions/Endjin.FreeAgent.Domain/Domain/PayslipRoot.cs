// <copyright file="PayslipRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a <see cref="Domain.Payslip"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single payslip.
/// </remarks>
/// <seealso cref="Payslip"/>
public record PayslipRoot
{
    /// <summary>
    /// Gets the payslip from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.Payslip"/> object returned by the API.
    /// </value>
    [JsonPropertyName("payslip")]
    public Payslip? Payslip { get; init; }
}