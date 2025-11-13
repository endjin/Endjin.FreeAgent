// <copyright file="CapitalAssetType.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a capital asset type classification in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Capital asset types define categories for different kinds of capital assets and specify the tax treatment,
/// depreciation rates, and capital allowance rules that apply to assets of that type. Each type determines
/// how the asset is depreciated for accounting purposes and what tax relief can be claimed.
/// </para>
/// <para>
/// Common capital asset types include Plant & Machinery, Fixtures & Fittings, Motor Vehicles, and Computer Equipment.
/// Each type has specific capital allowance rates and treatment under tax regulations (e.g., UK Capital Allowances Act).
/// </para>
/// <para>
/// API Endpoint: /v2/capital_asset_types
/// </para>
/// <para>
/// Minimum Access Level: Assets
/// </para>
/// </remarks>
/// <seealso cref="CapitalAsset"/>
public record CapitalAssetType
{
    /// <summary>
    /// Gets the unique URI identifier for this capital asset type.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this capital asset type in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the description or name of this capital asset type.
    /// </summary>
    /// <value>
    /// A descriptive name such as "Plant & Machinery", "Fixtures & Fittings", or "Motor Vehicles".
    /// </value>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the type of capital allowance applicable to this asset type.
    /// </summary>
    /// <value>
    /// The allowance classification such as "Main Rate Pool", "Special Rate Pool", or "Annual Investment Allowance",
    /// determining how tax relief is calculated for assets of this type.
    /// </value>
    [JsonPropertyName("allowance_type")]
    public string? AllowanceType { get; init; }

    /// <summary>
    /// Gets the depreciation or capital allowance rate for this asset type.
    /// </summary>
    /// <value>
    /// The annual rate as a decimal (e.g., 0.18 for 18% writing down allowance) used to calculate
    /// depreciation and tax relief for assets of this type.
    /// </value>
    [JsonPropertyName("rate")]
    public decimal? Rate { get; init; }
}