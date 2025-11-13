// <copyright file="InvoiceEmailRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for invoice email operations.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses when working with invoice email operations.
/// </remarks>
/// <seealso cref="InvoiceEmailWrapper"/>
/// <seealso cref="InvoiceEmail"/>
/// <seealso cref="Invoice"/>
public record InvoiceEmailRoot
{
    /// <summary>
    /// Gets the invoice email wrapper from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="InvoiceEmailWrapper"/> object containing email configuration.
    /// </value>
    [JsonPropertyName("invoice")]
    public InvoiceEmailWrapper? Invoice { get; init; }
}