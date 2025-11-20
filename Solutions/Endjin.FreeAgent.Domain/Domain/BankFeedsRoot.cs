// <copyright file="BankFeedsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>
/// Root object for list of bank feeds responses.
/// </summary>
public record BankFeedsRoot
{
    /// <summary>
    /// Gets the list of bank feeds.
    /// </summary>
    [JsonPropertyName("bank_feeds")]
    public List<BankFeed>? BankFeeds { get; init; }
}
