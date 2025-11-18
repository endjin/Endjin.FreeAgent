// <copyright file="CreditNoteReconciliation.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a credit note reconciliation in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Credit note reconciliations link credit notes to invoices, allowing you to offset credit amounts
/// against outstanding invoice balances. When a credit note is reconciled with an invoice, it reduces
/// the amount owed on that invoice by the reconciled gross value.
/// </para>
/// <para>
/// Reconciliations can be created in the same currency as the related invoice and credit note, or in
/// different currencies using exchange rates. The gross value represents the amount being reconciled
/// between the two documents.
/// </para>
/// <para>
/// API Endpoint: /v2/credit_note_reconciliations
/// </para>
/// <para>
/// Minimum Access Level: Estimates and Invoices
/// </para>
/// </remarks>
/// <seealso cref="CreditNote"/>
/// <seealso cref="Invoice"/>
[DebuggerDisplay("GrossValue = {" + nameof(GrossValue) + "}, DatedOn = {" + nameof(DatedOn) + "}")]
public record CreditNoteReconciliation
{
    /// <summary>
    /// Gets the unique URI identifier for this credit note reconciliation.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this credit note reconciliation in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the amount reconciled between the credit note and invoice.
    /// </summary>
    /// <value>
    /// The gross value being reconciled. This reduces the outstanding amount on the linked invoice
    /// by the specified amount from the linked credit note.
    /// </value>
    [JsonPropertyName("gross_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? GrossValue { get; init; }

    /// <summary>
    /// Gets the effective date of the reconciliation.
    /// </summary>
    /// <value>
    /// The date when the reconciliation takes effect, in YYYY-MM-DD format.
    /// </value>
    [JsonPropertyName("dated_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? DatedOn { get; init; }

    /// <summary>
    /// Gets the currency of the reconciliation.
    /// </summary>
    /// <value>
    /// A three-letter ISO 4217 currency code (e.g., "GBP", "USD", "EUR").
    /// </value>
    [JsonPropertyName("currency")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Currency { get; init; }

    /// <summary>
    /// Gets the exchange rate at which the reconciled amount is converted into the company's native currency.
    /// </summary>
    /// <value>
    /// The rate used to convert the reconciled amount to the company's base currency,
    /// or <see langword="null"/> if not applicable.
    /// </value>
    [JsonPropertyName("exchange_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? ExchangeRate { get; init; }

    /// <summary>
    /// Gets the URI reference to the invoice being reconciled.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Invoice"/> that this reconciliation is offsetting.
    /// This field is required when creating a reconciliation.
    /// </value>
    [JsonPropertyName("invoice")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Invoice { get; init; }

    /// <summary>
    /// Gets the URI reference to the credit note being reconciled.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.CreditNote"/> being applied to the invoice.
    /// This field is required when creating a reconciliation.
    /// </value>
    [JsonPropertyName("credit_note")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? CreditNote { get; init; }

    /// <summary>
    /// Gets the date and time when this reconciliation was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing the creation timestamp in UTC.
    /// </value>
    [JsonPropertyName("created_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this reconciliation was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing the last modification timestamp in UTC.
    /// </value>
    [JsonPropertyName("updated_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? UpdatedAt { get; init; }
}
