// <copyright file="FinalAccountsReport.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Converters;
using Endjin.FreeAgent.Domain.Domain;

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a Final Accounts report (statutory accounts) for a UK limited company in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Final Accounts reports are statutory accounts that UK limited companies must file with Companies House. These
/// accounts summarize the company's financial position at the end of the accounting period and include the
/// balance sheet, profit and loss account (if applicable), and notes to the accounts.
/// </para>
/// <para>
/// Key aspects of Final Accounts:
/// - Filed annually based on the company's accounting period (typically 12 months)
/// - Must be filed with Companies House within 9 months of the accounting period end date (for private companies)
/// - Public companies must file within 6 months of their accounting period end date
/// - Can be filed electronically through Companies House
/// </para>
/// <para>
/// The report tracks the filing status, filing deadline, and Companies House references for audit purposes.
/// </para>
/// <para>
/// API Endpoint: /v2/final_accounts_reports
/// </para>
/// <para>
/// Minimum Access Level: Tax, Accounting &amp; Users
/// </para>
/// </remarks>
/// <seealso cref="Company"/>
/// <seealso cref="BalanceSheet"/>
public record FinalAccountsReport
{
    /// <summary>
    /// Gets the unique URI identifier for this Final Accounts report.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this Final Accounts report in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    public required Uri Url { get; init; }

    /// <summary>
    /// Gets the start date of the accounting period for this Final Accounts report.
    /// </summary>
    /// <value>
    /// The first date of the company's accounting period covered by this report.
    /// </value>
    [JsonPropertyName("period_starts_on")]
    public required DateOnly PeriodStartsOn { get; init; }

    /// <summary>
    /// Gets the end date of the accounting period for this Final Accounts report.
    /// </summary>
    /// <value>
    /// The last date of the company's accounting period covered by this report.
    /// This date determines the filing deadline.
    /// </value>
    [JsonPropertyName("period_ends_on")]
    public required DateOnly PeriodEndsOn { get; init; }

    /// <summary>
    /// Gets the deadline date for filing the Final Accounts report with Companies House.
    /// </summary>
    /// <value>
    /// The submission deadline for the Final Accounts report. Read-only.
    /// For private companies, this is typically 9 months after the accounting period end date.
    /// Late filing incurs penalties from Companies House.
    /// </value>
    [JsonPropertyName("filing_due_on")]
    public required DateOnly FilingDueOn { get; init; }

    /// <summary>
    /// Gets the filing status of this Final Accounts report.
    /// </summary>
    /// <value>
    /// The current filing status: draft, unfiled, pending, rejected, filed, or marked_as_filed.
    /// </value>
    [JsonPropertyName("filing_status")]
    [JsonConverter(typeof(FinalAccountsFilingStatusNonNullableJsonConverter))]
    public required FinalAccountsFilingStatus FilingStatus { get; init; }

    /// <summary>
    /// Gets the date and time when this Final Accounts report was filed with Companies House.
    /// </summary>
    /// <value>
    /// The timestamp when the statutory accounts were submitted to Companies House. Read-only, set automatically when filed online.
    /// </value>
    [JsonPropertyName("filed_at")]
    public DateTime? FiledAt { get; init; }

    /// <summary>
    /// Gets the reference number for this filed Final Accounts report.
    /// </summary>
    /// <value>
    /// The submission reference number from Companies House for this filed report.
    /// Read-only, set automatically when filed online.
    /// </value>
    [JsonPropertyName("filed_reference")]
    public string? FiledReference { get; init; }
}
