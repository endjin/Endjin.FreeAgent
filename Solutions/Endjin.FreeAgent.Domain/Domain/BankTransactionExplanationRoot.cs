// <copyright file="BankTransactionExplanationRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a single <see cref="Domain.BankTransactionExplanation"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single bank transaction explanation object.
/// </remarks>
/// <seealso cref="BankTransactionExplanation"/>
public record BankTransactionExplanationRoot
{
    /// <summary>
    /// Gets the bank transaction explanation from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.BankTransactionExplanation"/> object returned by the API.
    /// </value>
    [JsonPropertyName("bank_transaction_explanation")]
    public BankTransactionExplanation? BankTransactionExplanation { get; init; }
}