// <copyright file="OpeningBalanceRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for an <see cref="Domain.OpeningBalance"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single opening balance record.
/// </remarks>
/// <seealso cref="OpeningBalance"/>
public record OpeningBalanceRoot
{
    /// <summary>
    /// Gets the opening balance from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.OpeningBalance"/> object returned by the API.
    /// </value>
    [JsonPropertyName("opening_balance")]
    public OpeningBalance? OpeningBalance { get; init; }
}