// <copyright file="CisBand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a Construction Industry Scheme (CIS) deduction band.
/// </summary>
/// <remarks>
/// <para>
/// CIS bands define the tax deduction rates that apply to subcontractors under the UK
/// Construction Industry Scheme. Each band specifies a deduction rate and corresponding
/// accounting nominal codes for income and deduction tracking.
/// </para>
/// <para>
/// The three standard CIS bands are:
/// <list type="bullet">
/// <item><strong>cis_gross</strong> - 0% deduction rate (nominal code 061)</item>
/// <item><strong>cis_standard</strong> - 20% deduction rate (nominal code 062)</item>
/// <item><strong>cis_higher</strong> - 30% deduction rate (nominal code 063)</item>
/// </list>
/// </para>
/// <para>
/// These bands are read-only reference data provided by FreeAgent and do not change frequently.
/// Client applications are encouraged to cache this data to reduce API calls.
/// </para>
/// <para>
/// CIS bands are only available to UK companies enrolled in the Construction Industry Scheme
/// for Subcontractors and require "Estimates &amp; Invoices" access level.
/// </para>
/// <para>
/// API Endpoint: /v2/cis_bands
/// </para>
/// </remarks>
/// <seealso cref="Bill"/>
/// <seealso cref="Invoice"/>
public record CisBand
{
    /// <summary>
    /// Gets the unique identifier for this CIS band.
    /// </summary>
    /// <value>
    /// The band identifier (e.g., "cis_gross", "cis_standard", "cis_higher").
    /// </value>
    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; init; }

    /// <summary>
    /// Gets the tax deduction rate for this CIS band.
    /// </summary>
    /// <value>
    /// The deduction percentage as a decimal (e.g., 0.0 for 0%, 0.2 for 20%, 0.3 for 30%).
    /// </value>
    [JsonPropertyName("deduction_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? DeductionRate { get; init; }

    /// <summary>
    /// Gets the display description for CIS income under this band.
    /// </summary>
    /// <value>
    /// A human-readable description of the income category for this CIS band.
    /// </value>
    [JsonPropertyName("income_description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? IncomeDescription { get; init; }

    /// <summary>
    /// Gets the display description for CIS deductions under this band.
    /// </summary>
    /// <value>
    /// A human-readable description of the deduction category for this CIS band.
    /// </value>
    [JsonPropertyName("deduction_description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DeductionDescription { get; init; }

    /// <summary>
    /// Gets the nominal ledger code for this CIS band.
    /// </summary>
    /// <value>
    /// The accounting nominal code that corresponds to this CIS band (e.g., "061", "062", "063").
    /// </value>
    [JsonPropertyName("nominal_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? NominalCode { get; init; }
}
