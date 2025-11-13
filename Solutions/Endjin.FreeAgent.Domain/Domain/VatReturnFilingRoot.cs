// <copyright file="VatReturnFilingRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON request wrapper for <see cref="Domain.VatReturnFiling"/> data.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON serialization when recording VAT return filing details via the FreeAgent API.
/// </remarks>
/// <seealso cref="VatReturnFiling"/>
/// <seealso cref="VatReturn"/>
public record VatReturnFilingRoot
{
    /// <summary>
    /// Gets the VAT return filing data for the API request.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.VatReturnFiling"/> object containing filing details.
    /// </value>
    [JsonPropertyName("vat_return")]
    public VatReturnFiling? VatReturn { get; init; }
}