// <copyright file="BillRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a single <see cref="Domain.Bill"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single bill object.
/// </remarks>
/// <seealso cref="Bill"/>
public record BillRoot
{
    /// <summary>
    /// Gets the bill from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.Bill"/> object returned by the API.
    /// </value>
    [JsonPropertyName("bill")]
    public Bill? Bill { get; init; }
}