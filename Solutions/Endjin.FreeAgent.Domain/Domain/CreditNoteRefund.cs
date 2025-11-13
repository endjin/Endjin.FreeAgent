// <copyright file="CreditNoteRefund.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a refund payment issued against a credit note in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Credit note refunds record when and how money is returned to a customer following a credit note.
/// When a credit note is issued (e.g., for returned goods or billing corrections), the business may
/// need to refund the customer rather than apply the credit to future invoices.
/// </para>
/// <para>
/// Each refund is associated with a specific bank account from which the money is paid, allowing
/// proper cash flow tracking and bank reconciliation. The refund date determines when the transaction
/// appears in financial reports and bank account balances.
/// </para>
/// <para>
/// Common scenarios for credit note refunds:
/// <list type="bullet">
/// <item>Customer returns goods and requests money back</item>
/// <item>Overcharged customer needs reimbursement</item>
/// <item>Service cancellation with refund</item>
/// <item>Duplicate payment returned to customer</item>
/// <item>Customer closing account and liquidating credit balance</item>
/// </list>
/// </para>
/// <para>
/// API Access: Accessible via POST /v2/credit_note_refunds
/// Minimum Access Level: Invoicing access
/// </para>
/// </remarks>
/// <seealso cref="CreditNote"/>
/// <seealso cref="BankAccount"/>
/// <seealso cref="Invoice"/>
public record CreditNoteRefund
{
    /// <summary>
    /// Gets the date when the refund was issued to the customer.
    /// </summary>
    /// <value>
    /// The refund payment date in YYYY-MM-DD format. This determines when the transaction affects
    /// cash flow reports and bank account balances. This field is required when creating a refund.
    /// </value>
    [JsonPropertyName("refunded_on")]
    public string? RefundedOn { get; init; }

    /// <summary>
    /// Gets the API URL of the bank account from which the refund payment was made.
    /// </summary>
    /// <value>
    /// A reference to the <see cref="BankAccount"/> resource used to pay the refund.
    /// This bank account's balance will be decreased by the refund amount. This field is required
    /// when creating a refund.
    /// </value>
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; init; }
}