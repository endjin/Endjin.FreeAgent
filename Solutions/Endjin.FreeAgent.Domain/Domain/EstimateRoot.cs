// <copyright file="EstimateRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public class EstimateRoot
{
    [JsonPropertyName("estimate")]
    public Estimate? Estimate { get; set; }
}