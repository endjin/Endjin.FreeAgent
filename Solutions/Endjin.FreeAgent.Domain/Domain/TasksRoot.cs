// <copyright file="TasksRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record TasksRoot
{
    [JsonPropertyName("tasks")]
    public List<TaskItem>? Tasks { get; init; }
}