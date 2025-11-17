// <copyright file="LiabilitiesSection.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>
/// Represents the current liabilities section of a balance sheet.
/// </summary>
/// <remarks>
/// <para>
/// Current liabilities are short-term financial obligations due within one year,
/// such as accounts payable, short-term loans, and accrued expenses.
/// </para>
/// <para>
/// Negative total debit values indicate amounts owed by the business.
/// </para>
/// <para>
/// API Endpoint: GET https://api.freeagent.com/v2/accounting/balance_sheet
/// </para>
/// <para>
/// API Documentation: https://dev.freeagent.com/docs/balance_sheet
/// </para>
/// </remarks>
public record LiabilitiesSection
{
    /// <summary>
    /// Gets the individual current liability accounts that make up this section.
    /// </summary>
    /// <remarks>
    /// Negative values indicate amounts owed by the business.
    /// </remarks>
    [JsonPropertyName("accounts")]
    public List<BalanceSheetAccount>? Accounts { get; init; }
}
