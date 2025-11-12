// <copyright file="StockItemsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record StockItemsRoot
{
    [JsonPropertyName("stock_items")]
    public List<StockItem>? StockItems { get; init; }
}