// <copyright file="WebhookRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a <see cref="Domain.Webhook"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single webhook configuration.
/// </remarks>
/// <seealso cref="Webhook"/>
public record WebhookRoot
{
    /// <summary>
    /// Gets the webhook from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.Webhook"/> object returned by the API.
    /// </value>
    [JsonPropertyName("webhook")]
    public Webhook? Webhook { get; init; }
}