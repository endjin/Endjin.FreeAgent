// <copyright file="ProjectRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record ProjectRoot
{
    [JsonPropertyName("project")]
    public Project? Project { get; init; }
}