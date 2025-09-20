// <copyright file="AgedCreditorEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record AgedCreditorEntry
{
    [JsonPropertyName("contact")]
    public Uri? Contact { get; init; }

    [JsonPropertyName("contact_name")]
    public string? ContactName { get; init; }

    [JsonPropertyName("current")]
    public decimal? Current { get; init; }

    [JsonPropertyName("overdue_1_to_30_days")]
    public decimal? Overdue1To30Days { get; init; }

    [JsonPropertyName("overdue_31_to_60_days")]
    public decimal? Overdue31To60Days { get; init; }

    [JsonPropertyName("overdue_61_to_90_days")]
    public decimal? Overdue61To90Days { get; init; }

    [JsonPropertyName("overdue_over_90_days")]
    public decimal? OverdueOver90Days { get; init; }

    [JsonPropertyName("total")]
    public decimal? Total { get; init; }
}