// <copyright file="CategoriesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="Category"/> resources.
/// </summary>
/// <remarks>
/// <para>
/// The FreeAgent API returns categories grouped into four separate arrays based on their type:
/// administrative expenses, cost of sales, income, and general categories. This structure
/// reflects how the API organizes categories for accounting purposes.
/// </para>
/// <para>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses from GET /v2/categories.
/// </para>
/// </remarks>
/// <seealso cref="Category"/>
public record CategoriesRoot
{
    /// <summary>
    /// Gets the collection of administrative expense categories from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="Category"/> objects for administrative expenses (nominal codes 200-399).
    /// </value>
    [JsonPropertyName("admin_expenses_categories")]
    public List<Category> AdminExpensesCategories { get; init; } = [];

    /// <summary>
    /// Gets the collection of cost of sales categories from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="Category"/> objects for cost of sales (nominal codes 096-199).
    /// </value>
    [JsonPropertyName("cost_of_sales_categories")]
    public List<Category> CostOfSalesCategories { get; init; } = [];

    /// <summary>
    /// Gets the collection of income categories from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="Category"/> objects for income (nominal codes 001-049).
    /// </value>
    [JsonPropertyName("income_categories")]
    public List<Category> IncomeCategories { get; init; } = [];

    /// <summary>
    /// Gets the collection of general categories from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="Category"/> objects for general categories (assets, liabilities, equity).
    /// </value>
    [JsonPropertyName("general_categories")]
    public List<Category> GeneralCategories { get; init; } = [];
}