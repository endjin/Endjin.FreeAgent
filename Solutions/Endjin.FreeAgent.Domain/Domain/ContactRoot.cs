// <copyright file="ContactRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;
public record ContactRoot
{
    [JsonPropertyName("contact")]
    public Contact? Contact { get; init; }
}