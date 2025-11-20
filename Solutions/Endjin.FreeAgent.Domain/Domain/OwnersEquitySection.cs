// <copyright file="OwnersEquitySection.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>
/// Represents the owners' equity section of a balance sheet.
/// </summary>
/// <remarks>
/// <para>
/// Owners' equity represents the residual interest in the assets of the business
/// after deducting liabilities. It includes capital contributions and retained profits.
/// </para>
/// <para>
/// According to the accounting equation: Assets = Liabilities + Equity
/// </para>
/// <para>
/// The total owners' equity should always be the inverse of total assets in a balanced sheet.
/// </para>
/// <para>
/// API Endpoint: GET https://api.freeagent.com/v2/accounting/balance_sheet
/// </para>
/// <para>
/// API Documentation: https://dev.freeagent.com/docs/balance_sheet
/// </para>
/// </remarks>
public record OwnersEquitySection
{
    /// <summary>
    /// Gets the individual owners' equity accounts that make up this section.
    /// </summary>
    [JsonPropertyName("accounts")]
    public List<BalanceSheetAccount>? Accounts { get; init; }

    /// <summary>
    /// Gets the retained profit (accumulated earnings not distributed to owners).
    /// </summary>
    /// <remarks>
    /// The value is rounded to the nearest integer.
    /// </remarks>
    [JsonPropertyName("retained_profit")]
    public int? RetainedProfit { get; init; }
}
