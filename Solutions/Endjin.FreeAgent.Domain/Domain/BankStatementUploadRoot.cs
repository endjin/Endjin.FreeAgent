// <copyright file="BankStatementUploadRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record BankStatementUploadRoot
{
    [JsonPropertyName("statement")]
    public BankStatementUpload? Statement { get; init; }
}