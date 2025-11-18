// <copyright file="EmailAddressesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of verified sender email addresses.
/// </summary>
/// <remarks>
/// <para>
/// This wrapper type is used for JSON deserialization of the FreeAgent API response from GET /v2/email_addresses.
/// </para>
/// <para>
/// Email addresses are returned as strings in the format "Name &lt;email@domain.com&gt;" and represent
/// verified sender email addresses that can be used as the "from" address when sending invoices, estimates,
/// or credit notes.
/// </para>
/// <para>
/// API Endpoint: GET /v2/email_addresses
/// Minimum Access Level: Time
/// </para>
/// </remarks>
/// <seealso cref="InvoiceEmail"/>
/// <seealso cref="EstimateEmail"/>
public record EmailAddressesRoot
{
    /// <summary>
    /// Gets the collection of verified sender email addresses from the API response.
    /// </summary>
    /// <value>
    /// A list of strings where each entry is a verified sender email address in the format "Name &lt;email@domain.com&gt;".
    /// These can be used as the sender address when emailing invoices, estimates, or credit notes.
    /// </value>
    [JsonPropertyName("email_addresses")]
    public List<string> EmailAddresses { get; init; } = [];
}