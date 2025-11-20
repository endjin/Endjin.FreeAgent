// <copyright file="BankAccountOpeningBalance.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a bank account opening balance entry within a journal set.
/// </summary>
/// <remarks>
/// <para>
/// This record is used specifically within the opening balances journal set to represent the initial
/// balance of a bank account. It contains the bank account reference, a description, and the debit value.
/// </para>
/// <para>
/// See <see cref="JournalSet"/> for context on opening balances.
/// </para>
/// </remarks>
public record BankAccountOpeningBalance
{
    /// <summary>
    /// Gets the URI reference to the bank account.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="BankAccount"/> associated with this opening balance.
    /// </value>
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the description of the opening balance.
    /// </summary>
    /// <value>
    /// A text description, typically "Default bank account" or similar.
    /// </value>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the debit value of the opening balance.
    /// </summary>
    /// <value>
    /// The monetary amount. Positive values represent debits, negative values represent credits.
    /// </value>
    [JsonPropertyName("debit_value")]
    public decimal? DebitValue { get; init; }
}
