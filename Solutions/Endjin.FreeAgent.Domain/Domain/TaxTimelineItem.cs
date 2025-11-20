// <copyright file="TaxTimelineItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a tax timeline event in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Tax timeline items represent upcoming tax obligations and deadlines including VAT returns,
/// Companies House filings, Corporation Tax deadlines, and other compliance events.
/// </para>
/// <para>
/// API Endpoint: /v2/company/tax_timeline
/// </para>
/// <para>
/// Minimum Access Level: Tax, Accounting and Users
/// </para>
/// </remarks>
[DebuggerDisplay("Description = {" + nameof(Description) + "}, DatedOn = {" + nameof(DatedOn) + "}")]
public record TaxTimelineItem
{
    /// <summary>
    /// Gets the description of the tax event.
    /// </summary>
    /// <value>
    /// The event name, such as "VAT Return 09 11" or "Corporation Tax Return Due".
    /// </value>
    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the nature or type of the tax event.
    /// </summary>
    /// <value>
    /// The event type, such as "Electronic Submission and Payment Due" or "Filing Due".
    /// </value>
    [JsonPropertyName("nature")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Nature { get; init; }

    /// <summary>
    /// Gets the date when the tax event is due.
    /// </summary>
    /// <value>
    /// The date of the tax obligation or deadline.
    /// </value>
    [JsonPropertyName("dated_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? DatedOn { get; init; }

    /// <summary>
    /// Gets the amount due for this tax event.
    /// </summary>
    /// <value>
    /// The financial amount associated with this event, if applicable.
    /// </value>
    [JsonPropertyName("amount_due")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? AmountDue { get; init; }

    /// <summary>
    /// Gets a value indicating whether this event is personal (as opposed to business-related).
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the event is personal; <see langword="false"/> if it is business-related.
    /// </value>
    [JsonPropertyName("is_personal")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsPersonal { get; init; }
}
