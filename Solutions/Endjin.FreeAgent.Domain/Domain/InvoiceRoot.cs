// <copyright file="InvoiceRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a single <see cref="Domain.Invoice"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single invoice object.
/// </remarks>
/// <seealso cref="Invoice"/>
public record InvoiceRoot
{
    /// <summary>
    /// Gets the invoice from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.Invoice"/> object returned by the API.
    /// </value>
    [JsonPropertyName("invoice")]
    public Invoice? Invoice { get; init; }
}