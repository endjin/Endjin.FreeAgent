// <copyright file="CreditNoteRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record CreditNoteRoot
{
    [JsonPropertyName("credit_note")]
    public CreditNote? CreditNote { get; init; }
}