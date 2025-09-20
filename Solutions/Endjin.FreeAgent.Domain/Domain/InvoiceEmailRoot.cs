// <copyright file="InvoiceEmailRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record InvoiceEmailRoot
{
    [JsonPropertyName("invoice")]
    public InvoiceEmailWrapper? Invoice { get; init; }
}