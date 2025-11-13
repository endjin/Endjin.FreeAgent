// <copyright file="UsersRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="User"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple users.
/// </remarks>
/// <seealso cref="User"/>
public record UsersRoot
{
    /// <summary>
    /// Gets the collection of users from the API response.
    /// </summary>
    /// <value>
    /// An immutable list of <see cref="User"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("users")]
    public ImmutableList<User> Users { get; init; } = [];
}