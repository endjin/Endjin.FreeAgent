// <copyright file="BankTransactionRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Text.Json.Serialization;

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a single <see cref="Domain.BankTransaction"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single bank transaction object.
/// </remarks>
/// <seealso cref="BankTransaction"/>
public record BankTransactionRoot
{
    /// <summary>
    /// Gets the bank transaction from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.BankTransaction"/> object returned by the API.
    /// </value>
    [JsonPropertyName("bank_transaction")]
    public BankTransaction? BankTransaction { get; init; }
}