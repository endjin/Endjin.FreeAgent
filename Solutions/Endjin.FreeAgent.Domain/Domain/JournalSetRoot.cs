// <copyright file="JournalSetRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

using System.Text.Json.Serialization;

public record JournalSetRoot
{
    [JsonPropertyName("journal_set")]
    public JournalSet? JournalSet { get; init; }
}