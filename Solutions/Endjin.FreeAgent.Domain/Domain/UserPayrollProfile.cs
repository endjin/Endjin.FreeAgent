// <copyright file="UserPayrollProfile.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the payroll profile data for a user in the FreeAgent system.
/// </summary>
/// <remarks>
/// <para>
/// This class contains summary payroll information from previous employment that is relevant for
/// tax calculations and reporting. It is returned as part of the User resource when payroll is active.
/// </para>
/// <para>
/// This is a read-only object returned by the API; it cannot be created or updated directly.
/// </para>
/// </remarks>
public record UserPayrollProfile
{
    /// <summary>
    /// Gets the total pay from previous employment in the current tax year.
    /// </summary>
    /// <value>
    /// The cumulative gross pay amount from all previous employers in the current tax year,
    /// used for calculating tax obligations.
    /// </value>
    [JsonPropertyName("total_pay_in_previous_employment")]
    public decimal? TotalPayInPreviousEmployment { get; init; }

    /// <summary>
    /// Gets the total tax paid in previous employment in the current tax year.
    /// </summary>
    /// <value>
    /// The cumulative tax deducted by all previous employers in the current tax year,
    /// used for calculating remaining tax obligations.
    /// </value>
    [JsonPropertyName("total_tax_in_previous_employment")]
    public decimal? TotalTaxInPreviousEmployment { get; init; }
}
