// <copyright file="InvoicePdf.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the PDF content response from the FreeAgent API for invoices.
/// </summary>
[ExcludeFromCodeCoverage]
public record InvoicePdf
{
    /// <summary>
    /// Gets the base64-encoded PDF content.
    /// </summary>
    /// <remarks>
    /// The API returns a base64-encoded representation of the PDF data.
    /// This content needs to be decoded to obtain the actual PDF bytes.
    /// </remarks>
    [JsonPropertyName("content")]
    public string? Content { get; init; }
}
