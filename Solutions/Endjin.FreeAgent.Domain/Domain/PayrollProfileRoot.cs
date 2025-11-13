// <copyright file="PayrollProfileRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON request/response wrapper for a <see cref="Domain.PayrollProfile"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON serialization and deserialization of FreeAgent API operations with payroll profile records.
/// </remarks>
/// <seealso cref="PayrollProfile"/>
public record PayrollProfileRoot
{
    /// <summary>
    /// Gets the payroll profile from the API request/response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.PayrollProfile"/> object for the API transaction.
    /// </value>
    [JsonPropertyName("payroll_profile")]
    public PayrollProfile? PayrollProfile { get; init; }
}