// <copyright file="CapitalAsset.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a capital asset in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Capital assets are long-term tangible or intangible assets that are used in business operations and provide
/// economic value over multiple years. Examples include equipment, machinery, computers, furniture, vehicles,
/// and property. Each asset is tracked from purchase through disposal, with depreciation calculations and
/// tax allowance claims recorded.
/// </para>
/// <para>
/// Capital assets support UK tax features including Annual Investment Allowance (AIA), First Year Allowance (FYA),
/// and Super Deduction claims for corporation tax purposes. Assets are categorized by type and depreciated
/// over their useful life according to accounting policies.
/// </para>
/// <para>
/// When an asset is disposed of (sold or scrapped), the disposal proceeds and date are recorded to calculate
/// gains or losses for tax reporting.
/// </para>
/// <para>
/// API Endpoint: /v2/capital_assets
/// </para>
/// <para>
/// Minimum Access Level: Banking
/// </para>
/// </remarks>
/// <seealso cref="CapitalAssetType"/>
public record CapitalAsset
{
    /// <summary>
    /// Gets the unique URI identifier for this capital asset.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this capital asset in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the description or name of the capital asset.
    /// </summary>
    /// <value>
    /// A descriptive name for the asset, such as "Dell Laptop", "Office Furniture", or "Company Vehicle".
    /// This field is required when creating a capital asset.
    /// </value>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the date when the asset was purchased.
    /// </summary>
    /// <value>
    /// The purchase date in YYYY-MM-DD format. This field is required when creating a capital asset.
    /// </value>
    [JsonPropertyName("purchased_on")]
    public DateOnly? PurchasedOn { get; init; }

    /// <summary>
    /// Gets the date when the asset was disposed of (sold or scrapped).
    /// </summary>
    /// <value>
    /// The disposal date, or <see langword="null"/> if the asset is still in use.
    /// </value>
    [JsonPropertyName("disposed_on")]
    public DateOnly? DisposedOn { get; init; }

    /// <summary>
    /// Gets the asset type classification.
    /// </summary>
    /// <value>
    /// The asset type name such as "Computer Equipment", "Fixtures and Fittings",
    /// "Motor Vehicles", "Other Capital Asset", or a custom user-defined type.
    /// This field is required when creating a capital asset.
    /// </value>
    [JsonPropertyName("asset_type")]
    public string? AssetType { get; init; }

    /// <summary>
    /// Gets the expected useful life of the asset in years.
    /// </summary>
    /// <value>
    /// The number of years over which the asset will be depreciated for accounting purposes.
    /// </value>
    /// <remarks>
    /// This field is deprecated. Use <see cref="DepreciationProfile"/> instead for more flexible
    /// depreciation calculation options.
    /// </remarks>
    [Obsolete("This field is deprecated. Use DepreciationProfile instead.")]
    [JsonPropertyName("asset_life_years")]
    public int? AssetLifeYears { get; init; }

    /// <summary>
    /// Gets the depreciation settings defining how this asset's value decreases over time.
    /// </summary>
    /// <value>
    /// A <see cref="CapitalAssetDepreciationSettings"/> object specifying the depreciation method,
    /// rate, and frequency for calculating depreciation expense. Use this instead of the
    /// deprecated <see cref="AssetLifeYears"/> field for more sophisticated depreciation calculations.
    /// </value>
    [JsonPropertyName("depreciation_profile")]
    public CapitalAssetDepreciationSettings? DepreciationProfile { get; init; }

    /// <summary>
    /// Gets the date and time when this capital asset record was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the creation timestamp in UTC.
    /// </value>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this capital asset record was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the last modification timestamp in UTC.
    /// </value>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; init; }

    /// <summary>
    /// Gets the lifecycle event history for this capital asset.
    /// </summary>
    /// <value>
    /// An array of <see cref="CapitalAssetHistoryEvent"/> objects representing significant events
    /// in the asset's lifecycle including purchase, depreciation, tax allowance claims, and disposal.
    /// This property is only populated when the <c>include_history=true</c> query parameter is used.
    /// </value>
    [JsonPropertyName("capital_asset_history")]
    public CapitalAssetHistoryEvent[]? CapitalAssetHistory { get; init; }
}