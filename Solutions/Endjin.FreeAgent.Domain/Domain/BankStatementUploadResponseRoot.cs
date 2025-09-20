// <copyright file="BankStatementUploadResponseRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record BankStatementUploadResponseRoot
{
    [JsonPropertyName("import_summary")]
    public BankStatementUploadResponse? ImportSummary { get; init; }
}