// <copyright file="EstimateItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

[DebuggerDisplay("{" + nameof(Description) + "}, {" + nameof(Price) + "}")]
public record EstimateItem
{
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    [JsonPropertyName("position")]
    public int? Position { get; init; }

    /// <summary>
    /// Hours, Days, Weeks, Months, Years
    /// </summary>
    [JsonPropertyName("item_type")]
    public string? ItemType { get; init; }

    [JsonPropertyName("quantity")]
    public decimal? Quantity { get; init; }

    [JsonPropertyName("price")]
    public decimal? Price { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("sales_tax_value")]
    public decimal? SalesTaxValue { get; init; }

    [JsonPropertyName("sales_tax_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SalesTaxRate { get; init; }

    [JsonPropertyName("category")]
    public Uri? Category { get; init; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; init; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }
}