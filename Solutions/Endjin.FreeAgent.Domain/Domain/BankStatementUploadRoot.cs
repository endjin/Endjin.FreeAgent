// <copyright file="BankStatementUploadRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON request wrapper for uploading a bank statement file.
/// </summary>
/// <remarks>
/// <para>
/// This wrapper type is used for JSON serialization when posting bank statement uploads to the FreeAgent API.
/// The API requires the bank statement upload data to be wrapped in a "statement" property at the root level.
/// </para>
/// <para>
/// Example JSON structure:
/// <code>
/// {
///   "statement": {
///     "bank_account": "https://api.freeagent.com/v2/bank_accounts/123",
///     "statement": "base64_encoded_file_content...",
///     "file_type": "ofx"
///   }
/// }
/// </code>
/// </para>
/// </remarks>
/// <seealso cref="BankStatementUpload"/>
/// <seealso cref="BankStatementUploadResponseRoot"/>
public record BankStatementUploadRoot
{
    /// <summary>
    /// Gets the bank statement upload request data.
    /// </summary>
    /// <value>
    /// A <see cref="BankStatementUpload"/> object containing the bank account reference,
    /// Base64-encoded statement file content, and file type.
    /// </value>
    [JsonPropertyName("statement")]
    public BankStatementUpload? Statement { get; init; }
}