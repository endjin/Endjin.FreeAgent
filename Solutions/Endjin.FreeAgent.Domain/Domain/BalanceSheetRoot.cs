// <copyright file="BalanceSheetRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a <see cref="Domain.BalanceSheet"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single balance sheet report.
/// </remarks>
/// <seealso cref="BalanceSheet"/>
public record BalanceSheetRoot
{
    /// <summary>
    /// Gets the balance sheet from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.BalanceSheet"/> object returned by the API.
    /// </value>
    [JsonPropertyName("balance_sheet")]
    public BalanceSheet? BalanceSheet { get; init; }
}