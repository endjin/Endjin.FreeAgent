// <copyright file="BankTransactionUploadRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Text.Json.Serialization;

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root wrapper for uploading an array of bank transactions to FreeAgent.
/// </summary>
/// <remarks>
/// <para>
/// This type wraps an array of <see cref="BankTransactionUpload"/> objects for submission to the
/// FreeAgent API when uploading transactions via JSON or XML format.
/// </para>
/// <para>
/// The API accepts an array of transaction objects and automatically creates bank transactions,
/// performing deduplication based on date, amount, and description matching.
/// </para>
/// <para>
/// API Endpoint: POST /v2/bank_transactions/statement
/// Content-Type: application/json or application/xml
/// </para>
/// </remarks>
/// <seealso cref="BankTransactionUpload"/>
/// <seealso cref="BankTransaction"/>
public record BankTransactionUploadRoot
{
    /// <summary>
    /// Gets the array of bank transactions to upload.
    /// </summary>
    /// <value>
    /// A collection of <see cref="BankTransactionUpload"/> objects representing the transactions to create.
    /// </value>
    [JsonPropertyName("statement")]
    public IEnumerable<BankTransactionUpload>? Statement { get; init; }
}
