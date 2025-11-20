// <copyright file="PayrollProfilesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="PayrollProfile"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return payroll profiles.
/// The API returns profiles wrapped in a "profiles" property.
/// </remarks>
/// <seealso cref="PayrollProfile"/>
public record PayrollProfilesRoot
{
    /// <summary>
    /// Gets the collection of payroll profiles from the API response.
    /// </summary>
    /// <value>
    /// An immutable list of <see cref="PayrollProfile"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("profiles")]
    public ImmutableList<PayrollProfile> Profiles { get; init; } = [];
}
