// <copyright file="MileagesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record MileagesRoot
{
    [JsonPropertyName("mileages")]
    public List<Mileage>? Mileages { get; init; }
}