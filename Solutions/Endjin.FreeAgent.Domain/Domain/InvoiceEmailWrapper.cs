// <copyright file="InvoiceEmailWrapper.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record InvoiceEmailWrapper
{
    [JsonPropertyName("email")]
    public InvoiceEmail? Email { get; init; }
}