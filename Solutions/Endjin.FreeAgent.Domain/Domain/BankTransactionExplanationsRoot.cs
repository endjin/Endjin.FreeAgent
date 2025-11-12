// <copyright file="BankTransactionExplanationsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record BankTransactionExplanationsRoot
{
    [JsonPropertyName("bank_transaction_explanations")]
    public List<BankTransactionExplanation> BankTransactionExplanations { get; init; } = [];
}