// <copyright file="CorporationTaxReturn.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record CorporationTaxReturn
{
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    [JsonPropertyName("period_starts_on")]
    public DateOnly? PeriodStartsOn { get; init; }

    [JsonPropertyName("period_ends_on")]
    public DateOnly? PeriodEndsOn { get; init; }

    [JsonPropertyName("status")]
    public string? Status { get; init; }

    [JsonPropertyName("tax_due")]
    public decimal? TaxDue { get; init; }

    [JsonPropertyName("filed_on")]
    public DateOnly? FiledOn { get; init; }

    [JsonPropertyName("filed_online")]
    public bool? FiledOnline { get; init; }

    [JsonPropertyName("hmrc_reference")]
    public string? HmrcReference { get; init; }

    [JsonPropertyName("payment_due_on")]
    public DateOnly? PaymentDueOn { get; init; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; init; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }
}