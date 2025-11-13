// <copyright file="StockItemRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a <see cref="Domain.StockItem"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single stock item.
/// </remarks>
/// <seealso cref="StockItem"/>
public record StockItemRoot
{
    /// <summary>
    /// Gets the stock item from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.StockItem"/> object returned by the API.
    /// </value>
    [JsonPropertyName("stock_item")]
    public StockItem? StockItem { get; init; }
}