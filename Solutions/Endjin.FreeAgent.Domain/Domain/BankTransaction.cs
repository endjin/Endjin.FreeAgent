// <copyright file="BankTransaction.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

/// <summary>
/// Represents a bank transaction in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Bank transactions represent individual movements of money in and out of bank accounts, whether imported
/// from bank feeds, statement uploads, or manually entered. Each transaction must be explained (categorized)
/// to properly reconcile accounts and generate accurate financial reports.
/// </para>
/// <para>
/// Transactions can be explained through <see cref="BankTransactionExplanation"/> objects that link them to
/// invoices, bills, expenses, transfers, or other accounting categories. A single transaction can have multiple
/// explanations to split the amount across different categories.
/// </para>
/// <para>
/// Locked transactions cannot be modified or deleted, typically because they belong to a finalized accounting period.
/// Manual transactions are user-entered rather than imported from bank feeds.
/// </para>
/// <para>
/// API Endpoint: /v2/bank_transactions
/// </para>
/// <para>
/// Minimum Access Level: Banking
/// </para>
/// </remarks>
/// <seealso cref="BankAccount"/>
/// <seealso cref="BankTransactionExplanation"/>
[DebuggerDisplay("Description = {" + nameof(Description) + "}, Amount = {" + nameof(Amount) + "}")]
public record BankTransaction
{
    /// <summary>
    /// Gets the unique URI identifier for this bank transaction.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this bank transaction in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the URI reference to the bank account containing this transaction.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.BankAccount"/> where this transaction occurred.
    /// </value>
    [JsonPropertyName("bank_account")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? BankAccount { get; init; }

    /// <summary>
    /// Gets the date when the transaction occurred.
    /// </summary>
    /// <value>
    /// The transaction date in YYYY-MM-DD format as recorded by the bank.
    /// </value>
    [JsonPropertyName("dated_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? DatedOn { get; init; }

    /// <summary>
    /// Gets the short description of the transaction.
    /// </summary>
    /// <value>
    /// A brief description of the transaction, typically from the bank statement.
    /// </value>
    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the complete detailed description of the transaction.
    /// </summary>
    /// <value>
    /// The full transaction description including all details provided by the bank.
    /// </value>
    [JsonPropertyName("full_description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? FullDescription { get; init; }

    /// <summary>
    /// Gets the transaction amount.
    /// </summary>
    /// <value>
    /// The monetary value of the transaction. Positive values represent money received (credits),
    /// negative values represent money paid out (debits).
    /// </value>
    [JsonPropertyName("amount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Amount { get; init; }

    /// <summary>
    /// Gets the portion of the transaction amount that has not yet been explained.
    /// </summary>
    /// <value>
    /// The unexplained balance remaining after partial explanations. When fully explained, this becomes zero.
    /// </value>
    [JsonPropertyName("unexplained_amount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? UnexplainedAmount { get; init; }

    /// <summary>
    /// Gets a value indicating whether this transaction has been fully explained.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the entire transaction amount has been categorized through explanations;
    /// otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("is_explained")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsExplained { get; init; }

    /// <summary>
    /// Gets a value indicating whether this transaction was manually entered.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the transaction was manually created by a user; <see langword="false"/>
    /// if imported from a bank feed or statement upload.
    /// </value>
    [JsonPropertyName("is_manual")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsManual { get; init; }

    /// <summary>
    /// Gets a value indicating whether this transaction is locked.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the transaction is locked and cannot be modified or deleted; otherwise,
    /// <see langword="false"/>. Transactions are typically locked when they belong to finalized accounting periods.
    /// </value>
    [JsonPropertyName("is_locked")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsLocked { get; init; }

    /// <summary>
    /// Gets the URI reference to the explanations for this transaction.
    /// </summary>
    /// <value>
    /// The URI endpoint for accessing the collection of <see cref="BankTransactionExplanation"/> objects
    /// that categorize this transaction.
    /// </value>
    [JsonPropertyName("bank_transaction_explanations")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? BankTransactionExplanations { get; init; }

    /// <summary>
    /// Gets the date and time when this transaction was uploaded.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing when the transaction was imported into FreeAgent,
    /// or <see langword="null"/> for manually created transactions.
    /// </value>
    [JsonPropertyName("uploaded_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? UploadedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this transaction record was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing the creation timestamp in UTC.
    /// </value>
    [JsonPropertyName("created_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this transaction record was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing the last modification timestamp in UTC.
    /// </value>
    [JsonPropertyName("updated_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? UpdatedAt { get; init; }
}