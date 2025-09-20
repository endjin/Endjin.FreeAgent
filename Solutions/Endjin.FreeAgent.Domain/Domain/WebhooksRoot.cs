// <copyright file="WebhooksRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record WebhooksRoot
{
    [JsonPropertyName("webhooks")]
    public List<Webhook>? Webhooks { get; init; }
}