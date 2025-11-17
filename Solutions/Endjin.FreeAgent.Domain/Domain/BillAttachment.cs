// <copyright file="BillAttachment.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

using System.Text.Json.Serialization;

/// <summary>
/// Represents an attachment to be uploaded with a bill.
/// </summary>
/// <remarks>
/// <para>
/// Attachments can be included when creating or updating bills.
/// The file data must be base64-encoded binary data.
/// </para>
/// <para>
/// Supported content types:
/// <list type="bullet">
/// <item><description>image/png</description></item>
/// <item><description>image/x-png</description></item>
/// <item><description>image/jpeg</description></item>
/// <item><description>image/jpg</description></item>
/// <item><description>image/gif</description></item>
/// <item><description>application/x-pdf</description></item>
/// </list>
/// </para>
/// <para>
/// Maximum file size: 5MB.
/// </para>
/// </remarks>
public sealed record BillAttachment
{
    /// <summary>
    /// Gets the base64-encoded binary data of the file.
    /// </summary>
    /// <remarks>
    /// The file contents must be encoded as a base64 string before setting this property.
    /// </remarks>
    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Data { get; init; }

    /// <summary>
    /// Gets the name of the file including the extension.
    /// </summary>
    /// <remarks>
    /// Example: "invoice.pdf" or "receipt.png".
    /// </remarks>
    [JsonPropertyName("file_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? FileName { get; init; }

    /// <summary>
    /// Gets the MIME type of the file.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Supported values:
    /// <list type="bullet">
    /// <item><description>image/png</description></item>
    /// <item><description>image/x-png</description></item>
    /// <item><description>image/jpeg</description></item>
    /// <item><description>image/jpg</description></item>
    /// <item><description>image/gif</description></item>
    /// <item><description>application/x-pdf</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    [JsonPropertyName("content_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ContentType { get; init; }

    /// <summary>
    /// Gets an optional description of the attachment.
    /// </summary>
    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; init; }
}
