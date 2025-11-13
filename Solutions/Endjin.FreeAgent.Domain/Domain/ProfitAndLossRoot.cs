// <copyright file="ProfitAndLossRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a <see cref="Domain.ProfitAndLoss"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single profit and loss report.
/// </remarks>
/// <seealso cref="ProfitAndLoss"/>
public record ProfitAndLossRoot
{
    /// <summary>
    /// Gets the profit and loss report from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.ProfitAndLoss"/> object returned by the API.
    /// </value>
    [JsonPropertyName("profit_and_loss")]
    public ProfitAndLoss? ProfitAndLoss { get; init; }
}