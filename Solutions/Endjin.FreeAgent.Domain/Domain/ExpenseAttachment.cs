// <copyright file="ExpenseAttachment.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a file attachment (typically a receipt) associated with an expense in the FreeAgent system.
/// </summary>
/// <remarks>
/// <para>
/// Expense attachments allow users to upload digital copies of receipts, invoices, and other supporting
/// documentation for expense claims. This is essential for HMRC compliance, audit trails, and expense
/// verification.
/// </para>
/// <para>
/// HMRC requires businesses to retain valid receipts as evidence for allowable business expenses. Digital
/// receipts uploaded to FreeAgent satisfy these requirements when they clearly show:
/// <list type="bullet">
/// <item>Date of purchase</item>
/// <item>Merchant/supplier name</item>
/// <item>Amount paid</item>
/// <item>Items or services purchased</item>
/// <item>VAT amount (if applicable)</item>
/// </list>
/// </para>
/// <para>
/// Attachments are uploaded as Base64-encoded data, which allows binary image and PDF files to be transmitted
/// via JSON. The file is encoded client-side before sending to the API and decoded by FreeAgent for storage.
/// </para>
/// <para>
/// Supported file formats:
/// <list type="bullet">
/// <item>PNG - photographs or screenshots of receipts</item>
/// <item>JPEG/JPG - photographs of receipts (most common from mobile cameras)</item>
/// <item>GIF - images of receipts</item>
/// <item>PDF - digital receipts or scanned multi-page documents</item>
/// </list>
/// </para>
/// <para>
/// Best practices:
/// <list type="bullet">
/// <item>Upload receipts as soon as possible after purchase</item>
/// <item>Ensure images are clear and legible</item>
/// <item>Include descriptions to aid searching and categorization</item>
/// <item>Use meaningful filenames (e.g., "2024-01-15-coffee-shop-receipt.jpg")</item>
/// </list>
/// </para>
/// </remarks>
/// <seealso cref="Expense"/>
/// <seealso cref="ExpenseAttachmentContentType"/>
/// <seealso cref="Attachment"/>
public record ExpenseAttachment
{
    /// <summary>
    /// Gets the file content encoded as a Base64 string.
    /// </summary>
    /// <value>
    /// The complete file content (image or PDF) encoded in Base64 format. The file should be read as binary
    /// and converted to Base64 before including in this field. Upon upload, FreeAgent decodes this data
    /// and stores the file. This field is required when creating an attachment.
    /// </value>
    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Data { get; init; }

    /// <summary>
    /// Gets the filename for the attached file.
    /// </summary>
    /// <value>
    /// The original filename including extension (e.g., "receipt.jpg", "invoice.pdf").
    /// This helps identify the attachment and is displayed in the FreeAgent interface.
    /// Use descriptive filenames for easier identification.
    /// </value>
    [JsonPropertyName("file_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? FileName { get; init; }

    /// <summary>
    /// Gets a text description of the attachment.
    /// </summary>
    /// <value>
    /// An optional description providing context about the attachment (e.g., "Receipt for client lunch",
    /// "Invoice from supplier"). This aids in searching and understanding the attachment without viewing it.
    /// </value>
    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the MIME content type of the attached file.
    /// </summary>
    /// <value>
    /// The MIME type identifying the file format (e.g., "image/jpeg", "image/png", "application/x-pdf").
    /// Use constants from <see cref="ExpenseAttachmentContentType"/> for standard formats.
    /// This field is required and must match the actual file type.
    /// </value>
    [JsonPropertyName("content_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ContentType { get; init; }
}