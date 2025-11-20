// <copyright file="CategoryRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a single <see cref="Category"/> resource.
/// </summary>
/// <remarks>
/// <para>
/// The FreeAgent API returns single categories wrapped in a type-specific property name
/// that varies based on the category's classification (income, cost of sales, admin expenses, or general).
/// This wrapper type handles all four possible response formats by declaring properties for each variant.
/// </para>
/// <para>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses from GET /v2/categories/:nominal_code,
/// POST /v2/categories, PUT /v2/categories/:nominal_code, and DELETE /v2/categories/:nominal_code.
/// </para>
/// </remarks>
/// <seealso cref="Category"/>
public record CategoryRoot
{
    /// <summary>
    /// Gets the income category from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Category"/> object if the response contains an income category; otherwise, <c>null</c>.
    /// </value>
    [JsonPropertyName("income_categories")]
    public Category? IncomeCategories { get; init; }

    /// <summary>
    /// Gets the cost of sales category from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Category"/> object if the response contains a cost of sales category; otherwise, <c>null</c>.
    /// </value>
    [JsonPropertyName("cost_of_sales_categories")]
    public Category? CostOfSalesCategories { get; init; }

    /// <summary>
    /// Gets the admin expenses category from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Category"/> object if the response contains an admin expenses category; otherwise, <c>null</c>.
    /// </value>
    [JsonPropertyName("admin_expenses_categories")]
    public Category? AdminExpensesCategories { get; init; }

    /// <summary>
    /// Gets the general category from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Category"/> object if the response contains a general category (assets, liabilities, equity); otherwise, <c>null</c>.
    /// </value>
    [JsonPropertyName("general_categories")]
    public Category? GeneralCategories { get; init; }

    /// <summary>
    /// Gets the category from the API response, regardless of which type-specific wrapper it was returned in.
    /// </summary>
    /// <value>
    /// The <see cref="Category"/> object from whichever type-specific property is populated; <c>null</c> if none are populated.
    /// </value>
    [JsonIgnore]
    public Category? Category => IncomeCategories ?? CostOfSalesCategories ?? AdminExpensesCategories ?? GeneralCategories;
}
