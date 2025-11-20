// <copyright file="SalesTaxPeriod.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Converters;

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a sales tax period in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Sales tax periods define the tax configuration for a company over a specific time period.
/// They track tax names, registration status, tax rates, and whether tax can be reclaimed.
/// Multiple periods can exist with different effective dates to handle changes in tax status over time.
/// </para>
/// <para>
/// Available for US and Universal companies only. This functionality allows businesses to manage
/// different tax periods as their tax obligations change (e.g., registering for VAT, changing tax rates).
/// </para>
/// <para>
/// API Endpoint: /v2/sales_tax_periods
/// </para>
/// <para>
/// Minimum Access Level: Estimates and Invoices (read), Full Access (create/update/delete)
/// </para>
/// </remarks>
[DebuggerDisplay("SalesTaxName = {" + nameof(SalesTaxName) + "}, EffectiveDate = {" + nameof(EffectiveDate) + "}")]
public record SalesTaxPeriod
{
    /// <summary>
    /// Gets the unique URI identifier for this sales tax period.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this sales tax period in the FreeAgent system.
    /// This value is assigned by the API upon creation and is used to reference the period in other resources.
    /// </value>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the name of the sales tax (e.g., "VAT", "GST").
    /// </summary>
    /// <value>
    /// The tax identifier used in the accounting system. This field is required when creating a new period.
    /// Common values include "VAT" for UK/EU businesses, "GST" for Australian/NZ businesses, or custom names.
    /// </value>
    [JsonPropertyName("sales_tax_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SalesTaxName { get; init; }

    /// <summary>
    /// Gets the sales tax registration status.
    /// </summary>
    /// <value>
    /// Indicates whether the company is registered for sales tax ("Registered") or not ("Not Registered").
    /// This field is required when creating a new period and affects tax calculations and reporting.
    /// </value>
    [JsonPropertyName("sales_tax_registration_status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(SalesTaxRegistrationStatusJsonConverter))]
    public SalesTaxRegistrationStatus? SalesTaxRegistrationStatus { get; init; }

    /// <summary>
    /// Gets a value indicating whether sales tax is value-added (reclaimable).
    /// </summary>
    /// <value>
    /// <c>true</c> if the sales tax can be reclaimed (like VAT/GST); <c>false</c> if it cannot be reclaimed (like US sales tax).
    /// This field is required when creating a new period and affects how tax is handled in transactions.
    /// </value>
    [JsonPropertyName("sales_tax_is_value_added")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? SalesTaxIsValueAdded { get; init; }

    /// <summary>
    /// Gets the date from which this sales tax period is effective.
    /// </summary>
    /// <value>
    /// The date in YYYY-MM-DD format when this tax configuration becomes active.
    /// This field is required when creating a new period. Periods cannot have overlapping effective dates.
    /// </value>
    [JsonPropertyName("effective_date")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? EffectiveDate { get; init; }

    /// <summary>
    /// Gets the first sales tax rate percentage.
    /// </summary>
    /// <value>
    /// The primary tax rate as a percentage (e.g., 20 for 20% VAT).
    /// This is typically the standard rate for the tax jurisdiction.
    /// </value>
    [JsonPropertyName("sales_tax_rate_1")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SalesTaxRate1 { get; init; }

    /// <summary>
    /// Gets the second sales tax rate percentage.
    /// </summary>
    /// <value>
    /// A secondary tax rate as a percentage (e.g., 5 for 5% reduced rate).
    /// Used for reduced rates in jurisdictions that have multiple tax rates.
    /// </value>
    [JsonPropertyName("sales_tax_rate_2")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SalesTaxRate2 { get; init; }

    /// <summary>
    /// Gets the third sales tax rate percentage.
    /// </summary>
    /// <value>
    /// A tertiary tax rate as a percentage (e.g., 0 for zero-rated items).
    /// Used for zero-rated or special rate items in the tax system.
    /// </value>
    [JsonPropertyName("sales_tax_rate_3")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SalesTaxRate3 { get; init; }

    /// <summary>
    /// Gets the sales tax registration number.
    /// </summary>
    /// <value>
    /// The official tax registration number assigned by the tax authority (e.g., VAT number).
    /// This is typically required when the registration status is "Registered".
    /// </value>
    [JsonPropertyName("sales_tax_registration_number")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SalesTaxRegistrationNumber { get; init; }

    /// <summary>
    /// Gets a value indicating whether this sales tax period is locked.
    /// </summary>
    /// <value>
    /// <c>true</c> if the period is locked and cannot be modified or deleted;
    /// <c>false</c> if it can be edited. Periods are typically locked when they have associated transactions.
    /// </value>
    [JsonPropertyName("is_locked")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsLocked { get; init; }

    /// <summary>
    /// Gets a value indicating whether there is a reason for the period being locked.
    /// </summary>
    /// <value>
    /// <c>true</c> if there is a specific reason why this period is locked;
    /// <c>false</c> or <see langword="null"/> otherwise.
    /// </value>
    [JsonPropertyName("locked_reason")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? LockedReason { get; init; }

    /// <summary>
    /// Gets the name of the second sales tax (Universal companies only).
    /// </summary>
    /// <value>
    /// For Universal companies, this allows tracking a secondary tax system.
    /// This is used when a business needs to track multiple tax jurisdictions simultaneously.
    /// </value>
    [JsonPropertyName("second_sales_tax_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SecondSalesTaxName { get; init; }

    /// <summary>
    /// Gets the first rate for the second sales tax (Universal companies only).
    /// </summary>
    /// <value>
    /// The primary rate for the second tax system as a percentage.
    /// </value>
    [JsonPropertyName("second_sales_tax_rate_1")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SecondSalesTaxRate1 { get; init; }

    /// <summary>
    /// Gets the second rate for the second sales tax (Universal companies only).
    /// </summary>
    /// <value>
    /// The secondary rate for the second tax system as a percentage.
    /// </value>
    [JsonPropertyName("second_sales_tax_rate_2")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SecondSalesTaxRate2 { get; init; }

    /// <summary>
    /// Gets the third rate for the second sales tax (Universal companies only).
    /// </summary>
    /// <value>
    /// The tertiary rate for the second tax system as a percentage.
    /// </value>
    [JsonPropertyName("second_sales_tax_rate_3")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SecondSalesTaxRate3 { get; init; }

    /// <summary>
    /// Gets a value indicating whether the second sales tax is compound (Universal companies only).
    /// </summary>
    /// <value>
    /// <c>true</c> if the second tax is calculated on top of the first tax (compound);
    /// <c>false</c> if both taxes are calculated independently on the base amount.
    /// </value>
    [JsonPropertyName("second_sales_tax_is_compound")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? SecondSalesTaxIsCompound { get; init; }
}