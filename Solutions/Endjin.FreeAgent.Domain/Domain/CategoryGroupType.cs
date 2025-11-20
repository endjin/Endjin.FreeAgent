// <copyright file="CategoryGroupType.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Defines the category group types available in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Category groups organize different types of financial categories with specific nominal code ranges:
/// <list type="bullet">
/// <item><see cref="Income"/> - Nominal codes 001-049</item>
/// <item><see cref="CostOfSales"/> - Nominal codes 096-199</item>
/// <item><see cref="AdminExpenses"/> - Nominal codes 200-399</item>
/// <item><see cref="CurrentAssets"/> - Nominal codes 671-720</item>
/// <item><see cref="Liabilities"/> - Nominal codes 731-780</item>
/// <item><see cref="Equities"/> - Nominal codes 921-960</item>
/// </list>
/// </para>
/// <para>
/// Each category group has specific requirements for additional properties such as
/// tax_reporting_name and allowable_for_tax. See the FreeAgent API documentation
/// for detailed requirements per category group.
/// </para>
/// </remarks>
/// <seealso cref="Category"/>
/// <seealso cref="CategoryCreateRequest"/>
public enum CategoryGroupType
{
    /// <summary>
    /// Income categories for revenue streams (nominal codes 001-049).
    /// </summary>
    Income,

    /// <summary>
    /// Cost of sales categories for direct costs (nominal codes 096-199).
    /// </summary>
    CostOfSales,

    /// <summary>
    /// Administrative expense categories (nominal codes 200-399).
    /// </summary>
    AdminExpenses,

    /// <summary>
    /// Current asset categories (nominal codes 671-720).
    /// </summary>
    CurrentAssets,

    /// <summary>
    /// Liability categories (nominal codes 731-780).
    /// </summary>
    Liabilities,

    /// <summary>
    /// Equity categories (nominal codes 921-960).
    /// </summary>
    Equities,
}