// <copyright file="EstimatesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record EstimatesRoot
{
    [JsonPropertyName("estimates")]
    public List<Estimate>? Estimates { get; init; }
}