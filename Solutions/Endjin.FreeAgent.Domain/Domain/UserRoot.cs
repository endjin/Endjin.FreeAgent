// <copyright file="UserRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

using System.Text.Json.Serialization;

/// <summary>
/// Represents the root-level JSON response wrapper for a <see cref="Domain.User"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single user.
/// </remarks>
/// <seealso cref="User"/>
public partial record UserRoot
{
    /// <summary>
    /// Gets the user from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.User"/> object returned by the API.
    /// </value>
    [JsonPropertyName("user")]
    public User? User { get; init; }
}