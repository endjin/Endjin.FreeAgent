// <copyright file="BankStatementUploadResponse.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record BankStatementUploadResponse
{
    [JsonPropertyName("imported_transaction_count")]
    public int? ImportedTransactionCount { get; init; }

    [JsonPropertyName("duplicate_transaction_count")]
    public int? DuplicateTransactionCount { get; init; }

    [JsonPropertyName("ignored_transaction_count")]
    public int? IgnoredTransactionCount { get; init; }

    [JsonPropertyName("bank_transactions")]
    public List<BankTransaction>? BankTransactions { get; init; }

    [JsonPropertyName("errors")]
    public List<string>? Errors { get; init; }
}