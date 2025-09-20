// <copyright file="SalesAgedDebtors.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record SalesAgedDebtors
{
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; init; }

    [JsonPropertyName("entries")]
    public List<AgedDebtorEntry>? Entries { get; init; }

    [JsonPropertyName("total_current")]
    public decimal? TotalCurrent { get; init; }

    [JsonPropertyName("total_overdue_1_to_30_days")]
    public decimal? TotalOverdue1To30Days { get; init; }

    [JsonPropertyName("total_overdue_31_to_60_days")]
    public decimal? TotalOverdue31To60Days { get; init; }

    [JsonPropertyName("total_overdue_61_to_90_days")]
    public decimal? TotalOverdue61To90Days { get; init; }

    [JsonPropertyName("total_overdue_over_90_days")]
    public decimal? TotalOverdueOver90Days { get; init; }

    [JsonPropertyName("total")]
    public decimal? Total { get; init; }
}