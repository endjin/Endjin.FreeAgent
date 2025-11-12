// <copyright file="BankStatementUpload.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record BankStatementUpload
{
    [JsonPropertyName("bank_account")]
    public Uri? BankAccount { get; init; }

    [JsonPropertyName("statement")]
    public string? Statement { get; init; }

    [JsonPropertyName("file_type")]
    public string? FileType { get; init; }
}