// <copyright file="PriceListItemRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a <see cref="Domain.PriceListItem"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single price list item.
/// </remarks>
/// <seealso cref="PriceListItem"/>
public record PriceListItemRoot
{
    /// <summary>
    /// Gets the price list item from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.PriceListItem"/> object returned by the API.
    /// </value>
    [JsonPropertyName("price_list_item")]
    public PriceListItem? PriceListItem { get; init; }
}
