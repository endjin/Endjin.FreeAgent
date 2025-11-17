// <copyright file="BankAccount.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

/// <summary>
/// Represents a bank account in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Bank accounts track financial transactions and balances for business and personal accounts.
/// FreeAgent supports three types of accounts: Standard Bank Accounts, PayPal Accounts, and Credit Card Accounts.
/// Each type has specific properties and behaviors suited to its purpose.
/// </para>
/// <para>
/// Standard bank accounts include full banking details (account number, sort code, IBAN, BIC) and can be designated
/// as primary accounts. PayPal accounts track email-based transactions, while credit card accounts manage credit obligations.
/// </para>
/// <para>
/// Bank accounts support automatic bank feed integration for transaction import, multi-currency operations,
/// and differentiation between business and personal accounts. The currency becomes immutable once transactions exist
/// to maintain data integrity.
/// </para>
/// <para>
/// API Endpoint: /v2/bank_accounts
/// </para>
/// <para>
/// Minimum Access Level: Banking
/// </para>
/// </remarks>
/// <seealso cref="BankTransaction"/>
[DebuggerDisplay("Name = {" + nameof(Name) + "}, Type = {" + nameof(Type) + "}")]
public record BankAccount
{
    /// <summary>
    /// Gets the unique URI identifier for this bank account.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this bank account in the FreeAgent system.
    /// This value is assigned by the API upon creation and is used to reference the account in other resources.
    /// </value>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the type of bank account.
    /// </summary>
    /// <value>
    /// One of "StandardBankAccount" (default), "PaypalAccount", or "CreditCardAccount".
    /// This field is required when creating a bank account.
    /// </value>
    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Type { get; init; }

    /// <summary>
    /// Gets the display name for this bank account.
    /// </summary>
    /// <value>
    /// A user-friendly name to identify the account. This field is required when creating a bank account.
    /// </value>
    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; init; }

    /// <summary>
    /// Gets the nominal ledger code for this account.
    /// </summary>
    /// <value>
    /// The accounting nominal code used for categorization in the chart of accounts.
    /// </value>
    [JsonPropertyName("nominal_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? NominalCode { get; init; }

    /// <summary>
    /// Gets the bank account number.
    /// </summary>
    /// <value>
    /// The account number. For StandardBankAccount, this is the full account number.
    /// For CreditCardAccount, only the last four digits are stored.
    /// </value>
    [JsonPropertyName("account_number")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? AccountNumber { get; init; }

    /// <summary>
    /// Gets the primary sort code for the bank account.
    /// </summary>
    /// <value>
    /// The bank sort code (routing number). Applicable to StandardBankAccount types.
    /// </value>
    [JsonPropertyName("sort_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SortCode { get; init; }

    /// <summary>
    /// Gets the secondary sort code for the bank account.
    /// </summary>
    /// <value>
    /// An additional sort code field for accounts requiring multiple routing identifiers.
    /// </value>
    [JsonPropertyName("secondary_sort_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SecondarySortCode { get; init; }

    /// <summary>
    /// Gets the International Bank Account Number (IBAN).
    /// </summary>
    /// <value>
    /// The IBAN for international bank transfers. Applicable to StandardBankAccount types.
    /// </value>
    [JsonPropertyName("iban")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Iban { get; init; }

    /// <summary>
    /// Gets the Bank Identifier Code (BIC/SWIFT).
    /// </summary>
    /// <value>
    /// The BIC or SWIFT code for international transfers. Applicable to StandardBankAccount types.
    /// </value>
    [JsonPropertyName("bic")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Bic { get; init; }

    /// <summary>
    /// Gets the email address for PayPal accounts.
    /// </summary>
    /// <value>
    /// The PayPal login email address. This field is required when creating or updating a PaypalAccount type.
    /// </value>
    [JsonPropertyName("email")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Email { get; init; }

    /// <summary>
    /// Gets the opening balance when the account was added to FreeAgent.
    /// </summary>
    /// <value>
    /// The initial account balance. Use zero for accounts opened after the FreeAgent start date.
    /// </value>
    [JsonPropertyName("opening_balance")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? OpeningBalance { get; init; }

    /// <summary>
    /// Gets the current account balance.
    /// </summary>
    /// <value>
    /// The latest calculated balance based on all transactions.
    /// </value>
    [JsonPropertyName("current_balance")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? CurrentBalance { get; init; }

    /// <summary>
    /// Gets the date of the latest transaction activity.
    /// </summary>
    /// <value>
    /// The date of the most recent transaction activity in YYYY-MM-DD format, or <see langword="null"/> if no activity exists.
    /// </value>
    [JsonPropertyName("latest_activity_date")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LatestActivityDate { get; init; }

    /// <summary>
    /// Gets the name of the financial institution.
    /// </summary>
    /// <value>
    /// The bank or financial institution name. This field is required when creating a bank account.
    /// </value>
    [JsonPropertyName("bank_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? BankName { get; init; }

    /// <summary>
    /// Gets the bank code identifier.
    /// </summary>
    /// <value>
    /// A code identifying the specific bank or financial institution (e.g., "generic", "barclays", "natwest", "hsbc").
    /// </value>
    [JsonPropertyName("bank_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? BankCode { get; init; }

    /// <summary>
    /// Gets the currency for this bank account.
    /// </summary>
    /// <value>
    /// A three-letter ISO 4217 currency code (e.g., "GBP", "USD", "EUR"). Defaults to the company's native currency.
    /// This becomes immutable once transactions exist in the account.
    /// </value>
    [JsonPropertyName("currency")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Currency { get; init; }

    /// <summary>
    /// Gets a value indicating whether this is the primary bank account.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if this is designated as the primary account for the company; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("is_primary")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsPrimary { get; init; }

    /// <summary>
    /// Gets the status of the bank account.
    /// </summary>
    /// <value>
    /// Either "active" or "hidden". Hidden accounts are archived but historical data is retained.
    /// </value>
    [JsonPropertyName("status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Status { get; init; }

    /// <summary>
    /// Gets a value indicating whether this is a personal account.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if this account is for personal use rather than business; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("is_personal")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsPersonal { get; init; }

    /// <summary>
    /// Gets the identifier for the connected bank feed.
    /// </summary>
    /// <value>
    /// The bank feed integration identifier, if automatic transaction import is configured.
    /// </value>
    [JsonPropertyName("bank_feed_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? BankFeedId { get; init; }

    /// <summary>
    /// Gets the status of the bank feed connection.
    /// </summary>
    /// <value>
    /// The current state of the bank feed integration (e.g., active, disconnected, error).
    /// </value>
    [JsonPropertyName("bank_feed_status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? BankFeedStatus { get; init; }

    /// <summary>
    /// Gets a value indicating whether the bank guess feature is enabled.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if automatic transaction categorization based on bank descriptions is enabled; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("bank_guess_enabled")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? BankGuessEnabled { get; init; }

    /// <summary>
    /// Gets the date and time when this bank account was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing the creation timestamp in UTC.
    /// </value>
    [JsonPropertyName("created_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this bank account was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing the last modification timestamp in UTC.
    /// </value>
    [JsonPropertyName("updated_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? UpdatedAt { get; init; }
}