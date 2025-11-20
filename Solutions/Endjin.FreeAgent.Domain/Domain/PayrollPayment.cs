// <copyright file="PayrollPayment.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a payroll payment due to HMRC in the FreeAgent RTI (Real Time Information) payroll system.
/// </summary>
/// <remarks>
/// <para>
/// Payroll payments represent the amounts due to HMRC for PAYE tax, National Insurance contributions,
/// and other payroll-related liabilities. These payments are typically due monthly or quarterly
/// depending on the company's PAYE scheme.
/// </para>
/// <para>
/// Payment status values:
/// <list type="bullet">
/// <item><b>unpaid</b> - Payment has not yet been made to HMRC</item>
/// <item><b>marked_as_paid</b> - Payment has been marked as paid in FreeAgent</item>
/// </list>
/// </para>
/// <para>
/// Note: The "paid" status is reserved for future use when FreeAgent can confirm actual payment to HMRC.
/// </para>
/// <para>
/// API Access: Read-only via GET /v2/payroll/:year. Can be marked as paid/unpaid via PUT endpoints.
/// Minimum Access Level: Tax and Limited Accounting. Only available for UK companies.
/// </para>
/// </remarks>
/// <seealso cref="PayrollPeriod"/>
/// <seealso cref="Payslip"/>
public record PayrollPayment
{
    /// <summary>
    /// Gets the date when this payment is due to HMRC.
    /// </summary>
    /// <value>
    /// The payment due date in YYYY-MM-DD format. PAYE payments are typically due by the 22nd of each month
    /// (or 19th if paying by post).
    /// </value>
    [JsonPropertyName("due_on")]
    public DateOnly? DueOn { get; init; }

    /// <summary>
    /// Gets the amount due to HMRC.
    /// </summary>
    /// <value>
    /// The total amount due for this payment, including PAYE tax, National Insurance, student loan
    /// repayments, and any other deductions that must be remitted to HMRC.
    /// </value>
    [JsonPropertyName("amount_due")]
    public decimal? AmountDue { get; init; }

    /// <summary>
    /// Gets the payment status.
    /// </summary>
    /// <value>
    /// The current status of the payment: "unpaid" or "marked_as_paid".
    /// This field may be omitted if no payments are due.
    /// </value>
    [JsonPropertyName("status")]
    public string? Status { get; init; }
}
