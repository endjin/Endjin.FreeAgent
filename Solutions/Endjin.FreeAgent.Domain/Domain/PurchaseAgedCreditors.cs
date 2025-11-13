// <copyright file="PurchaseAgedCreditors.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a purchase aged creditors report (also known as aged payables or accounts payable aging report)
/// in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// The aged creditors report provides a comprehensive view of all outstanding supplier balances, showing how much
/// money the business owes to suppliers and how long bills have been unpaid. It aggregates data across all suppliers
/// to give a complete picture of payables.
/// </para>
/// <para>
/// This report is essential for:
/// <list type="bullet">
/// <item>Cash flow planning - scheduling when payments need to be made</item>
/// <item>Supplier relationship management - avoiding late payment penalties and maintaining good terms</item>
/// <item>Working capital optimization - balancing payment timing with available cash</item>
/// <item>Compliance - ensuring statutory payment terms are met (e.g., UK 30-day rules for small suppliers)</item>
/// <item>Discount optimization - identifying early payment discount opportunities</item>
/// </list>
/// </para>
/// <para>
/// The report breaks down total payables by aging buckets (current, 1-30 days, 31-60 days, 61-90 days, over 90 days)
/// and includes individual entries for each supplier via the <see cref="Entries"/> collection.
/// </para>
/// <para>
/// While businesses often focus on collecting from customers (aged debtors), managing aged creditors is equally
/// important for maintaining supplier relationships, avoiding late payment fees, and taking advantage of early
/// payment discounts.
/// </para>
/// <para>
/// API Access: Accessible via GET /v2/aged_creditors
/// Minimum Access Level: Read-only access to reports
/// </para>
/// </remarks>
/// <seealso cref="AgedCreditorEntry"/>
/// <seealso cref="SalesAgedDebtors"/>
/// <seealso cref="Contact"/>
/// <seealso cref="Bill"/>
public record PurchaseAgedCreditors
{
    /// <summary>
    /// Gets the date on which this aged creditors report was generated.
    /// </summary>
    /// <value>
    /// The report date, typically the current date or end of an accounting period.
    /// All aging calculations are based on this date.
    /// </value>
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; init; }

    /// <summary>
    /// Gets the list of individual aged creditor entries, one for each supplier with outstanding balances.
    /// </summary>
    /// <value>
    /// A collection of <see cref="AgedCreditorEntry"/> objects showing the breakdown of amounts owed
    /// to each supplier across different aging buckets.
    /// </value>
    [JsonPropertyName("entries")]
    public List<AgedCreditorEntry>? Entries { get; init; }

    /// <summary>
    /// Gets the total amount that is current (not yet due) across all suppliers.
    /// </summary>
    /// <value>
    /// The sum of all current amounts from all supplier entries. These bills are within their payment terms
    /// and may qualify for early payment discounts if paid promptly.
    /// </value>
    [JsonPropertyName("total_current")]
    public decimal? TotalCurrent { get; init; }

    /// <summary>
    /// Gets the total amount overdue by 1 to 30 days across all suppliers.
    /// </summary>
    /// <value>
    /// The sum of all amounts that are 1-30 days past their due date. These bills should be prioritized
    /// for payment to maintain good supplier relationships and avoid late payment charges.
    /// </value>
    [JsonPropertyName("total_overdue_1_to_30_days")]
    public decimal? TotalOverdue1To30Days { get; init; }

    /// <summary>
    /// Gets the total amount overdue by 31 to 60 days across all suppliers.
    /// </summary>
    /// <value>
    /// The sum of all amounts that are 31-60 days past their due date. These bills may already be incurring
    /// late payment interest and could damage supplier relationships if not addressed promptly.
    /// </value>
    [JsonPropertyName("total_overdue_31_to_60_days")]
    public decimal? TotalOverdue31To60Days { get; init; }

    /// <summary>
    /// Gets the total amount overdue by 61 to 90 days across all suppliers.
    /// </summary>
    /// <value>
    /// The sum of all amounts that are 61-90 days past their due date. These bills represent serious
    /// payment delays that may result in suppliers suspending credit terms or taking legal action.
    /// </value>
    [JsonPropertyName("total_overdue_61_to_90_days")]
    public decimal? TotalOverdue61To90Days { get; init; }

    /// <summary>
    /// Gets the total amount overdue by more than 90 days across all suppliers.
    /// </summary>
    /// <value>
    /// The sum of all amounts that are more than 90 days past their due date. These bills represent
    /// critical payment issues that may result in legal action, damage to credit rating, or loss of
    /// essential suppliers.
    /// </value>
    [JsonPropertyName("total_overdue_over_90_days")]
    public decimal? TotalOverdueOver90Days { get; init; }

    /// <summary>
    /// Gets the total amount owed across all suppliers and all aging buckets.
    /// </summary>
    /// <value>
    /// The grand total of all outstanding payables, calculated as the sum of TotalCurrent plus
    /// all overdue totals. This represents the business's total accounts payable balance.
    /// </value>
    [JsonPropertyName("total")]
    public decimal? Total { get; init; }
}