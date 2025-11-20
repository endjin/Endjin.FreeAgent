// <copyright file="PayrollYearRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for payroll year data.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of GET /v2/payroll/:year responses,
/// which return all periods and payments for a tax year.
/// </remarks>
/// <seealso cref="PayrollPeriod"/>
/// <seealso cref="PayrollPayment"/>
public record PayrollYearRoot
{
    /// <summary>
    /// Gets the collection of payroll periods for the tax year.
    /// </summary>
    /// <value>
    /// A list of <see cref="PayrollPeriod"/> objects representing each pay period in the tax year.
    /// </value>
    [JsonPropertyName("periods")]
    public List<PayrollPeriod> Periods { get; init; } = [];

    /// <summary>
    /// Gets the collection of HMRC payments for the tax year.
    /// </summary>
    /// <value>
    /// A list of <see cref="PayrollPayment"/> objects representing amounts due to HMRC.
    /// </value>
    [JsonPropertyName("payments")]
    public List<PayrollPayment> Payments { get; init; } = [];
}
