// <copyright file="CategoryUpdateRequest.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;
using Endjin.FreeAgent.Converters;

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a request to update an existing category in the FreeAgent system.
/// </summary>
/// <remarks>
/// <para>
/// Only empty categories (categories with no associated transactions) can be modified.
/// </para>
/// <para>
/// API Endpoint: PUT /v2/categories/:nominal_code
/// </para>
/// </remarks>
/// <seealso cref="Category"/>
public record CategoryUpdateRequest
{
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
    /// Gets the tax reporting category name used on statutory accounts and tax returns.
    /// </summary>
    /// <value>
    /// The reporting category for this item on tax returns and statutory accounts.
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
