// <copyright file="InvoiceEmailWrapper.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a wrapper object for sending invoice emails via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This wrapper type encapsulates an <see cref="InvoiceEmail"/> object for API requests when sending
/// invoices via email. The FreeAgent API requires email data to be wrapped in an "email" property
/// at the root level for the send email endpoints.
/// </para>
/// <para>
/// Example JSON structure:
/// <code>
/// {
///   "email": {
///     "to": "customer@example.com",
///     "subject": "Invoice #12345",
///     "body": "Please find your invoice attached.",
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
/// <seealso cref="Invoice"/>
public record InvoiceEmailWrapper
{
    /// <summary>
    /// Gets the invoice email configuration for sending.
    /// </summary>
    /// <value>
    /// An <see cref="InvoiceEmail"/> object containing all email details including recipients,
    /// subject, body, and attachment preferences.
    /// </value>
    [JsonPropertyName("email")]
    public InvoiceEmail? Email { get; init; }
}