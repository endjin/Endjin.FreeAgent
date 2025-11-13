// <copyright file="StockItemsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="StockItem"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple stock items.
/// </remarks>
/// <seealso cref="StockItem"/>
public record StockItemsRoot
{
    /// <summary>
    /// Gets the collection of stock items from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="StockItem"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("stock_items")]
    public List<StockItem>? StockItems { get; init; }
}