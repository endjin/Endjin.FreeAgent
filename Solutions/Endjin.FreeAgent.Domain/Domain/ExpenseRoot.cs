// <copyright file="ExpenseRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a single <see cref="Domain.Expense"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single expense object.
/// </remarks>
/// <seealso cref="Expense"/>
public record ExpenseRoot
{
    /// <summary>
    /// Gets the expense from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.Expense"/> object returned by the API.
    /// </value>
    [JsonPropertyName("expense")]
    public Expense? Expense { get; init; }
}