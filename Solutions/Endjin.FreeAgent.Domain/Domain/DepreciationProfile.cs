// <copyright file="DepreciationProfile.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a depreciation profile for calculating capital asset depreciation in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Depreciation profiles define how the value of capital assets (fixed assets) is reduced over time to reflect
/// wear and tear, obsolescence, and declining usefulness. They specify the calculation method, rate, and period
/// for systematic depreciation expense recognition.
/// </para>
/// <para>
/// Common depreciation methods supported:
/// - Straight Line: Equal depreciation expense each year over the asset's useful life
/// - Declining Balance: Higher depreciation in early years, reducing over time
/// - Sum of Years Digits: Accelerated depreciation weighted toward earlier years
/// </para>
/// <para>
/// Depreciation profiles ensure consistent treatment of similar asset types (e.g., "Computer Equipment - 3 years straight line"
/// or "Vehicles - 25% declining balance") and automate the calculation of depreciation expenses for financial reporting
/// and tax purposes.
/// </para>
/// <para>
/// API Endpoint: /v2/depreciation_profiles
/// </para>
/// <para>
/// Minimum Access Level: Standard
/// </para>
/// </remarks>
/// <seealso cref="CapitalAsset"/>
/// <seealso cref="CapitalAssetType"/>
public record DepreciationProfile
{
    /// <summary>
    /// Gets the unique URI identifier for this depreciation profile.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this depreciation profile in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the numeric identifier for this depreciation profile.
    /// </summary>
    /// <value>
    /// A unique numeric ID for this profile.
    /// </value>
    [JsonPropertyName("id")]
    public long? Id { get; init; }

    /// <summary>
    /// Gets the name of this depreciation profile.
    /// </summary>
    /// <value>
    /// A descriptive name such as "Computer Equipment - 3 Year Straight Line" or "Vehicles - 25% Declining Balance".
    /// </value>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Gets the depreciation calculation method.
    /// </summary>
    /// <value>
    /// The method name such as "Straight Line", "Declining Balance", or "Sum of Years Digits"
    /// that determines how depreciation is calculated.
    /// </value>
    [JsonPropertyName("method")]
    public string? Method { get; init; }

    /// <summary>
    /// Gets the depreciation period in years.
    /// </summary>
    /// <value>
    /// The useful life of assets using this profile, in years. For example, 3 for computers, 5 for vehicles,
    /// or 10 for furniture. This determines how long depreciation is spread over.
    /// </value>
    [JsonPropertyName("period_years")]
    public int? PeriodYears { get; init; }

    /// <summary>
    /// Gets the residual value percentage.
    /// </summary>
    /// <value>
    /// The estimated percentage of the asset's original value that will remain at the end of its useful life.
    /// For example, 10% means the asset retains 10% of its purchase price as salvage value.
    /// </value>
    [JsonPropertyName("residual_percentage")]
    public decimal? ResidualPercentage { get; init; }

    /// <summary>
    /// Gets the annual depreciation percentage for declining balance methods.
    /// </summary>
    /// <value>
    /// The percentage of the asset's current value to depreciate each year when using declining balance method.
    /// For example, 25% means 25% of the remaining book value is depreciated annually.
    /// </value>
    [JsonPropertyName("annual_percentage")]
    public decimal? AnnualPercentage { get; init; }
}