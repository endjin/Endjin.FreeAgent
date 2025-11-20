// <copyright file="CisBandsResponse.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the response containing available Construction Industry Scheme (CIS) deduction bands.
/// </summary>
/// <remarks>
/// <para>
/// This response is returned by the CIS Bands API endpoint and provides reference data about
/// the available CIS deduction bands that can be applied to subcontractor invoices and bills.
/// </para>
/// <para>
/// The response contains a collection of CIS bands, each defining:
/// <list type="bullet">
/// <item>Band identifier (name)</item>
/// <item>Deduction rate percentage</item>
/// <item>Income and deduction descriptions</item>
/// <item>Associated nominal ledger code</item>
/// </list>
/// </para>
/// <para>
/// CIS bands are read-only reference data that changes infrequently. FreeAgent recommends
/// caching this data client-side to minimize API calls. The standard bands include:
/// <list type="bullet">
/// <item><strong>cis_gross</strong> - No deduction (0%)</item>
/// <item><strong>cis_standard</strong> - Standard rate (20%)</item>
/// <item><strong>cis_higher</strong> - Higher rate (30%)</item>
/// </list>
/// </para>
/// <para>
/// This endpoint is only available to UK companies enrolled in the Construction Industry Scheme
/// for Subcontractors and requires "Estimates &amp; Invoices" access level.
/// </para>
/// <para>
/// API Endpoint: /v2/cis_bands
/// </para>
/// </remarks>
/// <seealso cref="CisBand"/>
/// <seealso cref="Bill"/>
/// <seealso cref="Invoice"/>
public record CisBandsResponse
{
    /// <summary>
    /// Gets the collection of available CIS deduction bands.
    /// </summary>
    /// <value>
    /// A list of <see cref="CisBand"/> objects representing all available Construction Industry Scheme
    /// deduction bands. Each band defines a specific deduction rate and associated accounting codes.
    /// </value>
    [JsonPropertyName("available_bands")]
    public List<CisBand>? AvailableBands { get; init; }
}
