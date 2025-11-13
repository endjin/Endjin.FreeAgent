// <copyright file="BankTransactionExplanationsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="BankTransactionExplanation"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple bank transaction explanation objects.
/// </remarks>
/// <seealso cref="BankTransactionExplanation"/>
public record BankTransactionExplanationsRoot
{
    /// <summary>
    /// Gets the collection of bank transaction explanations from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="BankTransactionExplanation"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("bank_transaction_explanations")]
    public List<BankTransactionExplanation> BankTransactionExplanations { get; init; } = [];
}