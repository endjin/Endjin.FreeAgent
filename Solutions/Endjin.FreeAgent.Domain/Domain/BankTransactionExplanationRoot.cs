// <copyright file="BankTransactionExplanationRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record BankTransactionExplanationRoot
{
    [JsonPropertyName("bank_transaction_explanation")]
    public BankTransactionExplanation? BankTransactionExplanation { get; init; }
}