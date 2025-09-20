// <copyright file="OpeningBalanceEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record OpeningBalanceEntry
{
    [JsonPropertyName("category")]
    public Uri? Category { get; init; }

    [JsonPropertyName("amount")]
    public decimal? Amount { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }
}