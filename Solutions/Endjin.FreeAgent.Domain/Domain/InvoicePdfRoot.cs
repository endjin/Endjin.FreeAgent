// <copyright file="InvoicePdfRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root wrapper for an invoice PDF response from the FreeAgent API.
/// </summary>
[ExcludeFromCodeCoverage]
public record InvoicePdfRoot
{
    /// <summary>
    /// Gets the PDF object containing the base64-encoded content.
    /// </summary>
    [JsonPropertyName("pdf")]
    public InvoicePdf? Pdf { get; init; }
}
