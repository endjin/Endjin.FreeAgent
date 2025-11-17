// <copyright file="CashFlowRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Text.Json.Serialization;

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a <see cref="Domain.CashFlow"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single cash flow report.
/// </remarks>
/// <seealso cref="CashFlow"/>
public record CashFlowRoot
{
    /// <summary>
    /// Gets the cash flow report from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.CashFlow"/> object returned by the API.
    /// </value>
    [JsonPropertyName("cashflow")]
    public CashFlow? CashFlow { get; init; }
}
