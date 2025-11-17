// <copyright file="Attachment.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a file attachment in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Attachments are files uploaded to support financial records such as bills, expenses, bank transaction explanations,
/// and other documents. Common attachment types include receipts, invoices, contracts, and supporting documentation
/// in formats like PDF, images (JPG, PNG), and office documents.
/// </para>
/// <para>
/// Each attachment stores metadata including the filename, file size, content type (MIME type), and an optional
/// description. Attachments can be downloaded via their URL and are associated with specific financial records
/// for audit and compliance purposes.
/// </para>
/// <para>
/// <strong>Important:</strong> The FreeAgent API provides temporary URLs for accessing attachment content via the
/// <see cref="ContentSrc"/>, <see cref="ContentSrcMedium"/>, and <see cref="ContentSrcSmall"/> properties. These URLs
/// expire after the time specified in <see cref="ExpiresAt"/>. After expiration, you must fetch the attachment metadata
/// again to obtain new access URLs.
/// </para>
/// <para>
/// API Endpoint: /v2/attachments
/// </para>
/// <para>
/// Minimum Access Level: Varies by attached resource (typically Bills, Expenses, or Banking)
/// </para>
/// </remarks>
/// <seealso cref="Bill"/>
/// <seealso cref="Expense"/>
/// <seealso cref="BankTransactionExplanation"/>
public record Attachment
{
    /// <summary>
    /// Gets the unique URI identifier for this attachment.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this attachment in the FreeAgent system and serves as the download URL.
    /// </value>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the original filename of the uploaded file.
    /// </summary>
    /// <value>
    /// The name of the file as it was uploaded, including the file extension (e.g., "receipt.pdf", "invoice.jpg").
    /// </value>
    [JsonPropertyName("file_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Filename { get; init; }

    /// <summary>
    /// Gets the MIME content type of the attached file.
    /// </summary>
    /// <value>
    /// The content type identifier such as "application/pdf", "image/jpeg", "image/png", or other MIME types.
    /// </value>
    [JsonPropertyName("content_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ContentType { get; init; }

    /// <summary>
    /// Gets the file size in bytes.
    /// </summary>
    /// <value>
    /// The size of the attached file in bytes.
    /// </value>
    [JsonPropertyName("file_size")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? Size { get; init; }

    /// <summary>
    /// Gets the optional description or notes about this attachment.
    /// </summary>
    /// <value>
    /// Free-text description providing context about what the attachment contains or why it was uploaded.
    /// </value>
    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the date and time when this attachment was uploaded.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing the upload timestamp in UTC.
    /// </value>
    [JsonPropertyName("created_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? CreatedAt { get; init; }

    /// <summary>
    /// Gets the URL to access the original full-size file content.
    /// </summary>
    /// <value>
    /// A temporary URL that provides direct access to the original file. This URL expires at the time
    /// specified in <see cref="ExpiresAt"/>. After expiration, fetch the attachment metadata again
    /// to obtain a new URL.
    /// </value>
    [JsonPropertyName("content_src")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? ContentSrc { get; init; }

    /// <summary>
    /// Gets the URL to access a medium-sized thumbnail version of the file.
    /// </summary>
    /// <value>
    /// A temporary URL that provides access to a medium-sized thumbnail (if available for the file type).
    /// This property is null for non-image files. The URL expires at the time specified in <see cref="ExpiresAt"/>.
    /// </value>
    [JsonPropertyName("content_src_medium")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? ContentSrcMedium { get; init; }

    /// <summary>
    /// Gets the URL to access a small thumbnail version of the file.
    /// </summary>
    /// <value>
    /// A temporary URL that provides access to a small thumbnail (if available for the file type).
    /// This property is null for non-image files. The URL expires at the time specified in <see cref="ExpiresAt"/>.
    /// </value>
    [JsonPropertyName("content_src_small")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? ContentSrcSmall { get; init; }

    /// <summary>
    /// Gets the expiration timestamp for the content URLs.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> in UTC indicating when the <see cref="ContentSrc"/>, <see cref="ContentSrcMedium"/>,
    /// and <see cref="ContentSrcSmall"/> URLs will expire and no longer be accessible. After this time, you must
    /// fetch the attachment metadata again to obtain new temporary URLs.
    /// </value>
    [JsonPropertyName("expires_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? ExpiresAt { get; init; }
}