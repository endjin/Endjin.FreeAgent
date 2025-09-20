// <copyright file="BillsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record BillsRoot
{
    [JsonPropertyName("bills")]
    public List<Bill> Bills { get; init; } = [];
}