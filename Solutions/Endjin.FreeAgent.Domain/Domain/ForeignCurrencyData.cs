// <copyright file="ForeignCurrencyData.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents foreign currency data for a transaction in a non-native currency.
/// </summary>
/// <remarks>
/// <para>
/// This object is present only when a transaction involves a currency different from
/// the company's native currency. It contains the original currency code and the
/// transaction amount in that foreign currency.
/// </para>
/// </remarks>
/// <seealso cref="Transaction"/>
[DebuggerDisplay("CurrencyCode = {" + nameof(CurrencyCode) + "}, DebitValue = {" + nameof(DebitValue) + "}")]
public record ForeignCurrencyData
{
    /// <summary>
    /// Gets the ISO currency code for the foreign currency.
    /// </summary>
    /// <value>
    /// A three-letter ISO 4217 currency code (e.g., "USD", "EUR", "GBP").
    /// </value>
    [JsonPropertyName("currency_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CurrencyCode { get; init; }

    /// <summary>
    /// Gets the transaction amount in the foreign currency.
    /// </summary>
    /// <value>
    /// The debit value in the foreign currency. Negative values indicate credits.
    /// </value>
    [JsonPropertyName("debit_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? DebitValue { get; init; }
}
