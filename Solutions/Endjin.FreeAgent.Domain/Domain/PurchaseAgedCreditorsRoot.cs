// <copyright file="PurchaseAgedCreditorsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a <see cref="Domain.PurchaseAgedCreditors"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a purchase aged creditors report.
/// </remarks>
/// <seealso cref="PurchaseAgedCreditors"/>
public record PurchaseAgedCreditorsRoot
{
    /// <summary>
    /// Gets the purchase aged creditors report from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.PurchaseAgedCreditors"/> object returned by the API.
    /// </value>
    [JsonPropertyName("purchase_aged_creditors")]
    public PurchaseAgedCreditors? PurchaseAgedCreditors { get; init; }
}