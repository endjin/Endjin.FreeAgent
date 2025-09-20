// <copyright file="CashFlowItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record CashFlowItem
{
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("value")]
    public decimal? Value { get; init; }

    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; init; }
}