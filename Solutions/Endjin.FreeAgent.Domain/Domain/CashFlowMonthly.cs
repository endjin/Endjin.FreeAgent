// <copyright file="CashFlowMonthly.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a monthly cash flow breakdown.
/// </summary>
/// <remarks>
/// This model represents the cash flow amount for a specific month and year within a
/// cash flow report. It provides granular visibility into the distribution of incoming
/// or outgoing cash across individual months within the reporting period.
/// </remarks>
public record CashFlowMonthly
{
    /// <summary>
    /// Gets the month number (1-12).
    /// </summary>
    /// <value>
    /// An integer representing the month (1 = January, 12 = December), or null if not provided.
    /// </value>
    [JsonPropertyName("month")]
    public int? Month { get; init; }

    /// <summary>
    /// Gets the year.
    /// </summary>
    /// <value>
    /// An integer representing the four-digit year (e.g., 2024), or null if not provided.
    /// </value>
    [JsonPropertyName("year")]
    public int? Year { get; init; }

    /// <summary>
    /// Gets the total cash flow amount for this month.
    /// </summary>
    /// <value>
    /// A decimal value representing the monetary amount for this specific month,
    /// or null if not provided.
    /// </value>
    [JsonPropertyName("total")]
    public decimal? Total { get; init; }
}
