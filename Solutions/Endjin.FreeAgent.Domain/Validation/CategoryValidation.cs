// <copyright file="CategoryValidation.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Validation;

/// <summary>
/// Provides validation methods for category-related operations in the FreeAgent system.
/// </summary>
/// <remarks>
/// This class validates business rules specific to FreeAgent categories, including
/// nominal code ranges per category group and required fields based on category type.
/// </remarks>
public static class CategoryValidation
{
    /// <summary>
    /// Validates that a nominal code is within the valid range for the specified category group.
    /// </summary>
    /// <param name="nominalCode">The nominal code to validate.</param>
    /// <param name="categoryGroup">The category group type.</param>
    /// <returns><c>true</c> if the nominal code is valid for the category group; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="nominalCode"/> is null.</exception>
    public static bool IsValidNominalCodeForGroup(string nominalCode, CategoryGroupType categoryGroup)
    {
        ArgumentNullException.ThrowIfNull(nominalCode);

        if (!int.TryParse(nominalCode, out int code))
        {
            return false;
        }

        return categoryGroup switch
        {
            CategoryGroupType.Income => code >= 1 && code <= 49,
            CategoryGroupType.CostOfSales => code >= 96 && code <= 199,
            CategoryGroupType.AdminExpenses => code >= 200 && code <= 399,
            CategoryGroupType.CurrentAssets => code >= 671 && code <= 720,
            CategoryGroupType.Liabilities => code >= 731 && code <= 780,
            CategoryGroupType.Equities => code >= 921 && code <= 960,
            _ => false
        };
    }

    /// <summary>
    /// Gets the valid nominal code range for the specified category group.
    /// </summary>
    /// <param name="categoryGroup">The category group type.</param>
    /// <returns>A tuple containing the minimum and maximum nominal codes for the group.</returns>
    public static (int Min, int Max) GetNominalCodeRange(CategoryGroupType categoryGroup)
    {
        return categoryGroup switch
        {
            CategoryGroupType.Income => (1, 49),
            CategoryGroupType.CostOfSales => (96, 199),
            CategoryGroupType.AdminExpenses => (200, 399),
            CategoryGroupType.CurrentAssets => (671, 720),
            CategoryGroupType.Liabilities => (731, 780),
            CategoryGroupType.Equities => (921, 960),
            _ => throw new ArgumentOutOfRangeException(nameof(categoryGroup), $"Invalid category group: {categoryGroup}")
        };
    }

    /// <summary>
    /// Determines whether tax reporting name is required for the specified category group.
    /// </summary>
    /// <param name="categoryGroup">The category group type.</param>
    /// <returns><c>true</c> if tax reporting name is required; otherwise, <c>false</c>.</returns>
    public static bool IsTaxReportingNameRequired(CategoryGroupType categoryGroup)
    {
        return categoryGroup switch
        {
            CategoryGroupType.CostOfSales => true,
            CategoryGroupType.AdminExpenses => true,
            CategoryGroupType.CurrentAssets => true,
            CategoryGroupType.Liabilities => true,
            _ => false
        };
    }

    /// <summary>
    /// Determines whether allowable for tax is required for the specified category group.
    /// </summary>
    /// <param name="categoryGroup">The category group type.</param>
    /// <returns><c>true</c> if allowable for tax is required; otherwise, <c>false</c>.</returns>
    public static bool IsAllowableForTaxRequired(CategoryGroupType categoryGroup)
    {
        // Only required for spending categories (Cost of Sales and Admin Expenses)
        return categoryGroup == CategoryGroupType.CostOfSales ||
               categoryGroup == CategoryGroupType.AdminExpenses;
    }

    /// <summary>
    /// Determines whether the auto sales tax rate can be applied to the specified category group.
    /// </summary>
    /// <param name="categoryGroup">The category group type.</param>
    /// <returns><c>true</c> if auto sales tax rate can be applied; otherwise, <c>false</c>.</returns>
    public static bool CanApplyAutoSalesTaxRate(CategoryGroupType categoryGroup)
    {
        // Only applicable to income and spending categories
        return categoryGroup == CategoryGroupType.Income ||
               categoryGroup == CategoryGroupType.CostOfSales ||
               categoryGroup == CategoryGroupType.AdminExpenses;
    }

    /// <summary>
    /// Determines whether the specified auto sales tax rate is valid for the category group.
    /// </summary>
    /// <param name="taxRate">The auto sales tax rate type.</param>
    /// <param name="categoryGroup">The category group type.</param>
    /// <returns><c>true</c> if the tax rate is valid for the category group; otherwise, <c>false</c>.</returns>
    public static bool IsValidAutoSalesTaxRateForGroup(AutoSalesTaxRateType? taxRate, CategoryGroupType categoryGroup)
    {
        if (taxRate == null)
        {
            return true; // Null is always valid (optional field)
        }

        // Exempt is only valid for income categories
        if (taxRate == AutoSalesTaxRateType.Exempt && categoryGroup != CategoryGroupType.Income)
        {
            return false;
        }

        // Auto sales tax rate is only applicable to income and spending categories
        return CanApplyAutoSalesTaxRate(categoryGroup);
    }

    /// <summary>
    /// Validates a category create request for completeness and correctness.
    /// </summary>
    /// <param name="request">The category create request to validate.</param>
    /// <returns>A collection of validation error messages, or an empty collection if valid.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
    public static IEnumerable<string> ValidateCategoryCreateRequest(CategoryCreateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        List<string> errors = new List<string>();

        // Validate nominal code range
        if (!IsValidNominalCodeForGroup(request.NominalCode, request.CategoryGroup))
        {
            (int min, int max) = GetNominalCodeRange(request.CategoryGroup);
            errors.Add($"Nominal code {request.NominalCode} is not valid for category group {request.CategoryGroup}. Valid range is {min:D3}-{max:D3}.");
        }

        // Validate required tax reporting name
        if (IsTaxReportingNameRequired(request.CategoryGroup) && string.IsNullOrWhiteSpace(request.TaxReportingName))
        {
            errors.Add($"Tax reporting name is required for category group {request.CategoryGroup}.");
        }

        // Validate required allowable for tax
        if (IsAllowableForTaxRequired(request.CategoryGroup) && request.AllowableForTax == null)
        {
            errors.Add($"Allowable for tax is required for category group {request.CategoryGroup}.");
        }

        // Validate auto sales tax rate
        if (!IsValidAutoSalesTaxRateForGroup(request.AutoSalesTaxRate, request.CategoryGroup))
        {
            if (request.AutoSalesTaxRate == AutoSalesTaxRateType.Exempt)
            {
                errors.Add("Auto sales tax rate 'Exempt' is only valid for income categories.");
            }
            else
            {
                errors.Add($"Auto sales tax rate cannot be applied to category group {request.CategoryGroup}.");
            }
        }

        return errors;
    }

    /// <summary>
    /// Gets a description of the valid nominal code range for a category group.
    /// </summary>
    /// <param name="categoryGroup">The category group type.</param>
    /// <returns>A string describing the valid nominal code range.</returns>
    public static string GetNominalCodeRangeDescription(CategoryGroupType categoryGroup)
    {
        (int min, int max) = GetNominalCodeRange(categoryGroup);
        return $"{min:D3}-{max:D3}";
    }
}