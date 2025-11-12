// <copyright file="RecurringInvoice.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record RecurringInvoice
{
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    [JsonPropertyName("contact")]
    public Uri? Contact { get; init; }

    [JsonPropertyName("frequency")]
    public string? Frequency { get; init; }

    [JsonPropertyName("recurring_starts_on")]
    public DateOnly? RecurringStartsOn { get; init; }

    [JsonPropertyName("recurring_ends_on")]
    public DateOnly? RecurringEndsOn { get; init; }

    [JsonPropertyName("next_recurs_on")]
    public DateOnly? NextRecursOn { get; init; }

    [JsonPropertyName("profile_name")]
    public string? ProfileName { get; init; }

    [JsonPropertyName("reference")]
    public string? Reference { get; init; }

    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    [JsonPropertyName("net_value")]
    public decimal? NetValue { get; init; }

    [JsonPropertyName("total_value")]
    public decimal? TotalValue { get; init; }

    [JsonPropertyName("status")]
    public string? Status { get; init; }

    [JsonPropertyName("payment_terms_in_days")]
    public int? PaymentTermsInDays { get; init; }

    [JsonPropertyName("ec_status")]
    public string? EcStatus { get; init; }

    [JsonPropertyName("invoice_items")]
    public List<InvoiceItem>? InvoiceItems { get; init; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; init; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }
}