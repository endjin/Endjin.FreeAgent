// <copyright file="BankTransactionRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record BankTransactionRoot
{
    [JsonPropertyName("bank_transaction")]
    public BankTransaction? BankTransaction { get; init; }
}