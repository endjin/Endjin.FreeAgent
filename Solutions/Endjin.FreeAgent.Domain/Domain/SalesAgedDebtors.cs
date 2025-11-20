// <copyright file="SalesAgedDebtors.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a sales aged debtors report showing all outstanding customer balances grouped by aging.
/// </summary>
/// <remarks>
/// <para>
/// The sales aged debtors report provides a comprehensive view of all money owed to the business by customers,
/// broken down by how overdue the invoices are. This report aggregates individual <see cref="AgedDebtorEntry"/>
/// records and provides totals across all customers for each aging bucket.
/// </para>
/// <para>
/// This report is essential for:
/// - Monitoring overall accounts receivable and cash flow
/// - Identifying collection priorities (older debts are higher risk)
/// - Assessing the business's credit control effectiveness
/// - Understanding working capital tied up in unpaid invoices
/// - Making decisions about credit terms and debt recovery
/// </para>
/// <para>
/// The aging buckets help visualize the urgency of collections: current amounts are within terms,
/// while amounts in the 90+ days bucket may require immediate action such as payment reminders,
/// suspension of credit, or formal debt recovery procedures.
/// </para>
/// <para>
/// API Endpoint: /v2/reports/aged_debtors
/// </para>
/// <para>
/// Minimum Access Level: Standard
/// </para>
/// </remarks>
/// <seealso cref="AgedDebtorEntry"/>
/// <seealso cref="PurchaseAgedCreditors"/>
/// <seealso cref="Contact"/>
/// <seealso cref="Invoice"/>
public record SalesAgedDebtors
{
    /// <summary>
    /// Gets the date on which this aged debtors report was generated.
    /// </summary>
    /// <value>
    /// The report date, typically the current date or end of an accounting period.
    /// All aging calculations are based on this date.
    /// </value>
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; init; }

    /// <summary>
    /// Gets the list of individual aged debtor entries, one for each customer with outstanding balances.
    /// </summary>
    /// <value>
    /// A collection of <see cref="AgedDebtorEntry"/> objects showing the breakdown of amounts owed
    /// by each customer across different aging buckets.
    /// </value>
    [JsonPropertyName("entries")]
    public List<AgedDebtorEntry>? Entries { get; init; }

    /// <summary>
    /// Gets the total amount that is current (not yet overdue) across all customers.
    /// </summary>
    /// <value>
    /// The sum of all current amounts from all customer entries. These invoices are within their payment terms
    /// and do not require immediate collection action.
    /// </value>
    [JsonPropertyName("total_current")]
    public decimal? TotalCurrent { get; init; }

    /// <summary>
    /// Gets the total amount overdue by 1 to 30 days across all customers.
    /// </summary>
    /// <value>
    /// The sum of all amounts that are 1-30 days past their due date. These invoices warrant
    /// a polite payment reminder.
    /// </value>
    [JsonPropertyName("total_overdue_1_to_30_days")]
    public decimal? TotalOverdue1To30Days { get; init; }

    /// <summary>
    /// Gets the total amount overdue by 31 to 60 days across all customers.
    /// </summary>
    /// <value>
    /// The sum of all amounts that are 31-60 days past their due date. These invoices require
    /// follow-up and may warrant phone calls or escalated communication.
    /// </value>
    [JsonPropertyName("total_overdue_31_to_60_days")]
    public decimal? TotalOverdue31To60Days { get; init; }

    /// <summary>
    /// Gets the total amount overdue by 61 to 90 days across all customers.
    /// </summary>
    /// <value>
    /// The sum of all amounts that are 61-90 days past their due date. These invoices represent
    /// significant credit risk and may require formal collection procedures.
    /// </value>
    [JsonPropertyName("total_overdue_61_to_90_days")]
    public decimal? TotalOverdue61To90Days { get; init; }

    /// <summary>
    /// Gets the total amount overdue by more than 90 days across all customers.
    /// </summary>
    /// <value>
    /// The sum of all amounts that are more than 90 days past their due date. These invoices represent
    /// the highest collection risk and may require legal action, debt collection services, or write-off as bad debt.
    /// </value>
    [JsonPropertyName("total_overdue_over_90_days")]
    public decimal? TotalOverdueOver90Days { get; init; }

    /// <summary>
    /// Gets the total amount owed across all customers and all aging buckets.
    /// </summary>
    /// <value>
    /// The grand total of all outstanding receivables, calculated as the sum of TotalCurrent plus
    /// all overdue totals. This represents the business's total accounts receivable balance.
    /// </value>
    [JsonPropertyName("total")]
    public decimal? Total { get; init; }
}