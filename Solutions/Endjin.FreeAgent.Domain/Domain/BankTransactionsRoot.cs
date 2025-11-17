// <copyright file="BankTransactionsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Text.Json.Serialization;

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="BankTransaction"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple bank transaction objects.
/// </remarks>
/// <seealso cref="BankTransaction"/>
public record BankTransactionsRoot
{
    /// <summary>
    /// Gets the collection of bank transactions from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="BankTransaction"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("bank_transactions")]
    public List<BankTransaction> BankTransactions { get; init; } = [];
}