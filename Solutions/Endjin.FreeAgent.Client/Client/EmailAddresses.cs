// <copyright file="EmailAddresses.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Client;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Caching.Memory;

/// <summary>
/// Provides methods for accessing verified sender email addresses via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service provides read-only access to the list of verified sender email addresses.
/// These addresses can be used as the "from" address when sending invoices, estimates, or credit notes
/// via the FreeAgent API.
/// </para>
/// <para>
/// Results are cached for 1 hour as the list of verified email addresses changes infrequently.
/// </para>
/// </remarks>
/// <seealso cref="InvoiceEmail"/>
/// <seealso cref="EstimateEmail"/>
public class EmailAddresses
{
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailAddresses"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing email address data.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="client"/> or <paramref name="cache"/> is <see langword="null"/>.
    /// </exception>
    public EmailAddresses(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Retrieves all verified sender email addresses from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// strings where each entry is a verified sender email address in the format "Name &lt;email@domain.com&gt;".
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// <para>
    /// This method calls GET /v2/email_addresses and caches the result for 1 hour.
    /// </para>
    /// <para>
    /// The returned email addresses can be used as the "from" field when sending invoices, estimates,
    /// or credit notes via the <see cref="InvoiceEmail"/>, <see cref="EstimateEmail"/>, or credit note
    /// email endpoints.
    /// </para>
    /// <para>
    /// Minimum access level required: Time
    /// </para>
    /// </remarks>
    public async Task<IEnumerable<string>> GetAllAsync()
    {
        await this.client.InitializeAndAuthorizeAsync();

        string cacheKey = "email_addresses_all";

        if (this.cache.TryGetValue(cacheKey, out IEnumerable<string>? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(
            new Uri(this.client.ApiBaseUrl, "/v2/email_addresses"));
        response.EnsureSuccessStatusCode();

        EmailAddressesRoot? root = await response.Content.ReadFromJsonAsync<EmailAddressesRoot>(
            SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        IEnumerable<string> emailAddresses = root?.EmailAddresses ?? [];

        this.cache.Set(cacheKey, emailAddresses, TimeSpan.FromHours(1));

        return emailAddresses;
    }
}