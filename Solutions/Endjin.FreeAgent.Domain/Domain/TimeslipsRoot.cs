// <copyright file="TimeslipsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="Timeslip"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple timeslips.
/// </remarks>
/// <seealso cref="Timeslip"/>
public record TimeslipsRoot
{
    /// <summary>
    /// Gets the collection of timeslips from the API response.
    /// </summary>
    /// <value>
    /// An immutable list of <see cref="Timeslip"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("timeslips")]
    public ImmutableList<Timeslip> Timeslips { get; init; } = [];
}