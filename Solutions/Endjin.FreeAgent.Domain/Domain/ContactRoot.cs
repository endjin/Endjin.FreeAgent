// <copyright file="ContactRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a single <see cref="Domain.Contact"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single contact object.
/// </remarks>
/// <seealso cref="Contact"/>
public record ContactRoot
{
    /// <summary>
    /// Gets the contact resource from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.Contact"/> object returned by the API.
    /// </value>
    [JsonPropertyName("contact")]
    public Contact? Contact { get; init; }
}