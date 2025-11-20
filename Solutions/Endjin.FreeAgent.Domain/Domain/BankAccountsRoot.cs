// <copyright file="BankAccountsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="BankAccount"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple bank account objects.
/// </remarks>
/// <seealso cref="BankAccount"/>
public record BankAccountsRoot
{
    /// <summary>
    /// Gets the collection of bank accounts from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="BankAccount"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("bank_accounts")]
    public List<BankAccount> BankAccounts { get; init; } = [];
}