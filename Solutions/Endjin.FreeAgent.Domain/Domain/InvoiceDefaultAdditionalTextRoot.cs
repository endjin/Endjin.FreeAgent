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
/// <seealso cref="Invoice"/>
public record InvoiceDefaultAdditionalTextRoot
{
    /// <summary>
    /// Gets the default additional text string from the API response.
    /// </summary>
    /// <value>
    /// The default text content that is automatically added to new invoices. This can include
    /// payment terms, bank details, or other standard invoice footer information.
    /// </value>
    [JsonPropertyName("default_additional_text")]
    public string? DefaultAdditionalText { get; init; }
}
