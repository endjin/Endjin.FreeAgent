// <copyright file="CompanyRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record CompanyRoot
{
    [JsonPropertyName("company")]
    public Company? Company { get; init; }
}