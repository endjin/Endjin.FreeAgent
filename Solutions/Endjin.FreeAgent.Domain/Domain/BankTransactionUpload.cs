// <copyright file="BankTransactionUpload.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a single bank transaction for upload to FreeAgent via JSON or XML array.
/// </summary>
/// <remarks>
/// <para>
/// This type is used when uploading transactions as a JSON or XML array rather than as a bank statement file.
/// It allows direct creation of bank transactions with specified amounts, dates, and descriptions without
/// requiring a bank statement file.
/// </para>
/// <para>
/// The API performs automatic deduplication based on date, amount, and description matching. Transactions
/// that already exist in the system will not be duplicated.
/// </para>
/// <para>
/// API Endpoint: POST /v2/bank_transactions/statement
/// </para>
/// </remarks>
/// <seealso cref="BankTransaction"/>
/// <seealso cref="StatementUpload"/>
public record BankTransactionUpload
{
    /// <summary>
    /// Gets the date when the transaction occurred.
    /// </summary>
    /// <value>
    /// The transaction date in YYYY-MM-DD format. This field is required.
    /// </value>
    [JsonPropertyName("dated_on")]
    public DateOnly DatedOn { get; init; }

    /// <summary>
    /// Gets the description of the transaction.
    /// </summary>
    /// <value>
    /// A description of the transaction. Defaults to empty string if not provided.
    /// </value>
    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the transaction amount.
    /// </summary>
    /// <value>
    /// The monetary value of the transaction. Positive values represent money received (credits),
    /// negative values represent money paid out (debits). Defaults to 0 if not provided.
    /// </value>
    [JsonPropertyName("amount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Amount { get; init; }

    /// <summary>
    /// Gets the Financial Institution Transaction ID for deduplication.
    /// </summary>
    /// <value>
    /// A unique identifier from the financial institution used to prevent duplicate imports.
    /// Transactions with the same FITID are considered duplicates. Defaults to null if not provided.
    /// </value>
    [JsonPropertyName("fitid")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Fitid { get; init; }

    /// <summary>
    /// Gets the transaction type classification.
    /// </summary>
    /// <value>
    /// The transaction type from the OFX/QFX standard. Valid values include: CREDIT, DEBIT, INT, DIV,
    /// FEE, SRVCHG, DEP, ATM, POS, XFER, CHECK, PAYMENT, CASH, DIRECTDEP, DIRECTDEBIT, REPEATPMT, OTHER.
    /// Defaults to "OTHER" if not provided.
    /// </value>
    [JsonPropertyName("transaction_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TransactionType { get; init; }
}
