// <copyright file="BankAccountRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

using System.Text.Json.Serialization;

public record BankAccountRoot
{
    [JsonPropertyName("bank_account")]
    public BankAccount? BankAccount { get; init; }
}