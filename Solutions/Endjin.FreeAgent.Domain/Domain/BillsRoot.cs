// <copyright file="BillsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="Bill"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple bill objects.
/// </remarks>
/// <seealso cref="Bill"/>
public record BillsRoot
{
    /// <summary>
    /// Gets the collection of bills from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="Bill"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("bills")]
    public List<Bill> Bills { get; init; } = [];
}