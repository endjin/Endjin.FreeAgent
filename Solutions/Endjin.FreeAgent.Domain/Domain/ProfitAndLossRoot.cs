// <copyright file="ProfitAndLossRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record ProfitAndLossRoot
{
    [JsonPropertyName("profit_and_loss")]
    public ProfitAndLoss? ProfitAndLoss { get; init; }
}