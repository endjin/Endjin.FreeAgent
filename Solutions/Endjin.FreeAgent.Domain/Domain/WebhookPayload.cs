// <copyright file="WebhookPayload.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the payload (notification data) sent when a webhook is triggered in the FreeAgent system.
/// </summary>
/// <remarks>
/// <para>
/// When events occur in FreeAgent (such as an invoice being created or updated), configured webhooks send
/// HTTP POST requests to external URLs containing this payload. The payload provides information about what
/// event occurred and which resource was affected.
/// </para>
/// <para>
/// Webhook payloads are designed to be lightweight - they contain metadata about the event rather than the
/// full resource data. The receiving application should use the <see cref="ResourceUrl"/> to fetch the complete
/// resource details via the FreeAgent API if needed.
/// </para>
/// <para>
/// Common webhook events include:
/// <list type="bullet">
/// <item>create - A new resource was created</item>
/// <item>update - An existing resource was modified</item>
/// <item>delete - A resource was deleted (soft delete in most cases)</item>
/// </list>
/// </para>
/// <para>
/// Webhook requests include an HMAC-SHA256 signature in the X-FreeAgent-Signature header, which can be verified
/// using the webhook's secret to confirm the request genuinely came from FreeAgent. This prevents unauthorized
/// third parties from triggering webhook handlers.
/// </para>
/// <para>
/// Best practices for handling webhook payloads:
/// <list type="number">
/// <item>Always verify the HMAC signature before processing</item>
/// <item>Respond quickly (within 5 seconds) with a 2xx status code</item>
/// <item>Process the event asynchronously if it requires time-consuming operations</item>
/// <item>Handle duplicate notifications gracefully (use <see cref="Timestamp"/> and resource URL for idempotency)</item>
/// <item>Implement retry logic for failed API calls when fetching resource details</item>
/// </list>
/// </para>
/// </remarks>
/// <seealso cref="Webhook"/>
public record WebhookPayload
{
    /// <summary>
    /// Gets the type of event that triggered this webhook notification.
    /// </summary>
    /// <value>
    /// The event type, typically one of: "create", "update", or "delete".
    /// This indicates what action occurred on the resource.
    /// </value>
    [JsonPropertyName("event")]
    public string? Event { get; init; }

    /// <summary>
    /// Gets the type of resource that was affected by this event.
    /// </summary>
    /// <value>
    /// The resource type name (e.g., "Invoice", "Contact", "BankTransaction", "Expense").
    /// This identifies which type of FreeAgent object the event relates to.
    /// </value>
    [JsonPropertyName("resource")]
    public string? Resource { get; init; }

    /// <summary>
    /// Gets the API URL for the specific resource instance that was affected.
    /// </summary>
    /// <value>
    /// The full API URL that can be used to fetch complete details about the affected resource.
    /// For example: "https://api.freeagent.com/v2/invoices/12345". Use this URL with appropriate
    /// authentication to retrieve the current state of the resource.
    /// </value>
    [JsonPropertyName("resource_url")]
    public Uri? ResourceUrl { get; init; }

    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    /// <value>
    /// The timestamp of when the event was triggered in FreeAgent. This can be used for:
    /// <list type="bullet">
    /// <item>Ordering events chronologically</item>
    /// <item>Implementing idempotency checks to avoid processing duplicate notifications</item>
    /// <item>Auditing and logging webhook activity</item>
    /// <item>Filtering out stale notifications if webhook delivery was delayed</item>
    /// </list>
    /// </value>
    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp { get; init; }
}