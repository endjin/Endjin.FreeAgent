// <copyright file="BankAccountDetails.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Text.Json.Serialization;

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents bank account details for payment processing in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Bank account details contain the information customers need to make direct bank transfer payments.
/// This includes both domestic banking details (account number and sort code for UK banks) and
/// international banking details (IBAN and BIC for European and international transfers).
/// </para>
/// <para>
/// These details appear on invoices and estimates sent to customers, providing the necessary information
/// for customers to pay by bank transfer.
/// </para>
/// </remarks>
/// <seealso cref="PaymentMethods"/>
/// <seealso cref="Company"/>
public record BankAccountDetails
{
    /// <summary>
    /// Gets the name of the bank.
    /// </summary>
    /// <value>
    /// The bank name such as "Barclays", "HSBC", "Lloyds Bank", etc.
    /// </value>
    [JsonPropertyName("bank_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? BankName { get; init; }

    /// <summary>
    /// Gets the bank account number.
    /// </summary>
    /// <value>
    /// The domestic account number, typically 8 digits for UK bank accounts.
    /// </value>
    [JsonPropertyName("account_number")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? AccountNumber { get; init; }

    /// <summary>
    /// Gets the bank sort code.
    /// </summary>
    /// <value>
    /// The sort code identifying the bank branch, typically in the format "12-34-56" for UK banks.
    /// </value>
    [JsonPropertyName("sort_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SortCode { get; init; }

    /// <summary>
    /// Gets the International Bank Account Number (IBAN).
    /// </summary>
    /// <value>
    /// The IBAN for international transfers, consisting of a country code, check digits, and bank account details.
    /// Required for SEPA transfers and international payments.
    /// </value>
    [JsonPropertyName("iban")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Iban { get; init; }

    /// <summary>
    /// Gets the Bank Identifier Code (BIC), also known as SWIFT code.
    /// </summary>
    /// <value>
    /// The BIC/SWIFT code identifying the bank for international transfers, typically 8 or 11 alphanumeric characters.
    /// </value>
    [JsonPropertyName("bic")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Bic { get; init; }
}