// <copyright file="CapitalAsset.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record CapitalAsset
{
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("purchased_on")]
    public DateOnly? PurchasedOn { get; init; }

    [JsonPropertyName("disposed_on")]
    public DateOnly? DisposedOn { get; init; }

    [JsonPropertyName("capital_asset_type")]
    public Uri? CapitalAssetType { get; init; }

    [JsonPropertyName("asset_life_years")]
    public int? AssetLifeYears { get; init; }

    [JsonPropertyName("purchase_price")]
    public decimal? PurchasePrice { get; init; }

    [JsonPropertyName("disposal_proceeds")]
    public decimal? DisposalProceeds { get; init; }

    [JsonPropertyName("annual_investment_allowance_claimed")]
    public decimal? AnnualInvestmentAllowanceClaimed { get; init; }

    [JsonPropertyName("first_year_allowance_claimed")]
    public decimal? FirstYearAllowanceClaimed { get; init; }

    [JsonPropertyName("super_deduction_claimed")]
    public decimal? SuperDeductionClaimed { get; init; }

    [JsonPropertyName("residual_value")]
    public decimal? ResidualValue { get; init; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; init; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }
}