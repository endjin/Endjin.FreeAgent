// <copyright file="PriceListItemsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="PriceListItem"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple price list items.
/// </remarks>
/// <seealso cref="PriceListItem"/>
public record PriceListItemsRoot
{
    /// <summary>
    /// Gets the collection of price list items from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="PriceListItem"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("price_list_items")]
    public List<PriceListItem>? PriceListItems { get; init; }
}
