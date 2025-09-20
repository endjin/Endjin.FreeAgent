// <copyright file="InvoicePayment.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record InvoicePayment
{
    [JsonPropertyName("paid_on")]
    public string? PaidOn { get; init; }

    [JsonPropertyName("paid_into_bank_account")]
    public string? PaidIntoBankAccount { get; init; }
}