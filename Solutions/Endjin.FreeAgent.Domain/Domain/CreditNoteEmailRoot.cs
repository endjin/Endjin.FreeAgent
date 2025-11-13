// <copyright file="CreditNoteEmailRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for credit note email operations.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses when working with credit note email operations.
/// </remarks>
/// <seealso cref="CreditNoteEmailWrapper"/>
/// <seealso cref="InvoiceEmail"/>
/// <seealso cref="CreditNote"/>
public record CreditNoteEmailRoot
{
    /// <summary>
    /// Gets the credit note email wrapper from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="CreditNoteEmailWrapper"/> object containing email configuration.
    /// </value>
    [JsonPropertyName("credit_note")]
    public CreditNoteEmailWrapper? CreditNote { get; init; }
}