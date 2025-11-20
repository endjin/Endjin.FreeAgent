// <copyright file="ProfitAndLossRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a <see cref="Domain.ProfitAndLoss"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a profit and loss summary.
/// The API returns the summary nested under a "profit_and_loss_summary" property.
/// </remarks>
/// <seealso cref="ProfitAndLoss"/>
/// <seealso cref="ProfitAndLossDeduction"/>
public record ProfitAndLossRoot
{
    /// <summary>
    /// Gets the profit and loss report from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.ProfitAndLoss"/> object returned by the API.
    /// </value>
    [JsonPropertyName("profit_and_loss_summary")]
    public ProfitAndLoss? ProfitAndLoss { get; init; }
}