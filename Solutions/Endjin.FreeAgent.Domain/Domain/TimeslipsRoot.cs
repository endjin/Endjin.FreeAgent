// <copyright file="TimeslipsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record TimeslipsRoot
{
    [JsonPropertyName("timeslips")]
    public ImmutableList<Timeslip> Timeslips { get; init; } = [];
}