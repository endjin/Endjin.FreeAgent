// <copyright file="CategoryCreateRequest.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;
using Endjin.FreeAgent.Converters;

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a request to create a new category in the FreeAgent system.
/// </summary>
/// <remarks>
/// <para>
/// Categories must be created with specific nominal code ranges based on their group type:
/// <list type="bullet">
/// <item>Income: Nominal codes 001-049</item>
/// <item>Cost of Sales: Nominal codes 096-199</item>
/// <item>Admin Expenses: Nominal codes 200-399</item>
/// <item>Current Assets: Nominal codes 671-720</item>
/// <item>Liabilities: Nominal codes 731-780</item>
/// <item>Equities: Nominal codes 921-960</item>
/// </list>
/// </para>
/// <para>
/// Required fields: NominalCode, Description, and CategoryGroup. Additional fields may be required
/// based on the category group type (e.g., TaxReportingName for spending/assets/liabilities,
/// AllowableForTax for spending categories).
/// </para>
/// <para>
/// API Endpoint: POST /v2/categories
/// </para>
/// </remarks>
/// <seealso cref="Category"/>
/// <seealso cref="CategoryGroupType"/>
/// <seealso cref="AutoSalesTaxRateType"/>
public record CategoryCreateRequest
{
    /// <summary>
    /// Gets the nominal ledger code for this category.
    /// </summary>
    /// <value>
    /// The accounting nominal code that corresponds to this category in the chart of accounts.
    /// Must match the valid range for the selected category group.
    /// </value>
    /// <remarks>Required field.</remarks>
    [Required(ErrorMessage = "Nominal code is required")]
    [JsonPropertyName("nominal_code")]
    public string NominalCode { get; init; } = string.Empty;

    /// <summary>
    /// Gets the descriptive name of the category.
    /// </summary>
    /// <value>
    /// A human-readable description of what types of transactions this category represents.
    /// </value>
    /// <remarks>Required field.</remarks>
    [Required(ErrorMessage = "Description is required")]
    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Gets the group classification of this category.
    /// </summary>
    /// <value>
    /// The category group, which determines how transactions in this category are treated for accounting purposes.
    /// </value>
    /// <remarks>Required field.</remarks>
    [Required(ErrorMessage = "Category group is required")]
    [JsonPropertyName("category_group")]
    [JsonConverter(typeof(CategoryGroupTypeNonNullableJsonConverter))]
    public CategoryGroupType CategoryGroup { get; init; }

    /// <summary>
    /// Gets the tax reporting category name used on statutory accounts and tax returns.
    /// </summary>
    /// <value>
    /// The reporting category for this item on tax returns and statutory accounts.
    /// Required for cost_of_sales, admin_expenses, current_assets, and liabilities categories.
    /// Valid values vary by jurisdiction and category type.
    /// </value>
    [JsonPropertyName("tax_reporting_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TaxReportingName { get; init; }

    /// <summary>
    /// Gets the automatic sales tax rate to be applied to this category.
    /// </summary>
    /// <value>
    /// The default VAT/GST rate that should be applied to transactions in this category when automatic tax calculation is enabled.
    /// Note: "Exempt" is only valid for income categories.
    /// </value>
    /// <remarks>Optional field. Applicable to income and spending categories only.</remarks>
    [JsonPropertyName("auto_sales_tax_rate")]
    [JsonConverter(typeof(AutoSalesTaxRateTypeJsonConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AutoSalesTaxRateType? AutoSalesTaxRate { get; init; }

    /// <summary>
    /// Gets a value indicating whether expenses in this category are allowable for tax purposes.
    /// </summary>
    /// <value>
    /// <c>true</c> if expenses in this category are tax deductible; otherwise, <c>false</c>.
    /// </value>
    [JsonPropertyName("allowable_for_tax")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? AllowableForTax { get; init; }
}
