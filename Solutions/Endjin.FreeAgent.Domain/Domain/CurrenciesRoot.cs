// <copyright file="CurrenciesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record CurrenciesRoot
{
    [JsonPropertyName("currencies")]
    public List<Currency> Currencies { get; init; } = [];
}