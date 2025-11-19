// <copyright file="StatementUpload.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a bank statement file upload for automated transaction import in the FreeAgent system.
/// </summary>
/// <remarks>
/// <para>
/// This type is similar to <see cref="BankStatementUpload"/> and is used for uploading bank statement files
/// to automatically import transactions into FreeAgent. The file content is provided as a Base64-encoded string.
/// </para>
/// <para>
/// Supported file formats typically include OFX, QIF, CSV, and MT940. The upload process automatically
/// parses transactions, detects duplicates, and creates bank transaction records ready for explanation
/// (categorization and matching).
/// </para>
/// <para>
/// This automation significantly reduces manual data entry and helps maintain accurate, up-to-date bank
/// records for financial reporting and reconciliation.
/// </para>
/// <para>
/// Note: The bank_account parameter is passed as a query parameter in the API request, not in the request body.
/// See BankTransactions.UploadStatementAsync in the Client library for details on how statements are uploaded.
/// </para>
/// </remarks>
/// <seealso cref="BankStatementUpload"/>
/// <seealso cref="BankTransaction"/>
/// <seealso cref="BankAccount"/>
public record StatementUpload
{
    /// <summary>
    /// Gets the bank statement file content encoded as a Base64 string.
    /// </summary>
    /// <value>
    /// The complete file content of the bank statement, encoded in Base64 format for JSON transmission.
    /// </value>
    [JsonPropertyName("statement")]
    public string? Statement { get; init; }

    /// <summary>
    /// Gets the file type/format of the statement file.
    /// </summary>
    /// <value>
    /// The file format identifier (e.g., "ofx", "qif", "csv", "mt940") to help FreeAgent parse the file correctly.
    /// </value>
    [JsonPropertyName("file_type")]
    public string? FileType { get; init; }
}