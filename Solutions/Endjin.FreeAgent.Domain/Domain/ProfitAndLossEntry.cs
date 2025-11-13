// <copyright file="ProfitAndLossEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a single line item entry within a profit and loss report in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Profit and loss entries provide the detailed breakdown of income, cost of sales, and expenses,
/// showing individual accounting categories with their nominal codes, descriptions, values, and
/// percentage of turnover for analytical purposes.
/// </para>
/// <para>
/// Each entry corresponds to a specific accounting category and includes a percentage calculation
/// showing the entry's value as a proportion of total turnover, which is useful for trend analysis
/// and identifying significant cost or income drivers.
/// </para>
/// </remarks>
/// <seealso cref="ProfitAndLoss"/>
/// <seealso cref="Category"/>
public record ProfitAndLossEntry
{
    /// <summary>
    /// Gets the URI reference to the accounting category for this entry.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Category"/> in the chart of accounts that this profit and loss line represents.
    /// </value>
    [JsonPropertyName("category_url")]
    public Uri? CategoryUrl { get; init; }

    /// <summary>
    /// Gets the human-readable description of the accounting category.
    /// </summary>
    /// <value>
    /// The descriptive name of the category, such as "Sales", "Consultancy Income", "Office Expenses", or "Salaries".
    /// </value>
    [JsonPropertyName("category_description")]
    public string? CategoryDescription { get; init; }

    /// <summary>
    /// Gets the nominal code (account code) for this category.
    /// </summary>
    /// <value>
    /// The numeric or alphanumeric code that identifies this category in the chart of accounts,
    /// following standard accounting numbering conventions.
    /// </value>
    [JsonPropertyName("nominal_code")]
    public string? NominalCode { get; init; }

    /// <summary>
    /// Gets the monetary value for this profit and loss entry.
    /// </summary>
    /// <value>
    /// The total amount for this category during the reporting period.
    /// For income entries this represents revenue, for expense entries this represents costs.
    /// </value>
    [JsonPropertyName("value")]
    public decimal? Value { get; init; }

    /// <summary>
    /// Gets the percentage of turnover that this entry represents.
    /// </summary>
    /// <value>
    /// The entry's value expressed as a percentage of total turnover, useful for analyzing
    /// the relative significance of different income sources and expense categories.
    /// For example, if this entry's value is £10,000 and turnover is £100,000, this would be 10.0.
    /// </value>
    [JsonPropertyName("percentage_of_turnover")]
    public decimal? PercentageOfTurnover { get; init; }
}