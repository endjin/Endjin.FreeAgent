// <copyright file="FinalAccountsReportsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="FinalAccountsReport"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple Final Accounts reports.
/// </remarks>
/// <seealso cref="FinalAccountsReport"/>
public record FinalAccountsReportsRoot
{
    /// <summary>
    /// Gets the collection of Final Accounts reports from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="FinalAccountsReport"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("final_accounts_reports")]
    public List<FinalAccountsReport>? FinalAccountsReports { get; init; }
}
