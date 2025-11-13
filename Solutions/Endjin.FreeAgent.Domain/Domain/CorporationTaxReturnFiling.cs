// <copyright file="CorporationTaxReturnFiling.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the filing details for a UK Corporation Tax return (CT600) in the FreeAgent system.
/// </summary>
/// <remarks>
/// <para>
/// Corporation Tax return filing records track when and how a company's CT600 tax return was submitted to
/// HMRC. This information is essential for compliance tracking and audit trails, confirming that statutory
/// filing obligations have been met.
/// </para>
/// <para>
/// UK companies must file their CT600 Corporation Tax return within 12 months of the end of their accounting
/// period. The return can be filed online via HMRC's Corporation Tax Online Service or (in some cases) by paper.
/// Most companies are required to file online.
/// </para>
/// <para>
/// Key information tracked:
/// <list type="bullet">
/// <item>Filing date - when the return was submitted to HMRC</item>
/// <item>Filing method - whether submitted online or by paper</item>
/// <item>HMRC reference - confirmation reference number from HMRC</item>
/// </list>
/// </para>
/// <para>
/// The HMRC reference number serves as proof of filing and should be retained for record-keeping.
/// Late filing can result in automatic penalties from HMRC, making it important to track filing deadlines
/// and submission status.
/// </para>
/// </remarks>
/// <seealso cref="CorporationTaxReturn"/>
public record CorporationTaxReturnFiling
{
    /// <summary>
    /// Gets the date when the Corporation Tax return was filed with HMRC.
    /// </summary>
    /// <value>
    /// The filing date in YYYY-MM-DD format. This should be on or before the filing deadline
    /// (12 months after the accounting period end date) to avoid penalties.
    /// </value>
    [JsonPropertyName("filed_on")]
    public string? FiledOn { get; init; }

    /// <summary>
    /// Gets a value indicating whether the return was filed online.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the return was submitted via HMRC's online filing system;
    /// <see langword="false"/> if filed by paper. Online filing is mandatory for most companies
    /// and is the standard method for modern tax submissions.
    /// </value>
    [JsonPropertyName("filed_online")]
    public bool FiledOnline { get; init; }

    /// <summary>
    /// Gets the HMRC confirmation reference number for the filed return.
    /// </summary>
    /// <value>
    /// The reference number provided by HMRC upon successful submission. This serves as proof
    /// of filing and should be retained for audit purposes. The format varies depending on
    /// whether the return was filed online or by paper.
    /// </value>
    [JsonPropertyName("hmrc_reference")]
    public string? HmrcReference { get; init; }
}