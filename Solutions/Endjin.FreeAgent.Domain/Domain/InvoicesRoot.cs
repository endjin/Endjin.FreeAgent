// <copyright file="InvoicesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="Invoice"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple invoice objects.
/// </remarks>
/// <seealso cref="Invoice"/>
public record InvoicesRoot
{
    /// <summary>
    /// Gets the collection of invoices from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="Invoice"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("invoices")]
    public List<Invoice> Invoices { get; init; } = [];
}