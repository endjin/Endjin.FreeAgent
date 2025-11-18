// <copyright file="FinalAccountsFilingStatus.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the filing status of a Final Accounts report in FreeAgent.
/// </summary>
/// <remarks>
/// <para>
/// This enum reflects the various states a Final Accounts report (statutory accounts for UK companies)
/// can be in during its lifecycle, from draft to filed status.
/// </para>
/// <para>
/// The FreeAgent API represents filing status values as strings:
/// "draft", "unfiled", "pending", "rejected", "filed", and "marked_as_filed".
/// </para>
/// </remarks>
public enum FinalAccountsFilingStatus
{
    /// <summary>
    /// The report is in draft status.
    /// </summary>
    Draft,

    /// <summary>
    /// The report has not been filed yet.
    /// </summary>
    Unfiled,

    /// <summary>
    /// The report is pending submission.
    /// </summary>
    Pending,

    /// <summary>
    /// The report has been rejected by Companies House.
    /// </summary>
    Rejected,

    /// <summary>
    /// The report has been filed electronically with Companies House.
    /// </summary>
    Filed,

    /// <summary>
    /// The report has been marked as filed manually.
    /// </summary>
    MarkedAsFiled
}
