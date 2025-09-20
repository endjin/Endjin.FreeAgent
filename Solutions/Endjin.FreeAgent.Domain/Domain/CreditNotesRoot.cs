// <copyright file="CreditNotesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record CreditNotesRoot
{
    [JsonPropertyName("credit_notes")]
    public List<CreditNote> CreditNotes { get; init; } = [];
}