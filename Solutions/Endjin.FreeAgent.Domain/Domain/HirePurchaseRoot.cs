// <copyright file="HirePurchaseRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a single <see cref="Domain.HirePurchase"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single hire purchase object.
/// </remarks>
/// <seealso cref="HirePurchase"/>
public record HirePurchaseRoot
{
    /// <summary>
    /// Gets the hire purchase from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.HirePurchase"/> object returned by the API.
    /// </value>
    [JsonPropertyName("hire_purchase")]
    public HirePurchase? HirePurchase { get; init; }
}
