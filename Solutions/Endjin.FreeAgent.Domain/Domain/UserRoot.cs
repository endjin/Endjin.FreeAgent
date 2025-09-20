// <copyright file="UserRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

using System.Text.Json.Serialization;

public partial record UserRoot
{
    [JsonPropertyName("user")]
    public User? User { get; init; }
}