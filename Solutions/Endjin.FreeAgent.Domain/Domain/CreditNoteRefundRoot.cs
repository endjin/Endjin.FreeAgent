// <copyright file="CreditNoteRefundRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON request wrapper for a <see cref="Domain.CreditNoteRefund"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON serialization when creating credit note refund records via the FreeAgent API.
/// </remarks>
/// <seealso cref="CreditNoteRefund"/>
/// <seealso cref="CreditNote"/>
public record CreditNoteRefundRoot
{
    /// <summary>
    /// Gets the credit note refund data for the API request.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.CreditNoteRefund"/> object containing refund details.
    /// </value>
    [JsonPropertyName("credit_note")]
    public CreditNoteRefund? CreditNote { get; init; }
}