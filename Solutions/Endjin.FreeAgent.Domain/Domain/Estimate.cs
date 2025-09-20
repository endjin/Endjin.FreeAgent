// <copyright file="Estimate.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

[DebuggerDisplay("Ref = {" + nameof(Reference) + "}, Status = {" + nameof(Status) + "}")]
public record Estimate
{
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    [JsonPropertyName("project")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Project { get; init; }

    [JsonIgnore]
    public Project? ProjectEntry { get; init; }

    [JsonPropertyName("contact")]
    public Uri? Contact { get; init; }

    [JsonIgnore]
    public Contact? ContactEntry { get; init; }

    [JsonPropertyName("invoice")]
    public Uri? Invoice { get; init; }

    [JsonPropertyName("reference")]
    public string? Reference { get; init; }

    [JsonPropertyName("discount_percent")]
    public decimal? DiscountPercent { get; init; }

    [JsonPropertyName("estimate_type")]
    public string? EstimateType { get; init; }

    [JsonPropertyName("dated_on")]
    public DateTimeOffset? DatedOn { get; init; }

    [JsonPropertyName("status")]
    public string? Status { get; init; }

    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    [JsonPropertyName("net_value")]
    public decimal? NetValue { get; init; }

    [JsonPropertyName("total_value")]
    public decimal? TotalValue { get; init; }

    [JsonPropertyName("involves_sales_tax")]
    public bool? InvolvesSalesTax { get; init; }

    [JsonPropertyName("is_interim_uk_vat")]
    public bool? IsInterimUkVat { get; init; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; init; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }

    [JsonPropertyName("sales_tax_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SalesTaxValue { get; init; }

    [JsonPropertyName("notes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Notes { get; init; }

    [JsonPropertyName("estimate_items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ImmutableArray<EstimateItem> EstimateItems { get; init; } = [];
}