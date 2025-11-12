// <copyright file="OpeningBalance.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record OpeningBalance
{
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    [JsonPropertyName("journal")]
    public OpeningBalanceJournal? Journal { get; init; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; init; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }
}