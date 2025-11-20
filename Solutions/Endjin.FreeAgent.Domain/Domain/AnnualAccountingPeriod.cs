// <copyright file="AnnualAccountingPeriod.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents an annual accounting period for a company.
/// </summary>
/// <remarks>
/// <para>
/// Annual accounting periods define the start and end dates for each fiscal year,
/// forming the basis for financial reporting and tax calculations.
/// </para>
/// </remarks>
public record AnnualAccountingPeriod
{
    /// <summary>
    /// Gets the start date of the accounting period.
    /// </summary>
    /// <value>
    /// The date when this accounting period begins.
    /// </value>
    [JsonPropertyName("starts_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? StartsOn { get; init; }

    /// <summary>
    /// Gets the end date of the accounting period.
    /// </summary>
    /// <value>
    /// The date when this accounting period ends.
    /// </value>
    [JsonPropertyName("ends_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? EndsOn { get; init; }
}
