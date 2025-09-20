// <copyright file="InvoicesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record InvoicesRoot
{
    [JsonPropertyName("invoices")]
    public List<Invoice> Invoices { get; init; } = [];
}