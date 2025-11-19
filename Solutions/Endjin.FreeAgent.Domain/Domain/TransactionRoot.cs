// <copyright file="TransactionRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a single <see cref="Domain.Transaction"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single transaction object.
/// </remarks>
/// <seealso cref="Transaction"/>
public record TransactionRoot
{
    /// <summary>
    /// Gets the transaction from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.Transaction"/> object returned by the API.
    /// </value>
    [JsonPropertyName("transaction")]
    public Transaction? Transaction { get; init; }
}
