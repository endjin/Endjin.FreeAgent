// <copyright file="PayrollProfilesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="Domain.PayrollProfile"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple payroll profile records.
/// </remarks>
/// <seealso cref="PayrollProfile"/>
public record PayrollProfilesRoot
{
    /// <summary>
    /// Gets the collection of payroll profiles from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="Domain.PayrollProfile"/> objects returned by the API.
    /// Defaults to an empty collection if no profiles are present.
    /// </value>
    [JsonPropertyName("payroll_profiles")]
    public List<PayrollProfile> PayrollProfiles { get; init; } = [];
}