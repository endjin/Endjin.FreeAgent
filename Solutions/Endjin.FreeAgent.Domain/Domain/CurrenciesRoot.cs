// <copyright file="CurrenciesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="Currency"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple currency objects.
/// </remarks>
/// <seealso cref="Currency"/>
public record CurrenciesRoot
{
    /// <summary>
    /// Gets the collection of currencies from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="Currency"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("currencies")]
    public List<Currency> Currencies { get; init; } = [];
}