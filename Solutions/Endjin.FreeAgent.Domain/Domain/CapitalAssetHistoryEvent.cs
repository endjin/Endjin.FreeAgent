// <copyright file="CapitalAssetHistoryEvent.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a lifecycle event in the history of a capital asset.
/// </summary>
/// <remarks>
/// <para>
/// Capital asset history events track significant moments in an asset's lifecycle including
/// purchase, depreciation calculations, tax allowance claims, and disposal. These events
/// provide an audit trail for accounting and tax compliance purposes.
/// </para>
/// <para>
/// History events are only returned when the <c>include_history=true</c> query parameter
/// is specified on capital asset API requests.
/// </para>
/// </remarks>
/// <seealso cref="CapitalAsset"/>
public record CapitalAssetHistoryEvent
{
    /// <summary>
    /// Gets the type of lifecycle event.
    /// </summary>
    /// <value>
    /// One of: "purchase", "depreciation", "annual_investment_allowance", or "disposal".
    /// </value>
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    /// <summary>
    /// Gets the description of the event.
    /// </summary>
    /// <value>
    /// A human-readable description explaining what occurred in this event.
    /// </value>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the date when the event occurred.
    /// </summary>
    /// <value>
    /// The event date in YYYY-MM-DD format.
    /// </value>
    [JsonPropertyName("date")]
    public DateOnly? Date { get; init; }

    /// <summary>
    /// Gets the transaction value associated with this event.
    /// </summary>
    /// <value>
    /// The monetary value for the event (purchase price, depreciation amount, allowance claimed, etc.).
    /// </value>
    [JsonPropertyName("value")]
    public decimal? Value { get; init; }

    /// <summary>
    /// Gets the tax value component of this event.
    /// </summary>
    /// <value>
    /// The tax-related value for this event, such as the tax relief claimed.
    /// </value>
    [JsonPropertyName("tax_value")]
    public decimal? TaxValue { get; init; }

    /// <summary>
    /// Gets the URI link to the related transaction.
    /// </summary>
    /// <value>
    /// A URI reference to the associated Bill, Bank Transaction Explanation, or Expense that triggered this event.
    /// </value>
    [JsonPropertyName("link")]
    public Uri? Link { get; init; }
}
