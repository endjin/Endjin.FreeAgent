// <copyright file="OpeningBalanceRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record OpeningBalanceRoot
{
    [JsonPropertyName("opening_balance")]
    public OpeningBalance? OpeningBalance { get; init; }
}