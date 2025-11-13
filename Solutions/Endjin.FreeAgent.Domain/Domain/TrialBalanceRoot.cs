// <copyright file="TrialBalanceRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a <see cref="Domain.TrialBalance"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single trial balance report.
/// </remarks>
/// <seealso cref="TrialBalance"/>
public record TrialBalanceRoot
{
    /// <summary>
    /// Gets the trial balance report from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.TrialBalance"/> object returned by the API.
    /// </value>
    [JsonPropertyName("trial_balance")]
    public TrialBalance? TrialBalance { get; init; }
}