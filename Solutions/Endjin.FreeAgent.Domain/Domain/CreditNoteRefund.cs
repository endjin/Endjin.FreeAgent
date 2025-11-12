// <copyright file="CreditNoteRefund.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record CreditNoteRefund
{
    [JsonPropertyName("refunded_on")]
    public string? RefundedOn { get; init; }

    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; init; }
}