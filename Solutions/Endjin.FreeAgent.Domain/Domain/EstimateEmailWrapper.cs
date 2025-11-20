// <copyright file="EstimateEmailWrapper.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a wrapper object for sending estimate emails via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This wrapper type encapsulates an <see cref="EstimateEmail"/> object for API requests when sending
/// estimates via email. The FreeAgent API requires email data to be wrapped in an "email" property
/// at the root level for the send email endpoints.
/// </para>
/// <para>
/// Example JSON structure:
/// <code>
/// {
///   "email": {
///     "to": "customer@example.com",
///     "subject": "Estimate #12345",
///     "body": "Please find your estimate attached.",
///     "send_pdf_attachment": true
///   }
/// }
/// </code>
/// </para>
/// </remarks>
/// <seealso cref="EstimateEmail"/>
/// <seealso cref="Estimate"/>
public record EstimateEmailWrapper
{
    /// <summary>
    /// Gets the estimate email configuration for sending.
    /// </summary>
    /// <value>
    /// An <see cref="EstimateEmail"/> object containing all email details including recipients,
    /// subject, body, and attachment preferences.
    /// </value>
    [JsonPropertyName("email")]
    public EstimateEmail? Email { get; init; }
}
