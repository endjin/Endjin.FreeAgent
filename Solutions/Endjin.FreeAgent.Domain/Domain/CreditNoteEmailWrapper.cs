// <copyright file="CreditNoteEmailWrapper.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record CreditNoteEmailWrapper
{
    [JsonPropertyName("email")]
    public InvoiceEmail? Email { get; init; }
}