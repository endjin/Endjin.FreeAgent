// <copyright file="DepreciationProfile.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record DepreciationProfile
{
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    [JsonPropertyName("id")]
    public long? Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("method")]
    public string? Method { get; init; }

    [JsonPropertyName("period_years")]
    public int? PeriodYears { get; init; }

    [JsonPropertyName("residual_percentage")]
    public decimal? ResidualPercentage { get; init; }

    [JsonPropertyName("annual_percentage")]
    public decimal? AnnualPercentage { get; init; }
}