// <copyright file="BankTransactionsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record BankTransactionsRoot
{
    [JsonPropertyName("bank_transactions")]
    public List<BankTransaction> BankTransactions { get; init; } = [];
}