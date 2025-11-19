// <copyright file="BankFeed.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a bank feed connection in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Bank feeds automatically import transactions from a bank account into FreeAgent.
/// This resource allows you to inspect the status and details of these connections.
/// </para>
/// <para>
/// API Endpoint: /v2/bank_feeds
/// </para>
/// <para>
/// Minimum Access Level: Banking
/// </para>
/// </remarks>
public record BankFeed
{
    /// <summary>
    /// Gets the unique URI identifier for this bank feed.
    /// </summary>
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the URI of the bank account which the feed will import transactions into.
    /// </summary>
    [JsonPropertyName("bank_account")]
    public Uri? BankAccount { get; init; }

    /// <summary>
    /// Gets the current status of the feed.
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; init; }

    /// <summary>
    /// Gets the date and time when this feed was created.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this feed was last updated.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }

    /// <summary>
    /// Gets the type of the feed. Can be "api" or "open_banking".
    /// </summary>
    [JsonPropertyName("feed_type")]
    public string? FeedType { get; init; }

    /// <summary>
    /// Gets the display string for the name of the bank service this feed is connected to.
    /// </summary>
    [JsonPropertyName("bank_service_name")]
    public string? BankServiceName { get; init; }

    /// <summary>
    /// Gets the date and time at which the SCA (Strong Customer Authentication) will expire on an API Bank Feed.
    /// </summary>
    /// <remarks>
    /// Only provided for `api` feed types. This time may be in the past if the SCA has already expired.
    /// </remarks>
    [JsonPropertyName("sca_expires_at")]
    public DateTime? ScaExpiresAt { get; init; }
}
