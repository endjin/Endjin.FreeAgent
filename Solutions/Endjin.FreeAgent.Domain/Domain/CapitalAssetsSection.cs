// <copyright file="CapitalAssetsSection.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>
/// Represents the capital assets section of a balance sheet.
/// </summary>
/// <remarks>
/// <para>
/// Capital assets (also known as fixed assets or non-current assets) are long-term assets
/// used in the operation of a business, such as property, equipment, and vehicles.
/// </para>
/// <para>
/// This section includes both the detailed account breakdown and the net book value
/// (original cost minus accumulated depreciation).
/// </para>
/// <para>
/// API Endpoint: GET https://api.freeagent.com/v2/accounting/balance_sheet
/// </para>
/// <para>
/// API Documentation: https://dev.freeagent.com/docs/balance_sheet
/// </para>
/// </remarks>
public record CapitalAssetsSection
{
    /// <summary>
    /// Gets the individual capital asset accounts that make up this section.
    /// </summary>
    [JsonPropertyName("accounts")]
    public List<BalanceSheetAccount>? Accounts { get; init; }

    /// <summary>
    /// Gets the net book value of all capital assets (original cost minus depreciation).
    /// </summary>
    /// <remarks>
    /// The value is rounded to the nearest integer.
    /// </remarks>
    [JsonPropertyName("net_book_value")]
    public int? NetBookValue { get; init; }
}
