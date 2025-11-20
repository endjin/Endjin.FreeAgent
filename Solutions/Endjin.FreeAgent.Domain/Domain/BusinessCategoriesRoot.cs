// <copyright file="BusinessCategoriesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root response wrapper for the business categories API endpoint.
/// </summary>
/// <remarks>
/// This wrapper contains the list of valid business category values that can be assigned to a company.
/// </remarks>
public record BusinessCategoriesRoot
{
    /// <summary>
    /// Gets the collection of valid business categories.
    /// </summary>
    /// <value>
    /// A list of standardized business category names such as "Accounting &amp; Bookkeeping", "Administration", "Agriculture", etc.
    /// </value>
    [JsonPropertyName("business_categories")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? BusinessCategories { get; init; }
}
