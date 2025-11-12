// <copyright file="ProjectsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record ProjectsRoot
{
    [JsonPropertyName("projects")]
    public ImmutableList<Project> Projects { get; init; } = [];
}