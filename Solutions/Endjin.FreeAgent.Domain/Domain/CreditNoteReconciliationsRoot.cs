// <copyright file="CreditNoteReconciliationsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="CreditNoteReconciliation"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple credit note reconciliation objects.
/// </remarks>
/// <seealso cref="CreditNoteReconciliation"/>
public record CreditNoteReconciliationsRoot
{
    /// <summary>
    /// Gets the collection of credit note reconciliations from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="CreditNoteReconciliation"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("credit_note_reconciliations")]
    public List<CreditNoteReconciliation> CreditNoteReconciliations { get; init; } = [];
}
