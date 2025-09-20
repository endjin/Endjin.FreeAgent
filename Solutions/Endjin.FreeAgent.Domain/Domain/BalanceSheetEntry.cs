// <copyright file="BalanceSheetEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record BalanceSheetEntry
{
    [JsonPropertyName("category_url")]
    public Uri? CategoryUrl { get; init; }

    [JsonPropertyName("category_description")]
    public string? CategoryDescription { get; init; }

    [JsonPropertyName("nominal_code")]
    public string? NominalCode { get; init; }

    [JsonPropertyName("value")]
    public decimal? Value { get; init; }
}