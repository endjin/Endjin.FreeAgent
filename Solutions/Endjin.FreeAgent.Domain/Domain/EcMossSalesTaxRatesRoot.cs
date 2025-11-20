// <copyright file="EcMossSalesTaxRatesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="EcMossSalesTaxRate"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return
/// EC MOSS sales tax rates for a specific EU member state.
/// </remarks>
/// <seealso cref="EcMossSalesTaxRate"/>
public record EcMossSalesTaxRatesRoot
{
    /// <summary>
    /// Gets the collection of EC MOSS sales tax rates from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="EcMossSalesTaxRate"/> objects returned by the API for the specified
    /// country and date combination.
    /// </value>
    [JsonPropertyName("sales_tax_rates")]
    public List<EcMossSalesTaxRate> SalesTaxRates { get; init; } = [];
}
