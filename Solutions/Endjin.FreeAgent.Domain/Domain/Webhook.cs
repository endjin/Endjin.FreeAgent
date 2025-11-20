// <copyright file="Webhook.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a webhook subscription in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Webhooks enable real-time integration between FreeAgent and external applications by sending HTTP POST
/// notifications when specific events occur. Instead of repeatedly polling the API for changes, applications
/// can register webhooks to be notified immediately when relevant data changes.
/// </para>
/// <para>
/// Webhook capabilities:
/// - Subscribe to multiple event types (invoice created, contact updated, etc.)
/// - Receive notifications at a specified HTTPS endpoint
/// - Validate authenticity using HMAC signatures with a shared secret
/// - Enable or disable notifications without deleting the webhook
/// - Support real-time synchronization and automated workflows
/// </para>
/// <para>
/// Webhooks are essential for building integrations that need to respond quickly to changes in FreeAgent,
/// such as syncing invoices to external billing systems, triggering workflows when expenses are approved,
/// or updating CRM systems when contacts are modified.
/// </para>
/// <para>
/// API Endpoint: /v2/webhooks
/// </para>
/// <para>
/// Minimum Access Level: Full Access
/// </para>
/// </remarks>
/// <seealso cref="WebhookPayload"/>
public record Webhook
{
    /// <summary>
    /// Gets the unique URI identifier for this webhook subscription.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this webhook in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the list of event types this webhook is subscribed to.
    /// </summary>
    /// <value>
    /// A list of event names such as "Invoice.created", "Contact.updated", "Expense.approved"
    /// that will trigger webhook notifications. When any of these events occur, FreeAgent will
    /// send an HTTP POST to the payload URL.
    /// </value>
    [JsonPropertyName("events")]
    public List<string>? Events { get; init; }

    /// <summary>
    /// Gets the HTTPS endpoint URL where webhook notifications will be sent.
    /// </summary>
    /// <value>
    /// The fully qualified HTTPS URL of the external application endpoint that will receive
    /// POST requests containing event data when subscribed events occur.
    /// </value>
    [JsonPropertyName("payload_url")]
    public Uri? PayloadUrl { get; init; }

    /// <summary>
    /// Gets the current status of this webhook.
    /// </summary>
    /// <value>
    /// The webhook status such as "Active" or "Inactive". Inactive webhooks do not send notifications
    /// but remain configured for easy re-activation.
    /// </value>
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    /// <summary>
    /// Gets the shared secret used for HMAC signature verification.
    /// </summary>
    /// <value>
    /// A secret string used to generate HMAC-SHA256 signatures included in webhook requests,
    /// allowing the receiving application to verify that notifications genuinely come from FreeAgent.
    /// </value>
    [JsonPropertyName("secret")]
    public string? Secret { get; init; }

    /// <summary>
    /// Gets the date and time when this webhook was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing when this webhook subscription was first set up.
    /// </value>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this webhook was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the last time this webhook configuration was modified.
    /// </value>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; init; }
}