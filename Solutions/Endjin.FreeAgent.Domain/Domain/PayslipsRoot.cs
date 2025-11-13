// <copyright file="PayslipsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="Payslip"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple payslips.
/// </remarks>
/// <seealso cref="Payslip"/>
public record PayslipsRoot
{
    /// <summary>
    /// Gets the collection of payslips from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="Payslip"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("payslips")]
    public List<Payslip> Payslips { get; init; } = [];
}