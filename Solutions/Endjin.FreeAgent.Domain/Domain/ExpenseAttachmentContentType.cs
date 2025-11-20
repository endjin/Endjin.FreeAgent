// <copyright file="ExpenseAttachmentContentType.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Defines standard MIME content types for expense receipt attachments in the FreeAgent system.
/// </summary>
/// <remarks>
/// <para>
/// This static class provides constants for the supported file formats when uploading expense receipts
/// and supporting documentation. These MIME types are used in the <see cref="ExpenseAttachment.ContentType"/>
/// property to identify the file format.
/// </para>
/// <para>
/// Supported formats include common image types (PNG, JPEG, GIF) for photographed or scanned receipts,
/// and PDF for digital receipts or multi-page documents. Using these constants ensures consistency
/// and avoids typos in content type strings.
/// </para>
/// <para>
/// For best practice, receipts should be clear and legible, with all transaction details visible
/// (date, merchant, amount, items purchased) to satisfy HMRC requirements for expense evidence.
/// </para>
/// </remarks>
/// <seealso cref="ExpenseAttachment"/>
/// <seealso cref="Expense"/>
public static class ExpenseAttachmentContentType
{
    /// <summary>
    /// Standard PNG image format (image/png).
    /// </summary>
    public const string Png = "image/png";

    /// <summary>
    /// Alternative PNG image format identifier (image/x-png).
    /// </summary>
    /// <remarks>
    /// This is a legacy MIME type for PNG images. Modern applications should use <see cref="Png"/> instead,
    /// but this constant is provided for compatibility with systems that use the x-png notation.
    /// </remarks>
    public const string XPng = "image/x-png";

    /// <summary>
    /// Standard JPEG image format (image/jpeg).
    /// </summary>
    public const string Jpeg = "image/jpeg";

    /// <summary>
    /// Alternative JPEG image format identifier (image/jpg).
    /// </summary>
    /// <remarks>
    /// Some systems use "jpg" instead of "jpeg" in the MIME type. This constant supports that variation.
    /// </remarks>
    public const string Jpg = "image/jpg";

    /// <summary>
    /// GIF image format (image/gif).
    /// </summary>
    public const string Gif = "image/gif";

    /// <summary>
    /// PDF document format (application/x-pdf).
    /// </summary>
    /// <remarks>
    /// PDF is ideal for multi-page receipts, digital receipts from online merchants, and consolidated
    /// expense documentation. Note this uses the x-pdf variant; the standard application/pdf may also be accepted.
    /// </remarks>
    public const string Pdf = "application/x-pdf";
}