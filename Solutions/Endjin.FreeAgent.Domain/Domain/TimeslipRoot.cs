// <copyright file="TimeslipRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a single <see cref="Timeslip"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single timeslip,
/// such as GET /v2/timeslips/:id, POST /v2/timeslips, and PUT /v2/timeslips/:id.
/// </remarks>
/// <seealso cref="Timeslip"/>
public record TimeslipRoot
{
    /// <summary>
    /// Gets the timeslip from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.Timeslip"/> object returned by the API.
    /// </value>
    [JsonPropertyName("timeslip")]
    public Timeslip? Timeslip { get; init; }
}
