// <copyright file="ContactsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="Contact"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple contact objects.
/// </remarks>
/// <seealso cref="Contact"/>
public record ContactsRoot
{
    /// <summary>
    /// Gets the collection of contacts from the API response.
    /// </summary>
    /// <value>
    /// An immutable list of <see cref="Contact"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("contacts")]
    public ImmutableList<Contact> Contacts { get; init; } = [];
}