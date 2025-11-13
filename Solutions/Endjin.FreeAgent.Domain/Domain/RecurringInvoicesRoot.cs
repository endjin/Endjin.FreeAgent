// <copyright file="RecurringInvoicesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="RecurringInvoice"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple recurring invoices.
/// </remarks>
/// <seealso cref="RecurringInvoice"/>
public record RecurringInvoicesRoot
{
    /// <summary>
    /// Gets the collection of recurring invoices from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="RecurringInvoice"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("recurring_invoices")]
    public List<RecurringInvoice>? RecurringInvoices { get; init; }
}