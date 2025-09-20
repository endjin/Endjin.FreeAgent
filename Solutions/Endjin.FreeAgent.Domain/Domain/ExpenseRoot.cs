// <copyright file="ExpenseRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public class ExpenseRoot
{
    [JsonPropertyName("expense")]
    public Expense? Expense { get; set; }
}