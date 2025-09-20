// <copyright file="ExpenseRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record ExpenseRoot
{
    [JsonPropertyName("expense")]
    public Expense? Expense { get; init; }
}