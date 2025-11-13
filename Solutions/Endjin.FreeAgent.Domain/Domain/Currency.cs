// <copyright file="Currency.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a currency in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Currency objects provide information about supported currencies for multi-currency transactions.
/// Each currency includes its ISO 4217 code, display name, and symbol for presentation purposes.
/// </para>
/// <para>
/// FreeAgent supports transactions in multiple currencies with automatic exchange rate handling
/// for foreign currency invoices, bills, and expenses.
/// </para>
/// <para>
/// API Endpoint: /v2/currencies
/// </para>
/// </remarks>
/// <seealso cref="Invoice"/>
/// <seealso cref="BankAccount"/>
public record Currency
{
    /// <summary>
    /// Gets the ISO 4217 currency code.
    /// </summary>
    /// <value>
    /// A three-letter currency code (e.g., "GBP", "USD", "EUR") conforming to the ISO 4217 standard.
    /// </value>
    [JsonPropertyName("code")]
    public string? Code { get; init; }

    /// <summary>
    /// Gets the full name of the currency.
    /// </summary>
    /// <value>
    /// The human-readable name of the currency (e.g., "British Pound", "US Dollar", "Euro").
    /// </value>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Gets the currency symbol.
    /// </summary>
    /// <value>
    /// The symbol used to represent this currency (e.g., "£", "$", "€") for display purposes.
    /// </value>
    [JsonPropertyName("symbol")]
    public string? Symbol { get; init; }
}