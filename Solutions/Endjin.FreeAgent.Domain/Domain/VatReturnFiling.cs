// <copyright file="VatReturnFiling.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the filing details for a UK VAT return in the FreeAgent system.
/// </summary>
/// <remarks>
/// <para>
/// VAT return filing records track when and how a business's VAT return was submitted to HMRC.
/// This information is essential for compliance tracking and audit trails, confirming that statutory
/// filing obligations have been met under Making Tax Digital (MTD) regulations.
/// </para>
/// <para>
/// UK VAT-registered businesses must file VAT returns:
/// <list type="bullet">
/// <item>Quarterly for most businesses (every 3 months)</item>
/// <item>Monthly for some large businesses</item>
/// <item>Annually for businesses on the Annual Accounting Scheme</item>
/// </list>
/// </para>
/// <para>
/// Making Tax Digital (MTD) for VAT requires businesses with turnover above the VAT threshold to:
/// <list type="bullet">
/// <item>Keep digital records of their VAT transactions</item>
/// <item>File VAT returns online using MTD-compatible software</item>
/// <item>Submit returns within 1 month and 7 days of the end of the VAT period</item>
/// </list>
/// </para>
/// <para>
/// Key information tracked:
/// <list type="bullet">
/// <item>Filing date - when the return was submitted to HMRC</item>
/// <item>Filing method - whether submitted online (required for MTD)</item>
/// <item>HMRC reference - confirmation reference number from HMRC's system</item>
/// </list>
/// </para>
/// <para>
/// The HMRC reference number serves as proof of filing and should be retained for record-keeping.
/// Late filing can result in default surcharges and penalties from HMRC.
/// </para>
/// </remarks>
/// <seealso cref="VatReturn"/>
public record VatReturnFiling
{
    /// <summary>
    /// Gets the date when the VAT return was filed with HMRC.
    /// </summary>
    /// <value>
    /// The filing date in YYYY-MM-DD format. This should be on or before the filing deadline
    /// (1 month and 7 days after the VAT period end date) to avoid penalties.
    /// </value>
    [JsonPropertyName("filed_on")]
    public string? FiledOn { get; init; }

    /// <summary>
    /// Gets a value indicating whether the return was filed online.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the return was submitted via HMRC's online MTD service;
    /// <see langword="false"/> if filed by paper (only allowed for exempt businesses).
    /// Under Making Tax Digital, most VAT-registered businesses must file online.
    /// </value>
    [JsonPropertyName("filed_online")]
    public bool FiledOnline { get; init; }

    /// <summary>
    /// Gets the HMRC confirmation reference number for the filed return.
    /// </summary>
    /// <value>
    /// The reference number provided by HMRC upon successful submission. For MTD submissions,
    /// this includes a formattedVRN and receipt timestamp. This serves as proof of filing
    /// and should be retained for audit purposes.
    /// </value>
    [JsonPropertyName("hmrc_reference")]
    public string? HmrcReference { get; init; }
}