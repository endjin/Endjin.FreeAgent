// <copyright file="AgedDebtorEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a single customer entry in an aged debtors report showing outstanding invoice balances.
/// </summary>
/// <remarks>
/// <para>
/// Aged debtor entries break down how much each customer owes, grouped by how overdue the invoices are.
/// This aging analysis helps identify slow-paying customers, prioritize collection efforts, and assess
/// credit risk.
/// </para>
/// <para>
/// The aging buckets (current, 1-30 days, 31-60 days, 61-90 days, over 90 days) provide insight into
/// payment patterns and help identify customers who may need payment reminders or credit control actions.
/// </para>
/// </remarks>
/// <seealso cref="SalesAgedDebtors"/>
/// <seealso cref="Contact"/>
/// <seealso cref="Invoice"/>
public record AgedDebtorEntry
{
    /// <summary>
    /// Gets the URI reference to the customer contact.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Contact"/> who owes this money.
    /// </value>
    [JsonPropertyName("contact")]
    public Uri? Contact { get; init; }

    /// <summary>
    /// Gets the name of the customer contact.
    /// </summary>
    /// <value>
    /// The display name of the customer for reporting purposes.
    /// </value>
    [JsonPropertyName("contact_name")]
    public string? ContactName { get; init; }

    /// <summary>
    /// Gets the amount currently due but not yet overdue.
    /// </summary>
    /// <value>
    /// The total value of invoices that are due but within their payment terms (not yet late).
    /// </value>
    [JsonPropertyName("current")]
    public decimal? Current { get; init; }

    /// <summary>
    /// Gets the amount overdue by 1 to 30 days.
    /// </summary>
    /// <value>
    /// The total value of invoices that are between 1 and 30 days past their due date.
    /// </value>
    [JsonPropertyName("overdue_1_to_30_days")]
    public decimal? Overdue1To30Days { get; init; }

    /// <summary>
    /// Gets the amount overdue by 31 to 60 days.
    /// </summary>
    /// <value>
    /// The total value of invoices that are between 31 and 60 days past their due date.
    /// </value>
    [JsonPropertyName("overdue_31_to_60_days")]
    public decimal? Overdue31To60Days { get; init; }

    /// <summary>
    /// Gets the amount overdue by 61 to 90 days.
    /// </summary>
    /// <value>
    /// The total value of invoices that are between 61 and 90 days past their due date.
    /// </value>
    [JsonPropertyName("overdue_61_to_90_days")]
    public decimal? Overdue61To90Days { get; init; }

    /// <summary>
    /// Gets the amount overdue by more than 90 days.
    /// </summary>
    /// <value>
    /// The total value of invoices that are more than 90 days past their due date.
    /// These are significantly overdue and may require escalated collection actions.
    /// </value>
    [JsonPropertyName("overdue_over_90_days")]
    public decimal? OverdueOver90Days { get; init; }

    /// <summary>
    /// Gets the total amount owed by this customer across all aging buckets.
    /// </summary>
    /// <value>
    /// The sum of current and all overdue amounts, representing the customer's total outstanding balance.
    /// </value>
    [JsonPropertyName("total")]
    public decimal? Total { get; init; }
}