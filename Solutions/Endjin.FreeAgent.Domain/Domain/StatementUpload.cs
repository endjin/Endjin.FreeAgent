// <copyright file="StatementUpload.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record StatementUpload
{
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; init; }

    [JsonPropertyName("statement")]
    public string? Statement { get; init; }

    [JsonPropertyName("file_type")]
    public string? FileType { get; init; }
}