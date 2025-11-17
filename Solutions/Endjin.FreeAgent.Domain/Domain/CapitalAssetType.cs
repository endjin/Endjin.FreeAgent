// <copyright file="CapitalAssetType.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a capital asset type classification in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Capital asset types define categories for fixed assets. FreeAgent provides several system default types
/// (Computer Equipment, Fixtures and Fittings, Motor Vehicles, Other Capital Asset), and users can create
/// custom types to categorize their capital assets.
/// </para>
/// <para>
/// API Endpoint: /v2/capital_asset_types
/// </para>
/// <para>
/// Minimum Access Level: Full Access
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
    /// Gets the name of this capital asset type.
    /// </summary>
    /// <value>
    /// A custom or system-defined name such as "Computer Equipment", "Fixtures and Fittings",
    /// "Motor Vehicles", or "Other Capital Asset".
    /// </value>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Gets a value indicating whether this is a system default capital asset type.
    /// </summary>
    /// <value>
    /// <c>true</c> if this is a system default capital asset type;
    /// <c>false</c> if it was created by a user.
    /// </value>
    [JsonPropertyName("system_default")]
    public bool? SystemDefault { get; init; }

    /// <summary>
    /// Gets the timestamp when this capital asset type was created.
    /// </summary>
    /// <value>
    /// The creation timestamp in UTC.
    /// </value>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>
    /// Gets the timestamp when this capital asset type was last updated.
    /// </summary>
    /// <value>
    /// The last modification timestamp in UTC.
    /// </value>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; init; }
}