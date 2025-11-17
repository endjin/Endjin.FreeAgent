// <copyright file="BalanceSheetAccount.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

using System.Text.Json.Serialization;

/// <summary>
/// Represents an individual account entry within a balance sheet section.
/// </summary>
/// <remarks>
/// <para>
/// Each account contains the account name, nominal code, and total debit value.
/// All monetary values are returned as integers rounded to the nearest whole number.
/// </para>
/// <para>
/// Negative total debit values in liabilities sections indicate amounts owed by the business.
/// </para>
/// <para>
/// API Endpoint: GET https://api.freeagent.com/v2/accounting/balance_sheet
/// </para>
/// <para>
/// API Documentation: https://dev.freeagent.com/docs/balance_sheet
/// </para>
/// </remarks>
public record BalanceSheetAccount
{
    /// <summary>
    /// Gets the name of the account.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Gets the nominal code (account code) for this account.
    /// </summary>
    [JsonPropertyName("nominal_code")]
    public string? NominalCode { get; init; }

    /// <summary>
    /// Gets the total debit value for this account, rounded to the nearest integer.
    /// </summary>
    /// <remarks>
    /// Negative values in liability accounts indicate amounts owed by the business.
    /// </remarks>
    [JsonPropertyName("total_debit_value")]
    public int? TotalDebitValue { get; init; }
}
