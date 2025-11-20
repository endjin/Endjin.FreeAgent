// <copyright file="EmailAttachment.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a file attachment for invoice or estimate emails.
/// </summary>
/// <remarks>
/// <para>
/// Email attachments allow additional files to be included when sending invoices or estimates
/// to customers. Each attachment includes the file name, content type (MIME type), and the
/// file data encoded in base64 format.
/// </para>
/// <para>
/// The FreeAgent API enforces a maximum size of 5MB per attachment. The total size of all
/// attachments should not exceed reasonable email limits (typically 10-25MB total).
/// </para>
/// <para>
/// Common use cases include attaching:
/// <list type="bullet">
/// <item>Supporting documentation (contracts, specifications)</item>
/// <item>Terms and conditions documents</item>
/// <item>Delivery notes or receipts</item>
/// <item>Project-specific files</item>
/// </list>
/// </para>
/// </remarks>
/// <seealso cref="InvoiceEmail"/>
/// <seealso cref="EstimateEmail"/>
[DebuggerDisplay("{" + nameof(FileName) + "}")]
public record EmailAttachment
{
    /// <summary>
    /// Gets the name of the file to be attached.
    /// </summary>
    /// <value>
    /// The filename including extension (e.g., "terms-and-conditions.pdf", "specification.docx").
    /// This name will be visible to email recipients and used when they save the attachment.
    /// </value>
    [JsonPropertyName("file_name")]
    public string? FileName { get; init; }

    /// <summary>
    /// Gets the MIME content type of the file.
    /// </summary>
    /// <value>
    /// The MIME type describing the file format (e.g., "application/pdf", "image/png",
    /// "application/vnd.openxmlformats-officedocument.wordprocessingml.document" for .docx).
    /// This helps email clients properly handle and display the attachment.
    /// </value>
    [JsonPropertyName("content_type")]
    public string? ContentType { get; init; }

    /// <summary>
    /// Gets the base64-encoded file content.
    /// </summary>
    /// <value>
    /// The file data encoded in base64 format as per RFC 2045. The original binary file
    /// should be converted to base64 before setting this property. Maximum size after
    /// encoding is 5MB per attachment.
    /// </value>
    /// <remarks>
    /// Base64 encoding increases the file size by approximately 33%. For example, a 3.75MB
    /// binary file will be approximately 5MB when base64-encoded, reaching the API limit.
    /// </remarks>
    [JsonPropertyName("data")]
    public string? Data { get; init; }
}
