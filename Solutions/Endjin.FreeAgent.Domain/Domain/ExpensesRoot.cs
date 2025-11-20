// <copyright file="ExpensesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="Expense"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple expense objects.
/// </remarks>
/// <seealso cref="Expense"/>
public record ExpensesRoot
{
    /// <summary>
    /// Gets the collection of expense entries from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="Expense"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("expenses")]
    public List<Expense>? Expenses { get; init; }
}