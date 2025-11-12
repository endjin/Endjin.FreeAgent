// <copyright file="WebhookRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record WebhookRoot
{
    [JsonPropertyName("webhook")]
    public Webhook? Webhook { get; init; }
}