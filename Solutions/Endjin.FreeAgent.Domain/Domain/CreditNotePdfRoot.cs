// <copyright file="CreditNotePdfRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root wrapper for a credit note PDF response from the FreeAgent API.
/// </summary>
[ExcludeFromCodeCoverage]
public record CreditNotePdfRoot
{
    /// <summary>
    /// Gets the PDF object containing the base64-encoded content.
    /// </summary>
    [JsonPropertyName("pdf")]
    public CreditNotePdf? Pdf { get; init; }
}