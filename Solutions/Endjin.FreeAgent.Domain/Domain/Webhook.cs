// <copyright file="Webhook.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record Webhook
{
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    [JsonPropertyName("events")]
    public List<string>? Events { get; init; }

    [JsonPropertyName("payload_url")]
    public Uri? PayloadUrl { get; init; }

    [JsonPropertyName("status")]
    public string? Status { get; init; }

    [JsonPropertyName("secret")]
    public string? Secret { get; init; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; init; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }
}