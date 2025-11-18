// <copyright file="HirePurchase.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a hire purchase record in the FreeAgent system.
/// </summary>
/// <remarks>
/// <para>
/// Hire purchases are financing arrangements for purchasing assets. This entity represents
/// the liability tracking for bills that are paid through hire purchase financing. The API
/// provides read-only access to these records, which are automatically created and managed
/// through the Bills API.
/// </para>
/// <para>
/// All properties in this record are read-only. Hire purchase records are created when a bill
/// is set up with <see cref="Bill.IsPaidByHirePurchase"/> set to <see langword="true"/>, and
/// are removed when that bill is deleted or the hire purchase flag is cleared.
/// </para>
/// <para>
/// Each hire purchase tracks two liability categories: one for portions due within one year
/// and another for portions due over one year. These categories are used for proper balance
/// sheet reporting and financial analysis.
/// </para>
/// <para>
/// API Endpoint: /v2/hire_purchases
/// </para>
/// <para>
/// Minimum Access Level: Bills
/// </para>
/// <para>
/// This endpoint is only available for UK companies.
/// </para>
/// </remarks>
/// <seealso cref="Bill"/>
/// <seealso cref="Category"/>
public record HirePurchase
{
    /// <summary>
    /// Gets the unique URI identifier for this hire purchase.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this hire purchase in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the free-text description of the hire purchase.
    /// </summary>
    /// <value>
    /// The description taken from the associated bill.
    /// </value>
    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the URI reference to the associated bill.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Bill"/> that created this hire purchase record.
    /// </value>
    [JsonPropertyName("bill")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Bill { get; init; }

    /// <summary>
    /// Gets the URI of the category for liabilities over one year.
    /// </summary>
    /// <value>
    /// The <see cref="Category"/> URI for the portion of the hire purchase liability exceeding one year.
    /// </value>
    [JsonPropertyName("liabilities_over_one_year_category")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? LiabilitiesOverOneYearCategory { get; init; }

    /// <summary>
    /// Gets the URI of the category for liabilities under one year.
    /// </summary>
    /// <value>
    /// The <see cref="Category"/> URI for the portion of the hire purchase liability within one year.
    /// </value>
    [JsonPropertyName("liabilities_under_one_year_category")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? LiabilitiesUnderOneYearCategory { get; init; }
}
