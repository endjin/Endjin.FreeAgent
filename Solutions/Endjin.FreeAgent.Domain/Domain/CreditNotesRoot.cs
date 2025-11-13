// <copyright file="CreditNotesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="CreditNote"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple credit note objects.
/// </remarks>
/// <seealso cref="CreditNote"/>
public record CreditNotesRoot
{
    /// <summary>
    /// Gets the collection of credit notes from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="CreditNote"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("credit_notes")]
    public List<CreditNote> CreditNotes { get; init; } = [];
}