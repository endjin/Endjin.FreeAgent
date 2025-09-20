// <copyright file="TaskRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public class TaskRoot
{
    [JsonPropertyName("task")]
    public TaskItem? TaskItem { get; set; }
}