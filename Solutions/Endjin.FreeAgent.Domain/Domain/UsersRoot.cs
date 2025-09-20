// <copyright file="UsersRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record UsersRoot
{
    [JsonPropertyName("users")]
    public ImmutableList<User> Users { get; init; } = [];
}