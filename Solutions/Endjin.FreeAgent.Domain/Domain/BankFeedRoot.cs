// <copyright file="BankFeedRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

using System.Text.Json.Serialization;

/// <summary>
/// Root object for single bank feed responses.
/// </summary>
public record BankFeedRoot
{
    /// <summary>
    /// Gets the bank feed.
    /// </summary>
    [JsonPropertyName("bank_feed")]
    public BankFeed? BankFeed { get; init; }
}
