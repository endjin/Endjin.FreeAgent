// <copyright file="WebhooksRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="Webhook"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple webhooks.
/// </remarks>
/// <seealso cref="Webhook"/>
public record WebhooksRoot
{
    /// <summary>
    /// Gets the collection of webhooks from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="Webhook"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("webhooks")]
    public List<Webhook>? Webhooks { get; init; }
}