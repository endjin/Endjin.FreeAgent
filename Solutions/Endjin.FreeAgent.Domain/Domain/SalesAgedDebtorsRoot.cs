// <copyright file="SalesAgedDebtorsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a <see cref="Domain.SalesAgedDebtors"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a sales aged debtors report.
/// </remarks>
/// <seealso cref="SalesAgedDebtors"/>
public record SalesAgedDebtorsRoot
{
    /// <summary>
    /// Gets the sales aged debtors report from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.SalesAgedDebtors"/> object returned by the API.
    /// </value>
    [JsonPropertyName("sales_aged_debtors")]
    public SalesAgedDebtors? SalesAgedDebtors { get; init; }
}