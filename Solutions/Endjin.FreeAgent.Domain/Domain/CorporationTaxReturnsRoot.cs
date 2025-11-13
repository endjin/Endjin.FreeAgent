// <copyright file="CorporationTaxReturnsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="Domain.CorporationTaxReturn"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple Corporation Tax returns.
/// </remarks>
/// <seealso cref="CorporationTaxReturn"/>
public record CorporationTaxReturnsRoot
{
    /// <summary>
    /// Gets the collection of Corporation Tax returns from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="Domain.CorporationTaxReturn"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("corporation_tax_returns")]
    public List<CorporationTaxReturn>? CorporationTaxReturns { get; init; }
}