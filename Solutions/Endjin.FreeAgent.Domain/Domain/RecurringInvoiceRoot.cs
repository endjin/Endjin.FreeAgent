// <copyright file="RecurringInvoiceRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a <see cref="Domain.RecurringInvoice"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single recurring invoice.
/// </remarks>
/// <seealso cref="RecurringInvoice"/>
public record RecurringInvoiceRoot
{
    /// <summary>
    /// Gets the recurring invoice from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.RecurringInvoice"/> object returned by the API.
    /// </value>
    [JsonPropertyName("recurring_invoice")]
    public RecurringInvoice? RecurringInvoice { get; init; }
}