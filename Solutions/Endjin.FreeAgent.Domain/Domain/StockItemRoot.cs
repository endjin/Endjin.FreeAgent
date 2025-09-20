// <copyright file="StockItemRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record StockItemRoot
{
    [JsonPropertyName("stock_item")]
    public StockItem? StockItem { get; init; }
}