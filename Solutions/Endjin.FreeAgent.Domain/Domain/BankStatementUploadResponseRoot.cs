// <copyright file="BankStatementUploadResponseRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper returned after uploading a bank statement file.
/// </summary>
/// <remarks>
/// <para>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses after a bank statement
/// upload. The API returns the import summary wrapped in an "import_summary" property at the root level.
/// </para>
/// <para>
/// The response contains detailed information about the import operation, including counts of imported,
/// duplicate, and ignored transactions, along with the list of newly created bank transactions and any
/// errors encountered during processing.
/// </para>
/// <para>
/// Example JSON structure:
/// <code>
/// {
///   "import_summary": {
///     "imported_transaction_count": 25,
///     "duplicate_transaction_count": 3,
///     "ignored_transaction_count": 0,
///     "bank_transactions": [...],
///     "errors": []
///   }
/// }
/// </code>
/// </para>
/// </remarks>
/// <seealso cref="BankStatementUploadResponse"/>
/// <seealso cref="BankStatementUploadRoot"/>
/// <seealso cref="BankTransaction"/>
public record BankStatementUploadResponseRoot
{
    /// <summary>
    /// Gets the bank statement import summary from the API response.
    /// </summary>
    /// <value>
    /// A <see cref="BankStatementUploadResponse"/> object containing statistics about the import
    /// operation, including transaction counts, the list of imported transactions, and any errors.
    /// </value>
    [JsonPropertyName("import_summary")]
    public BankStatementUploadResponse? ImportSummary { get; init; }
}