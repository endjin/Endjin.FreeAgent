// <copyright file="CorporationTaxReturnRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a <see cref="Domain.CorporationTaxReturn"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single Corporation Tax return.
/// </remarks>
/// <seealso cref="CorporationTaxReturn"/>
public record CorporationTaxReturnRoot
{
    /// <summary>
    /// Gets the Corporation Tax return from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.CorporationTaxReturn"/> object returned by the API.
    /// </value>
    [JsonPropertyName("corporation_tax_return")]
    public CorporationTaxReturn? CorporationTaxReturn { get; init; }
}