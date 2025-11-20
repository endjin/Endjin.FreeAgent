// <copyright file="PayrollPeriodRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a payroll period with payslips.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of GET /v2/payroll/:year/:period responses,
/// which return a specific period including its embedded payslips.
/// </remarks>
/// <seealso cref="PayrollPeriod"/>
/// <seealso cref="Payslip"/>
public record PayrollPeriodRoot
{
    /// <summary>
    /// Gets the payroll period with embedded payslips.
    /// </summary>
    /// <value>
    /// The <see cref="PayrollPeriod"/> object containing period details and payslips.
    /// </value>
    [JsonPropertyName("period")]
    public PayrollPeriod? Period { get; init; }
}
