// <copyright file="BankStatementUpload.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a bank statement file upload request for automated transaction import in the FreeAgent system.
/// </summary>
/// <remarks>
/// <para>
/// Bank statement uploads allow businesses to import transactions directly from bank statement files rather than
/// manually entering each transaction. This automation saves time and reduces data entry errors.
/// </para>
/// <para>
/// Supported file formats typically include:
/// <list type="bullet">
/// <item><b>OFX</b> - Open Financial Exchange format, commonly exported by banks and accounting software</item>
/// <item><b>QIF</b> - Quicken Interchange Format, an older but still widely supported format</item>
/// <item><b>CSV</b> - Comma-separated values, though format varies by bank</item>
/// <item><b>MT940</b> - SWIFT format used for corporate banking</item>
/// </list>
/// </para>
/// <para>
/// The upload process:
/// <list type="number">
/// <item>Read the statement file and encode it as Base64</item>
/// <item>Create a BankStatementUpload with the encoded content and file type</item>
/// <item>POST to the FreeAgent API</item>
/// <item>Receive a <see cref="BankStatementUploadResponse"/> with import results</item>
/// <item>FreeAgent automatically matches and imports transactions, detecting duplicates</item>
/// </list>
/// </para>
/// <para>
/// FreeAgent intelligently handles duplicate detection by comparing transaction amounts, dates, and descriptions
/// to avoid importing the same transactions multiple times if you upload overlapping statements.
/// </para>
/// <para>
/// API Access: Accessible via POST /v2/bank_statement_uploads
/// Minimum Access Level: Bank transaction management access
/// </para>
/// </remarks>
/// <seealso cref="BankStatementUploadResponse"/>
/// <seealso cref="BankAccount"/>
/// <seealso cref="BankTransaction"/>
public record BankStatementUpload
{
    /// <summary>
    /// Gets the API URL of the bank account to which these transactions should be imported.
    /// </summary>
    /// <value>
    /// A reference to the <see cref="BankAccount"/> resource. All transactions in the statement
    /// will be imported into this account.
    /// </value>
    [JsonPropertyName("bank_account")]
    public Uri? BankAccount { get; init; }

    /// <summary>
    /// Gets the bank statement file content encoded as a Base64 string.
    /// </summary>
    /// <value>
    /// The complete file content of the bank statement file, encoded in Base64 format.
    /// The file should be read as binary and converted to Base64 before including in this field.
    /// Maximum file size varies but is typically limited to a few MB.
    /// </value>
    [JsonPropertyName("statement")]
    public string? Statement { get; init; }

    /// <summary>
    /// Gets the file type/format of the uploaded statement.
    /// </summary>
    /// <value>
    /// The file format identifier (e.g., "ofx", "qif", "csv", "mt940"). This helps FreeAgent
    /// parse the file correctly. The format must match the actual file content.
    /// </value>
    [JsonPropertyName("file_type")]
    public string? FileType { get; init; }
}