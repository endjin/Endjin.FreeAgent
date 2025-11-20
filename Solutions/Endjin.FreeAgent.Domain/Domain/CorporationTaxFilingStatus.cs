// <copyright file="CorporationTaxFilingStatus.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the filing status of a Corporation Tax return in FreeAgent.
/// </summary>
/// <remarks>
/// <para>
/// This enum reflects the various states a Corporation Tax return can be in during
/// its lifecycle, from draft to filed status.
/// </para>
/// <para>
/// The FreeAgent API represents filing status values as strings:
/// "draft", "unfiled", "pending", "rejected", "filed", and "marked_as_filed".
/// </para>
/// </remarks>
public enum CorporationTaxFilingStatus
{
    /// <summary>
    /// The return is in draft status.
    /// </summary>
    Draft,

    /// <summary>
    /// The return has not been filed yet.
    /// </summary>
    Unfiled,

    /// <summary>
    /// The return is pending submission.
    /// </summary>
    Pending,

    /// <summary>
    /// The return has been rejected by HMRC.
    /// </summary>
    Rejected,

    /// <summary>
    /// The return has been filed electronically with HMRC.
    /// </summary>
    Filed,

    /// <summary>
    /// The return has been marked as filed manually.
    /// </summary>
    MarkedAsFiled
}