// <copyright file="EstimatePdfRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root wrapper for an estimate PDF response from the FreeAgent API.
/// </summary>
[ExcludeFromCodeCoverage]
public record EstimatePdfRoot
{
    /// <summary>
    /// Gets the PDF object containing the base64-encoded content.
    /// </summary>
    [JsonPropertyName("pdf")]
    public EstimatePdf? Pdf { get; init; }
}