// <copyright file="BankAccountsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record BankAccountsRoot
{
    [JsonPropertyName("bank_accounts")]
    public List<BankAccount> BankAccounts { get; init; } = [];
}