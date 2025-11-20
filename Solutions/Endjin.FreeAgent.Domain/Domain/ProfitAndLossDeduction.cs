// <copyright file="ProfitAndLossDeduction.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a deduction entry in a profit and loss summary report.
/// </summary>
/// <remarks>
/// <para>
/// Deductions appear in the "less" section of the profit and loss summary, representing
/// items that are subtracted from operating profit to arrive at retained profit.
/// Common deductions include corporation tax, dividends, and director's drawings.
/// </para>
/// </remarks>
/// <seealso cref="ProfitAndLoss"/>
public record ProfitAndLossDeduction
{
    /// <summary>
    /// Gets the title or description of the deduction.
    /// </summary>
    /// <value>
    /// A descriptive name for the deduction, such as "Corporation Tax" or "Dividends".
    /// </value>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// Gets the total amount of the deduction.
    /// </summary>
    /// <value>
    /// The monetary value of this deduction in the company's native currency.
    /// </value>
    [JsonPropertyName("total")]
    public decimal? Total { get; init; }
}
