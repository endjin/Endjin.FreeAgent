// <copyright file="StockItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record StockItem
{
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("opening_quantity")]
    public decimal? OpeningQuantity { get; init; }

    [JsonPropertyName("opening_balance")]
    public decimal? OpeningBalance { get; init; }

    [JsonPropertyName("cost_of_sale_category")]
    public Uri? CostOfSaleCategory { get; init; }

    [JsonPropertyName("stock_on_hand_category")]
    public Uri? StockOnHandCategory { get; init; }

    [JsonPropertyName("stock_item_type")]
    public string? StockItemType { get; init; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; init; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }
}