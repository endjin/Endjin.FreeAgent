// <copyright file="CreditNoteRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a <see cref="Domain.CreditNote"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single credit note.
/// </remarks>
/// <seealso cref="CreditNote"/>
public record CreditNoteRoot
{
    /// <summary>
    /// Gets the credit note from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.CreditNote"/> object returned by the API.
    /// </value>
    [JsonPropertyName("credit_note")]
    public CreditNote? CreditNote { get; init; }
}