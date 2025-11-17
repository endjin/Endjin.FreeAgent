// <copyright file="InvoiceDefaultAdditionalTextRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for invoice default additional text.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses when retrieving
/// or updating default additional text for invoices.
/// </remarks>
/// <seealso cref="InvoiceDefaultAdditionalText"/>
/// <seealso cref="Invoice"/>
public record InvoiceDefaultAdditionalTextRoot
{
    /// <summary>
    /// Gets the default additional text object from the API response.
    /// </summary>
    /// <value>
    /// An <see cref="InvoiceDefaultAdditionalText"/> object containing the default text template.
    /// </value>
    [JsonPropertyName("invoice")]
    public InvoiceDefaultAdditionalText? Invoice { get; init; }
}
