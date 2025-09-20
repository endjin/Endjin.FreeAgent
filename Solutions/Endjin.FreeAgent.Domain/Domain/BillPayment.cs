// <copyright file="BillPayment.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record BillPayment
{
    [JsonPropertyName("paid_on")]
    public string? PaidOn { get; init; }

    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; init; }
}