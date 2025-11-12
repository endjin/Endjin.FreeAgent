// <copyright file="TaskRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record TaskRoot
{
    [JsonPropertyName("task")]
    public TaskItem? TaskItem { get; init; }
}