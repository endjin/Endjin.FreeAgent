// <copyright file="SelfAssessmentReturnFiling.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the filing details for a UK Self Assessment tax return (SA100) in the FreeAgent system.
/// </summary>
/// <remarks>
/// <para>
/// Self Assessment return filing records track when and how an individual's SA100 tax return was submitted to
/// HMRC. This information is essential for compliance tracking and audit trails, confirming that statutory
/// filing obligations have been met.
/// </para>
/// <para>
/// UK individuals in Self Assessment must file their tax return by the following deadlines:
/// <list type="bullet">
/// <item>31 October (paper filing) - for the tax year ended previous 5 April</item>
/// <item>31 January (online filing) - for the tax year ended previous 5 April</item>
/// </list>
/// </para>
/// <para>
/// Key information tracked:
/// <list type="bullet">
/// <item>Filing date - when the return was submitted to HMRC</item>
/// <item>Filing method - whether submitted online or by paper</item>
/// <item>UTR number - the individual's Unique Taxpayer Reference</item>
/// </list>
/// </para>
/// <para>
/// The Unique Taxpayer Reference (UTR) is a 10-digit reference number used to identify the taxpayer
/// with HMRC. Late filing can result in automatic penalties from HMRC (£100 minimum), making it important
/// to track filing deadlines and submission status.
/// </para>
/// </remarks>
/// <seealso cref="SelfAssessmentReturn"/>
public record SelfAssessmentReturnFiling
{
    /// <summary>
    /// Gets the date when the Self Assessment return was filed with HMRC.
    /// </summary>
    /// <value>
    /// The filing date in YYYY-MM-DD format. This should be on or before the filing deadline
    /// (31 January for online filing, 31 October for paper filing) to avoid penalties.
    /// </value>
    [JsonPropertyName("filed_on")]
    public string? FiledOn { get; init; }

    /// <summary>
    /// Gets a value indicating whether the return was filed online.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the return was submitted via HMRC's online Self Assessment service;
    /// <see langword="false"/> if filed by paper. Online filing is recommended as it has a later deadline
    /// (31 January vs 31 October) and provides immediate confirmation.
    /// </value>
    [JsonPropertyName("filed_online")]
    public bool FiledOnline { get; init; }

    /// <summary>
    /// Gets the Unique Taxpayer Reference (UTR) number for the individual.
    /// </summary>
    /// <value>
    /// The 10-digit UTR number assigned by HMRC to identify the taxpayer. This is required for all
    /// Self Assessment communications and filings. Format is typically 10 digits without spaces.
    /// </value>
    [JsonPropertyName("utr_number")]
    public string? UtrNumber { get; init; }
}