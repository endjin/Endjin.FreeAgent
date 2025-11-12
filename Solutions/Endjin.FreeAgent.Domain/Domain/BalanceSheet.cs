// <copyright file="BalanceSheet.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record BalanceSheet
{
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; init; }

    [JsonPropertyName("fixed_assets")]
    public decimal? FixedAssets { get; init; }

    [JsonPropertyName("current_assets")]
    public decimal? CurrentAssets { get; init; }

    [JsonPropertyName("current_liabilities")]
    public decimal? CurrentLiabilities { get; init; }

    [JsonPropertyName("net_current_assets")]
    public decimal? NetCurrentAssets { get; init; }

    [JsonPropertyName("total_assets_less_current_liabilities")]
    public decimal? TotalAssetsLessCurrentLiabilities { get; init; }

    [JsonPropertyName("capital_and_reserves")]
    public decimal? CapitalAndReserves { get; init; }

    [JsonPropertyName("fixed_asset_entries")]
    public List<BalanceSheetEntry>? FixedAssetEntries { get; init; }

    [JsonPropertyName("current_asset_entries")]
    public List<BalanceSheetEntry>? CurrentAssetEntries { get; init; }

    [JsonPropertyName("current_liability_entries")]
    public List<BalanceSheetEntry>? CurrentLiabilityEntries { get; init; }

    [JsonPropertyName("capital_and_reserve_entries")]
    public List<BalanceSheetEntry>? CapitalAndReserveEntries { get; init; }
}