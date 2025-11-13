// <copyright file="CashFlowItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents an individual cash transaction item within a cash flow statement in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Cash flow items represent actual money movements in or out of the business, such as customer payments,
/// supplier bills paid, expense reimbursements, loan receipts, or tax payments. Each item shows the specific
/// transaction, its value, and when it occurred.
/// </para>
/// <para>
/// Unlike accrual accounting which records income when earned and expenses when incurred, cash flow items
/// track actual cash movements, providing a true picture of liquidity and cash availability.
/// </para>
/// </remarks>
/// <seealso cref="CashFlow"/>
/// <seealso cref="BankTransaction"/>
public record CashFlowItem
{
    /// <summary>
    /// Gets the description of this cash flow transaction.
    /// </summary>
    /// <value>
    /// A text description identifying the cash transaction, such as "Customer Payment - INV-001",
    /// "Supplier Payment - ABC Ltd", or "Tax Payment - VAT".
    /// </value>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the monetary value of this cash transaction.
    /// </summary>
    /// <value>
    /// The amount of cash received (positive value) or paid out (negative value).
    /// </value>
    [JsonPropertyName("value")]
    public decimal? Value { get; init; }

    /// <summary>
    /// Gets the date when this cash transaction occurred.
    /// </summary>
    /// <value>
    /// The date the cash was actually received or paid, as recorded in the bank account.
    /// </value>
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; init; }
}