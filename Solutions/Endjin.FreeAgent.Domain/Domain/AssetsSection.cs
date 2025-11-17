// <copyright file="AssetsSection.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>
/// Represents the current assets section of a balance sheet.
/// </summary>
/// <remarks>
/// <para>
/// Current assets are short-term assets expected to be converted to cash or used up
/// within one year, such as cash, accounts receivable, and inventory.
/// </para>
/// <para>
/// API Endpoint: GET https://api.freeagent.com/v2/accounting/balance_sheet
/// </para>
/// <para>
/// API Documentation: https://dev.freeagent.com/docs/balance_sheet
/// </para>
/// </remarks>
public record AssetsSection
{
    /// <summary>
    /// Gets the individual current asset accounts that make up this section.
    /// </summary>
    [JsonPropertyName("accounts")]
    public List<BalanceSheetAccount>? Accounts { get; init; }
}
