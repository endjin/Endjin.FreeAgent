// <copyright file="FinalAccountsReportRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain.Domain;

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a <see cref="FinalAccountsReport"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single Final Accounts report.
/// </remarks>
/// <seealso cref="FinalAccountsReport"/>
public record FinalAccountsReportRoot
{
    /// <summary>
    /// Gets the Final Accounts report from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="FinalAccountsReport"/> object returned by the API.
    /// </value>
    [JsonPropertyName("final_accounts_report")]
    public FinalAccountsReport? FinalAccountsReport { get; init; }
}
