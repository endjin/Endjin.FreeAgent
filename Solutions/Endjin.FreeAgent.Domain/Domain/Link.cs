// <copyright file="Link.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

using System;

public record Link
{
    public required string Rel { get; init; }

    public required Uri Uri { get; init; }
}