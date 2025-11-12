// <copyright file="OpeningBalanceJournal.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record OpeningBalanceJournal
{
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("debit_entries")]
    public List<OpeningBalanceEntry>? DebitEntries { get; init; }

    [JsonPropertyName("credit_entries")]
    public List<OpeningBalanceEntry>? CreditEntries { get; init; }
}