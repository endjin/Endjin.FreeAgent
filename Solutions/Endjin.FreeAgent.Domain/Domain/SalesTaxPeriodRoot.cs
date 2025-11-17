// <copyright file="SalesTaxPeriodRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a <see cref="SalesTaxPeriod"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single sales tax period.
/// </remarks>
/// <seealso cref="SalesTaxPeriod"/>
public record SalesTaxPeriodRoot
{
    /// <summary>
    /// Gets the sales tax period from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="SalesTaxPeriod"/> object returned by the API.
    /// </value>
    [JsonPropertyName("sales_tax_period")]
    public SalesTaxPeriod? SalesTaxPeriod { get; init; }
}