// <copyright file="CashFlowDirection.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents directional cash flow data (incoming or outgoing).
/// </summary>
/// <remarks>
/// <para>
/// This model represents aggregated cash flow in a specific direction (incoming revenue or
/// outgoing expenses) over a reporting period. It includes both the total amount across all
/// months and a monthly breakdown for granular analysis.
/// </para>
/// <para>
/// Incoming cash flow represents revenue streams such as customer payments, while outgoing
/// cash flow represents expenses such as supplier payments, taxes, and operational costs.
/// </para>
/// </remarks>
public record CashFlowDirection
{
    /// <summary>
    /// Gets the total cash flow amount across the entire reporting period.
    /// </summary>
    /// <value>
    /// A decimal value representing the sum of all cash flow in this direction
    /// (incoming or outgoing) for the entire period, or null if not provided.
    /// </value>
    [JsonPropertyName("total")]
    public decimal? Total { get; init; }

    /// <summary>
    /// Gets the monthly breakdown of cash flow amounts.
    /// </summary>
    /// <value>
    /// A list of <see cref="CashFlowMonthly"/> objects containing the cash flow
    /// amount for each month within the reporting period, or null if not provided.
    /// </value>
    [JsonPropertyName("months")]
    public List<CashFlowMonthly>? Months { get; init; }
}
