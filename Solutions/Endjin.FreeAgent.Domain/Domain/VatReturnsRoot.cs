// <copyright file="VatReturnsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="VatReturn"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple VAT returns.
/// </remarks>
/// <seealso cref="VatReturn"/>
public record VatReturnsRoot
{
    /// <summary>
    /// Gets the collection of VAT returns from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="VatReturn"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("vat_returns")]
    public List<VatReturn> VatReturns { get; init; } = [];
}