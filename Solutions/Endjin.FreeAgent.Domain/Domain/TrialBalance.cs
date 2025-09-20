// <copyright file="TrialBalance.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record TrialBalance
{
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; init; }

    [JsonPropertyName("entries")]
    public List<TrialBalanceEntry>? Entries { get; init; }

    [JsonPropertyName("total_debit")]
    public decimal? TotalDebit { get; init; }

    [JsonPropertyName("total_credit")]
    public decimal? TotalCredit { get; init; }
}