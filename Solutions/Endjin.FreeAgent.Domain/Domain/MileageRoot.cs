// <copyright file="MileageRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record MileageRoot
{
    [JsonPropertyName("mileage")]
    public Mileage? Mileage { get; init; }
}