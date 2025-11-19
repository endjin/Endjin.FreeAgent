// <copyright file="StockItemOpeningBalance.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Text.Json.Serialization;

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a stock item opening balance entry within a journal set.
/// </summary>
/// <remarks>
/// <para>
/// This record is used specifically within the opening balances journal set to represent the initial
/// value of a stock item. It contains the stock item reference, a description, and the debit value.
/// </para>
/// <para>
/// See <see cref="JournalSet"/> for context on opening balances.
/// </para>
/// </remarks>
public record StockItemOpeningBalance
{
    /// <summary>
    /// Gets the URI reference to the stock item.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="StockItem"/> associated with this opening balance.
    /// </value>
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the description of the opening balance.
    /// </summary>
    /// <value>
    /// A text description, typically "Opening Balance for Stock Item: [Name]".
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
