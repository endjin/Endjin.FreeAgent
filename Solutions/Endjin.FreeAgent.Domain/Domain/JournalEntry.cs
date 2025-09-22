// <copyright file="JournalEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

[DebuggerDisplay("{Category} {DisplayText} - {DebitValue}")]
public record JournalEntry
{
    [JsonPropertyName("category")]
    public required string Category { get; init; }

    [JsonPropertyName("debit_value")]
    public required string DebitValue { get; init; }

    [JsonPropertyName("description")]
    public required string Description { get; init; }

    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Url { get; init; }

    [JsonPropertyName("user")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? User { get; init; }

    [JsonIgnore]
    public string? DisplayText { get; init; }
}