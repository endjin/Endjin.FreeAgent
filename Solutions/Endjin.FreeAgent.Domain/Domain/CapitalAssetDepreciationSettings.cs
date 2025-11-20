// <copyright file="CapitalAssetDepreciationSettings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the inline depreciation settings for a capital asset.
/// </summary>
/// <remarks>
/// <para>
/// This inline structure defines how a specific capital asset's value decreases over time for
/// accounting purposes. It specifies the calculation method, useful life, and posting frequency
/// for depreciation expense recognition.
/// </para>
/// <para>
/// Supported methods:
/// - straight_line: Equal depreciation expense each year over the asset's useful life
/// - reducing_balance: Percentage-based depreciation applied to the remaining book value
/// - no_depreciation: No depreciation calculations performed
/// </para>
/// <para>
/// This is an inline structure embedded in capital asset responses, distinct from the
/// DepreciationProfile entity available at the /v2/depreciation_profiles endpoint.
/// </para>
/// </remarks>
/// <seealso cref="CapitalAsset"/>
/// <seealso cref="DepreciationProfile"/>
public record CapitalAssetDepreciationSettings
{
    /// <summary>
    /// Gets the depreciation calculation method.
    /// </summary>
    /// <value>
    /// One of: "straight_line", "reducing_balance", or "no_depreciation".
    /// </value>
    [JsonPropertyName("method")]
    public string? Method { get; init; }

    /// <summary>
    /// Gets the asset's useful life in years.
    /// </summary>
    /// <value>
    /// The number of years over which the asset will be depreciated (2-25 years).
    /// Required when <see cref="Method"/> is "straight_line".
    /// </value>
    [JsonPropertyName("asset_life_years")]
    public int? AssetLifeYears { get; init; }

    /// <summary>
    /// Gets the annual depreciation percentage for reducing balance method.
    /// </summary>
    /// <value>
    /// The percentage of the asset's current book value to depreciate each year (1-99%).
    /// Required when <see cref="Method"/> is "reducing_balance".
    /// </value>
    [JsonPropertyName("annual_depreciation_percentage")]
    public int? AnnualDepreciationPercentage { get; init; }

    /// <summary>
    /// Gets the frequency at which depreciation is posted to ledgers.
    /// </summary>
    /// <value>
    /// One of: "monthly" or "annually". Defaults to "monthly" if not specified.
    /// </value>
    [JsonPropertyName("frequency")]
    public string? Frequency { get; init; }
}
