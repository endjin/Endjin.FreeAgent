// <copyright file="SalesTaxPeriodsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="SalesTaxPeriod"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple sales tax periods.
/// </remarks>
/// <seealso cref="SalesTaxPeriod"/>
public record SalesTaxPeriodsRoot
{
    /// <summary>
    /// Gets the collection of sales tax periods from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="SalesTaxPeriod"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("sales_tax_periods")]
    public List<SalesTaxPeriod>? SalesTaxPeriods { get; init; }
}