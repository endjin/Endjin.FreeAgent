// <copyright file="VatReturnRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a <see cref="Domain.VatReturn"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single VAT return.
/// </remarks>
/// <seealso cref="VatReturn"/>
public record VatReturnRoot
{
    /// <summary>
    /// Gets the VAT return from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.VatReturn"/> object returned by the API.
    /// </value>
    [JsonPropertyName("vat_return")]
    public VatReturn? VatReturn { get; init; }
}