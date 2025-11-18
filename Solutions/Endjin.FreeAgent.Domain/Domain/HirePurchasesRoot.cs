// <copyright file="HirePurchasesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="HirePurchase"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple hire purchase objects.
/// </remarks>
/// <seealso cref="HirePurchase"/>
public record HirePurchasesRoot
{
    /// <summary>
    /// Gets the collection of hire purchases from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="HirePurchase"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("hire_purchases")]
    public List<HirePurchase>? HirePurchases { get; init; }
}
