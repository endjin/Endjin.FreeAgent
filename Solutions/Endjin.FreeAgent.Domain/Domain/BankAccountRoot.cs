// <copyright file="BankAccountRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

using System.Text.Json.Serialization;

/// <summary>
/// Represents the root-level JSON response wrapper for a single <see cref="Domain.BankAccount"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single bank account object.
/// </remarks>
/// <seealso cref="BankAccount"/>
public record BankAccountRoot
{
    /// <summary>
    /// Gets the bank account from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.BankAccount"/> object returned by the API.
    /// </value>
    [JsonPropertyName("bank_account")]
    public BankAccount? BankAccount { get; init; }
}