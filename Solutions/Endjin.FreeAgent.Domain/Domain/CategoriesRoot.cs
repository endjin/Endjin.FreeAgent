// <copyright file="CategoriesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record CategoriesRoot
{
    [JsonPropertyName("categories")]
    public List<Category> Categories { get; init; } = [];
}