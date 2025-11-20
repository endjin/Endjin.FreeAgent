// <copyright file="CreditNoteReconciliationRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a <see cref="Domain.CreditNoteReconciliation"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single credit note reconciliation.
/// </remarks>
/// <seealso cref="CreditNoteReconciliation"/>
public record CreditNoteReconciliationRoot
{
    /// <summary>
    /// Gets the credit note reconciliation from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.CreditNoteReconciliation"/> object returned by the API.
    /// </value>
    [JsonPropertyName("credit_note_reconciliation")]
    public CreditNoteReconciliation? CreditNoteReconciliation { get; init; }
}
