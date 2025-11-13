// <copyright file="SalesTaxRatesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="SalesTaxRate"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple sales tax rates.
/// </remarks>
/// <seealso cref="SalesTaxRate"/>
public record SalesTaxRatesRoot
{
    /// <summary>
    /// Gets the collection of sales tax rates from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="SalesTaxRate"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("sales_tax_rates")]
    public List<SalesTaxRate> SalesTaxRates { get; init; } = [];
}