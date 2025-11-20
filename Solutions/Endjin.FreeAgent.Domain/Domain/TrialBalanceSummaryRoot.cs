// <copyright file="TrialBalanceSummaryRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a trial balance summary from the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return
/// trial balance summary data. The response contains an array of <see cref="TrialBalanceSummaryEntry"/>
/// objects representing each category in the trial balance.
/// </para>
/// <para>
/// API Endpoints:
/// <list type="bullet">
/// <item><description>GET /v2/accounting/trial_balance/summary - Returns the trial balance summary</description></item>
/// <item><description>GET /v2/accounting/trial_balance/summary/opening_balances - Returns opening balances</description></item>
/// </list>
/// </para>
/// </remarks>
/// <seealso cref="TrialBalanceSummaryEntry"/>
public record TrialBalanceSummaryRoot
{
    /// <summary>
    /// Gets the trial balance summary entries from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="TrialBalanceSummaryEntry"/> objects representing each category
    /// in the trial balance summary, or <c>null</c> if no entries are present.
    /// </value>
    [JsonPropertyName("trial_balance_summary")]
    public List<TrialBalanceSummaryEntry>? TrialBalanceSummary { get; init; }
}
