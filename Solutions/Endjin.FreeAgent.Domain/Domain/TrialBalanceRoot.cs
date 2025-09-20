// <copyright file="TrialBalanceRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record TrialBalanceRoot
{
    [JsonPropertyName("trial_balance")]
    public TrialBalance? TrialBalance { get; init; }
}