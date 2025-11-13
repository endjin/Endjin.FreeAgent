// <copyright file="Category.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents an accounting category in the FreeAgent system.
/// </summary>
/// <remarks>
/// <para>
/// Categories classify financial transactions for accounting and tax reporting purposes.
/// Each category corresponds to a nominal code in the chart of accounts and has specific
/// tax treatment rules for sales tax calculations.
/// </para>
/// <para>
/// Categories are predefined by FreeAgent based on the company type and cannot be created
/// or deleted via the API, though their properties can be retrieved for reference when
/// creating transactions.
/// </para>
/// <para>
/// API Endpoint: /v2/categories
/// </para>
/// </remarks>
/// <seealso cref="Expense"/>
/// <seealso cref="Invoice"/>
public record Category
{
    /// <summary>
    /// Gets the unique URI identifier for this category.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this category in the FreeAgent system.
    /// This URI is used when assigning categories to transactions.
    /// </value>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the nominal ledger code for this category.
    /// </summary>
    /// <value>
    /// The accounting nominal code that corresponds to this category in the chart of accounts.
    /// </value>
    [JsonPropertyName("nominal_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? NominalCode { get; init; }

    /// <summary>
    /// Gets the descriptive name of the category.
    /// </summary>
    /// <value>
    /// A human-readable description of what types of transactions this category represents.
    /// </value>
    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the type classification of this category.
    /// </summary>
    /// <value>
    /// The category type, which determines how transactions in this category are treated for accounting purposes.
    /// </value>
    [JsonPropertyName("category_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CategoryType { get; init; }

    /// <summary>
    /// Gets the tax reporting name for this category.
    /// </summary>
    /// <value>
    /// The name used when this category appears on tax returns and tax reports.
    /// </value>
    [JsonPropertyName("tax_reporting_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TaxReportingName { get; init; }

    /// <summary>
    /// Gets the automatic sales tax rate applied to this category.
    /// </summary>
    /// <value>
    /// The default VAT/GST rate URI that should be applied to transactions in this category when automatic tax calculation is enabled.
    /// </value>
    [JsonPropertyName("auto_sales_tax_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? AutoSalesTaxRate { get; init; }
}