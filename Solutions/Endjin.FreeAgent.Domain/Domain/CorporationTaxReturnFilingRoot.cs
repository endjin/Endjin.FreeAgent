// <copyright file="CorporationTaxReturnFilingRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON request wrapper for <see cref="Domain.CorporationTaxReturnFiling"/> data.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON serialization when recording Corporation Tax return filing details via the FreeAgent API.
/// </remarks>
/// <seealso cref="CorporationTaxReturnFiling"/>
/// <seealso cref="CorporationTaxReturn"/>
public record CorporationTaxReturnFilingRoot
{
    /// <summary>
    /// Gets the Corporation Tax return filing data for the API request.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.CorporationTaxReturnFiling"/> object containing filing details.
    /// </value>
    [JsonPropertyName("corporation_tax_return")]
    public CorporationTaxReturnFiling? CorporationTaxReturn { get; init; }
}