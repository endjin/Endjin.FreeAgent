// <copyright file="AgedCreditorEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a single supplier entry in an aged creditors report showing outstanding bill balances.
/// </summary>
/// <remarks>
/// <para>
/// Aged creditor entries break down how much the business owes to each supplier, grouped by how overdue
/// the bills are. This aging analysis helps manage cash flow, prioritize payments, maintain good supplier
/// relationships, and avoid late payment penalties.
/// </para>
/// <para>
/// The aging buckets (current, 1-30 days, 31-60 days, 61-90 days, over 90 days) help identify which
/// supplier payments are most urgent and ensure the business meets its payment obligations on time.
/// </para>
/// </remarks>
/// <seealso cref="PurchaseAgedCreditors"/>
/// <seealso cref="Contact"/>
/// <seealso cref="Bill"/>
public record AgedCreditorEntry
{
    /// <summary>
    /// Gets the URI reference to the supplier contact.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Contact"/> to whom this money is owed.
    /// </value>
    [JsonPropertyName("contact")]
    public Uri? Contact { get; init; }

    /// <summary>
    /// Gets the name of the supplier contact.
    /// </summary>
    /// <value>
    /// The display name of the supplier for reporting purposes.
    /// </value>
    [JsonPropertyName("contact_name")]
    public string? ContactName { get; init; }

    /// <summary>
    /// Gets the amount currently due but not yet overdue.
    /// </summary>
    /// <value>
    /// The total value of bills that are due but within their payment terms (not yet late).
    /// </value>
    [JsonPropertyName("current")]
    public decimal? Current { get; init; }

    /// <summary>
    /// Gets the amount overdue by 1 to 30 days.
    /// </summary>
    /// <value>
    /// The total value of bills that are between 1 and 30 days past their due date.
    /// </value>
    [JsonPropertyName("overdue_1_to_30_days")]
    public decimal? Overdue1To30Days { get; init; }

    /// <summary>
    /// Gets the amount overdue by 31 to 60 days.
    /// </summary>
    /// <value>
    /// The total value of bills that are between 31 and 60 days past their due date.
    /// </value>
    [JsonPropertyName("overdue_31_to_60_days")]
    public decimal? Overdue31To60Days { get; init; }

    /// <summary>
    /// Gets the amount overdue by 61 to 90 days.
    /// </summary>
    /// <value>
    /// The total value of bills that are between 61 and 90 days past their due date.
    /// </value>
    [JsonPropertyName("overdue_61_to_90_days")]
    public decimal? Overdue61To90Days { get; init; }

    /// <summary>
    /// Gets the amount overdue by more than 90 days.
    /// </summary>
    /// <value>
    /// The total value of bills that are more than 90 days past their due date.
    /// These may incur late payment penalties and could damage supplier relationships.
    /// </value>
    [JsonPropertyName("overdue_over_90_days")]
    public decimal? OverdueOver90Days { get; init; }

    /// <summary>
    /// Gets the total amount owed to this supplier across all aging buckets.
    /// </summary>
    /// <value>
    /// The sum of current and all overdue amounts, representing the total outstanding balance to this supplier.
    /// </value>
    [JsonPropertyName("total")]
    public decimal? Total { get; init; }
}