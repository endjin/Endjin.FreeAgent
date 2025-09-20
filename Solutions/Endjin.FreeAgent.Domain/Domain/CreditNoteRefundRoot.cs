// <copyright file="CreditNoteRefundRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record CreditNoteRefundRoot
{
    [JsonPropertyName("credit_note")]
    public CreditNoteRefund? CreditNote { get; init; }
}