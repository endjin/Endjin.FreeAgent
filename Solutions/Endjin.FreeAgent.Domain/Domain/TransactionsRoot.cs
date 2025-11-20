// <copyright file="TransactionsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="Transaction"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple transaction objects.
/// </remarks>
/// <seealso cref="Transaction"/>
public record TransactionsRoot
{
    /// <summary>
    /// Gets the collection of transactions from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="Transaction"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("transactions")]
    public List<Transaction>? Transactions { get; init; }
}
