// <copyright file="CreditNoteEmailWrapper.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a wrapper object for sending credit note emails via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This wrapper type encapsulates an <see cref="InvoiceEmail"/> object for API requests when sending
/// credit notes via email. Despite the name, credit notes reuse the <see cref="InvoiceEmail"/> type
/// as both documents share the same email structure and fields.
/// </para>
/// <para>
/// The FreeAgent API requires email data to be wrapped in an "email" property at the root level
/// for the send email endpoints. This maintains API consistency with invoice email sending.
/// </para>
/// <para>
/// Example JSON structure:
/// <code>
/// {
///   "email": {
///     "to": "customer@example.com",
///     "subject": "Credit Note #456",
///     "body": "Please find your credit note attached.",
///     "send_pdf_attachment": true
///   }
/// }
/// </code>
/// </para>
/// <para>
/// This wrapper pattern is common in REST APIs to provide namespacing and future extensibility
/// for request payloads.
/// </para>
/// </remarks>
/// <seealso cref="InvoiceEmail"/>
/// <seealso cref="CreditNote"/>
public record CreditNoteEmailWrapper
{
    /// <summary>
    /// Gets the credit note email configuration for sending.
    /// </summary>
    /// <value>
    /// An <see cref="InvoiceEmail"/> object containing all email details including recipients,
    /// subject, body, and attachment preferences. Note that the same InvoiceEmail type is used
    /// for both invoices and credit notes.
    /// </value>
    [JsonPropertyName("email")]
    public InvoiceEmail? Email { get; init; }
}