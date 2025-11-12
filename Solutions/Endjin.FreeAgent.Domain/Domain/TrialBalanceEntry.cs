// <copyright file="TrialBalanceEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record TrialBalanceEntry
{
    [JsonPropertyName("category_url")]
    public Uri? CategoryUrl { get; init; }

    [JsonPropertyName("category_description")]
    public string? CategoryDescription { get; init; }

    [JsonPropertyName("nominal_code")]
    public string? NominalCode { get; init; }

    [JsonPropertyName("debit")]
    public decimal? Debit { get; init; }

    [JsonPropertyName("credit")]
    public decimal? Credit { get; init; }

    [JsonPropertyName("balance")]
    public decimal? Balance { get; init; }
}