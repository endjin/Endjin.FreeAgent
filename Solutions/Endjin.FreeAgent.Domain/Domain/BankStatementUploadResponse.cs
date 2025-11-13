// <copyright file="BankStatementUploadResponse.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the response returned after uploading a bank statement for automated transaction import.
/// </summary>
/// <remarks>
/// <para>
/// After uploading a bank statement file via <see cref="BankStatementUpload"/>, FreeAgent processes the file
/// and returns this response detailing what happened during the import process.
/// </para>
/// <para>
/// The response provides comprehensive feedback about the import operation:
/// <list type="bullet">
/// <item>How many transactions were successfully imported as new transactions</item>
/// <item>How many transactions were detected as duplicates and skipped</item>
/// <item>How many transactions were ignored (e.g., invalid or unparseable entries)</item>
/// <item>The complete list of newly imported bank transactions</item>
/// <item>Any errors encountered during file parsing or import</item>
/// </list>
/// </para>
/// <para>
/// Understanding the response counts:
/// <list type="bullet">
/// <item><b>Imported</b> - New transactions successfully added to FreeAgent</item>
/// <item><b>Duplicate</b> - Transactions that already exist (matching date, amount, description)</item>
/// <item><b>Ignored</b> - Transactions that couldn't be processed (malformed data, unsupported transaction types)</item>
/// </list>
/// </para>
/// <para>
/// After a successful upload, the imported transactions appear in the bank account and are ready to be
/// explained (categorized and matched to invoices, bills, or expenses). Duplicate detection prevents
/// accidentally importing the same transactions multiple times if you upload overlapping statement periods.
/// </para>
/// <para>
/// If errors are present in the <see cref="Errors"/> list, the upload may have partially failed. Review
/// the errors to determine if you need to correct the file format or content and retry the upload.
/// </para>
/// </remarks>
/// <seealso cref="BankStatementUpload"/>
/// <seealso cref="BankTransaction"/>
/// <seealso cref="BankAccount"/>
public record BankStatementUploadResponse
{
    /// <summary>
    /// Gets the count of transactions that were successfully imported as new transactions.
    /// </summary>
    /// <value>
    /// The number of transactions from the statement file that were added to FreeAgent.
    /// These are new transactions that didn't already exist in the system.
    /// </value>
    [JsonPropertyName("imported_transaction_count")]
    public int? ImportedTransactionCount { get; init; }

    /// <summary>
    /// Gets the count of transactions that were detected as duplicates and skipped.
    /// </summary>
    /// <value>
    /// The number of transactions that matched existing transactions in FreeAgent (based on date, amount,
    /// and description). These transactions were not imported to avoid duplication.
    /// </value>
    [JsonPropertyName("duplicate_transaction_count")]
    public int? DuplicateTransactionCount { get; init; }

    /// <summary>
    /// Gets the count of transactions that were ignored during import.
    /// </summary>
    /// <value>
    /// The number of transactions that could not be processed or were intentionally skipped.
    /// This may include malformed entries, unsupported transaction types, or transactions
    /// that don't contain sufficient information for import.
    /// </value>
    [JsonPropertyName("ignored_transaction_count")]
    public int? IgnoredTransactionCount { get; init; }

    /// <summary>
    /// Gets the list of bank transactions that were successfully imported.
    /// </summary>
    /// <value>
    /// A collection of <see cref="BankTransaction"/> objects representing the newly imported transactions.
    /// Each transaction includes all details parsed from the statement file and is now ready to be
    /// explained (categorized). This list does not include duplicate or ignored transactions.
    /// </value>
    [JsonPropertyName("bank_transactions")]
    public List<BankTransaction>? BankTransactions { get; init; }

    /// <summary>
    /// Gets the list of errors encountered during the statement upload and import process.
    /// </summary>
    /// <value>
    /// A collection of error messages describing any problems that occurred while parsing or importing
    /// the statement file. Common errors include:
    /// <list type="bullet">
    /// <item>Unrecognized or invalid file format</item>
    /// <item>Malformed data within the file</item>
    /// <item>Missing required fields (date, amount, description)</item>
    /// <item>Date format inconsistencies</item>
    /// <item>File encoding issues</item>
    /// </list>
    /// If this list is empty or null, the upload was completely successful.
    /// </value>
    [JsonPropertyName("errors")]
    public List<string>? Errors { get; init; }
}