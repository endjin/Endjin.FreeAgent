// <copyright file="ProfitAndLossEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record ProfitAndLossEntry
{
    [JsonPropertyName("category_url")]
    public Uri? CategoryUrl { get; init; }

    [JsonPropertyName("category_description")]
    public string? CategoryDescription { get; init; }

    [JsonPropertyName("nominal_code")]
    public string? NominalCode { get; init; }

    [JsonPropertyName("value")]
    public decimal? Value { get; init; }

    [JsonPropertyName("percentage_of_turnover")]
    public decimal? PercentageOfTurnover { get; init; }
}