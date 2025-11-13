// <copyright file="BalanceSheetEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a single line item entry within a balance sheet report in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Balance sheet entries provide the detailed breakdown of each section of the balance sheet,
/// showing individual accounting categories with their nominal codes, descriptions, and values.
/// </para>
/// <para>
/// Each entry corresponds to a specific accounting category (line in the chart of accounts) and shows
/// the current balance for that category at the balance sheet date. Entries are grouped into sections
/// such as fixed assets, current assets, current liabilities, and capital and reserves.
/// </para>
/// </remarks>
/// <seealso cref="BalanceSheet"/>
/// <seealso cref="Category"/>
public record BalanceSheetEntry
{
    /// <summary>
    /// Gets the URI reference to the accounting category for this entry.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Category"/> in the chart of accounts that this balance sheet line represents.
    /// </value>
    [JsonPropertyName("category_url")]
    public Uri? CategoryUrl { get; init; }

    /// <summary>
    /// Gets the human-readable description of the accounting category.
    /// </summary>
    /// <value>
    /// The descriptive name of the category, such as "Office Equipment", "Trade Debtors", or "Share Capital".
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
    /// Gets the monetary value for this balance sheet entry.
    /// </summary>
    /// <value>
    /// The balance amount for this category at the balance sheet date.
    /// The value represents the net position of the account.
    /// </value>
    [JsonPropertyName("value")]
    public decimal? Value { get; init; }
}