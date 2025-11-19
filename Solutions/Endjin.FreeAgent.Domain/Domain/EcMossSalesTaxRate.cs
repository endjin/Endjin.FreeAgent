// <copyright file="EcMossSalesTaxRate.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents an EU VAT rate for the Mini One Stop Shop (MOSS) scheme in FreeAgent.
/// </summary>
/// <remarks>
/// <para>
/// EC MOSS (Mini One Stop Shop) sales tax rates are used for cross-border digital services
/// within the European Union. These rates vary by member state and tax band (e.g., Standard,
/// Reduced, Parking).
/// </para>
/// <para>
/// The rates returned are specific to a country and date combination, allowing businesses
/// to apply the correct VAT rates for digital services sold to consumers in different EU
/// member states.
/// </para>
/// <para>
/// API Endpoint: GET /v2/ec_moss/sales_tax_rates?country={country}&amp;date={date}
/// </para>
/// <para>
/// Minimum Access Level: Read Only
/// </para>
/// </remarks>
/// <seealso cref="SalesTaxRate"/>
public record EcMossSalesTaxRate
{
    /// <summary>
    /// Gets the tax rate percentage.
    /// </summary>
    /// <value>
    /// The VAT rate as a decimal value. For example, 20% is represented as 20.0, 13% as 13.0.
    /// </value>
    [JsonPropertyName("percentage")]
    public required decimal Percentage { get; init; }

    /// <summary>
    /// Gets the tax band classification for this rate.
    /// </summary>
    /// <value>
    /// The classification of the tax rate, such as "Standard", "Reduced", or "Parking".
    /// Different EU member states have different bands with varying rates.
    /// </value>
    [JsonPropertyName("band")]
    public required string Band { get; init; }
}
