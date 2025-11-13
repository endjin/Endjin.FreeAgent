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
/// Minimum Access Level: Assets
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
    /// Gets the URI reference to the capital asset type classification.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.CapitalAssetType"/> defining the category and depreciation rules
    /// for this asset. This field is required when creating a capital asset.
    /// </value>
    [JsonPropertyName("capital_asset_type")]
    public Uri? CapitalAssetType { get; init; }

    /// <summary>
    /// Gets the expected useful life of the asset in years.
    /// </summary>
    /// <value>
    /// The number of years over which the asset will be depreciated for accounting purposes.
    /// </value>
    [JsonPropertyName("asset_life_years")]
    public int? AssetLifeYears { get; init; }

    /// <summary>
    /// Gets the purchase price of the asset.
    /// </summary>
    /// <value>
    /// The original cost paid to acquire the asset. This field is required when creating a capital asset.
    /// </value>
    [JsonPropertyName("purchase_price")]
    public decimal? PurchasePrice { get; init; }

    /// <summary>
    /// Gets the proceeds received from disposing of the asset.
    /// </summary>
    /// <value>
    /// The amount received when the asset was sold or the scrap value received.
    /// Used to calculate gains or losses on disposal for tax purposes.
    /// </value>
    [JsonPropertyName("disposal_proceeds")]
    public decimal? DisposalProceeds { get; init; }

    /// <summary>
    /// Gets the amount of Annual Investment Allowance claimed for this asset.
    /// </summary>
    /// <value>
    /// The value of AIA tax relief claimed against corporation tax for this asset purchase.
    /// AIA allows businesses to deduct the full value of qualifying assets up to an annual limit.
    /// </value>
    [JsonPropertyName("annual_investment_allowance_claimed")]
    public decimal? AnnualInvestmentAllowanceClaimed { get; init; }

    /// <summary>
    /// Gets the amount of First Year Allowance claimed for this asset.
    /// </summary>
    /// <value>
    /// The value of FYA tax relief claimed for qualifying plant and machinery.
    /// First Year Allowances provide accelerated tax relief for certain types of assets.
    /// </value>
    [JsonPropertyName("first_year_allowance_claimed")]
    public decimal? FirstYearAllowanceClaimed { get; init; }

    /// <summary>
    /// Gets the amount of Super Deduction claimed for this asset.
    /// </summary>
    /// <value>
    /// The enhanced capital allowance claim (130% for main rate assets or 50% for special rate assets)
    /// available under the UK Super Deduction scheme for qualifying investments.
    /// </value>
    [JsonPropertyName("super_deduction_claimed")]
    public decimal? SuperDeductionClaimed { get; init; }

    /// <summary>
    /// Gets the current residual value of the asset after depreciation.
    /// </summary>
    /// <value>
    /// The book value remaining after accumulated depreciation. This value decreases over
    /// the <see cref="AssetLifeYears"/> until fully depreciated.
    /// </value>
    [JsonPropertyName("residual_value")]
    public decimal? ResidualValue { get; init; }

    /// <summary>
    /// Gets the date and time when this capital asset record was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing the creation timestamp in UTC.
    /// </value>
    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this capital asset record was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing the last modification timestamp in UTC.
    /// </value>
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }
}