// <copyright file="CreditNoteEmailRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record CreditNoteEmailRoot
{
    [JsonPropertyName("credit_note")]
    public CreditNoteEmailWrapper? CreditNote { get; init; }
}