// <copyright file="SalesTaxRate.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a sales tax (VAT) rate configuration in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Sales tax rates define the applicable VAT percentages for different time periods, supporting the UK's
/// changing VAT rates over time. The system maintains historical rates to ensure transactions are taxed
/// correctly based on when they occurred.
/// </para>
/// <para>
/// In the UK, common VAT rates include:
/// - Standard Rate: Currently 20% for most goods and services
/// - Reduced Rate: Currently 5% for certain goods (e.g., children's car seats, home energy)
/// - Zero Rate: 0% for exempt items (e.g., most food, books, children's clothes)
/// - Historical rates: Previous rates like the temporary 15% (2008-2010) and 17.5% (pre-2011)
/// </para>
/// <para>
/// Each rate has validity dates that determine when it applies, allowing FreeAgent to automatically
/// select the correct tax rate based on invoice and bill dates.
/// </para>
/// <para>
/// API Endpoint: /v2/sales_tax_rates
/// </para>
/// <para>
/// Minimum Access Level: Read Only
/// </para>
/// </remarks>
/// <seealso cref="Invoice"/>
/// <seealso cref="Bill"/>
/// <seealso cref="VatReturn"/>
public record SalesTaxRate
{
    /// <summary>
    /// Gets the tax rate percentage.
    /// </summary>
    /// <value>
    /// The VAT rate as a decimal value. For example, 20% is represented as 20.0, 5% as 5.0, and 0% as 0.0.
    /// </value>
    [JsonPropertyName("rate")]
    public decimal Rate { get; init; }

    /// <summary>
    /// Gets the description of this tax rate.
    /// </summary>
    /// <value>
    /// A descriptive name such as "Standard Rate", "Reduced Rate", "Zero Rate", or historical descriptions
    /// like "Standard Rate (2011 onwards)".
    /// </value>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the date from which this tax rate became valid.
    /// </summary>
    /// <value>
    /// The first date on which this tax rate applies to transactions. Invoices and bills dated on or
    /// after this date use this rate.
    /// </value>
    [JsonPropertyName("valid_from")]
    public DateOnly? ValidFrom { get; init; }

    /// <summary>
    /// Gets the date until which this tax rate remains valid.
    /// </summary>
    /// <value>
    /// The last date on which this tax rate applies. If null, the rate is current and has no end date.
    /// Superseded rates have an end date marking when a new rate took effect.
    /// </value>
    [JsonPropertyName("valid_to")]
    public DateOnly? ValidTo { get; init; }
}