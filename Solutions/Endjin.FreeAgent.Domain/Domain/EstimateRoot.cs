// <copyright file="EstimateRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record EstimateRoot
{
    [JsonPropertyName("estimate")]
    public Estimate? Estimate { get; init; }
}