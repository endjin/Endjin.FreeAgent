// <copyright file="JournalSets.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing journal sets via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent journal sets, which are collections of journal entries
/// used to record manual accounting adjustments. Journal sets allow accountants to post double-entry
/// bookkeeping transactions directly to the general ledger, such as adjusting entries, accruals, prepayments,
/// and other complex accounting transactions not captured through standard invoicing, bills, or banking
/// transactions.
/// </para>
/// <para>
/// Journal sets are not cached as they are typically created once and not frequently retrieved. Each journal
/// set must balance (total debits must equal total credits) and is associated with a specific date.
/// </para>
/// <para>
/// Minimum Access Level: Tax, Accounting and Users
/// </para>
/// </remarks>
/// <seealso cref="JournalSet"/>
/// <seealso cref="JournalEntry"/>
public class JournalSets
{
    private const string JournalSetsEndPoint = "v2/journal_sets";
    private readonly FreeAgentClient freeAgentClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="JournalSets"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    public JournalSets(FreeAgentClient client)
    {
        this.freeAgentClient = client;
    }

    /// <summary>
    /// Retrieves all journal sets from FreeAgent with optional filtering.
    /// </summary>
    /// <param name="fromDate">Optional filter to retrieve only journal sets dated on or after this date.</param>
    /// <param name="toDate">Optional filter to retrieve only journal sets dated on or before this date.</param>
    /// <param name="tag">Optional filter to retrieve only journal sets with this application tag.</param>
    /// <param name="updatedSince">Optional filter to retrieve only journal sets updated after this timestamp (ISO 8601 format).</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="JournalSet"/> objects matching the specified criteria.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/journal_sets with the specified query parameters and handles pagination automatically.
    /// </remarks>
    public async Task<IEnumerable<JournalSet>> GetAllAsync(
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        string? tag = null,
        DateTimeOffset? updatedSince = null)
    {
        List<string> queryParams = [];

        if (fromDate.HasValue)
        {
            queryParams.Add($"from_date={fromDate.Value:yyyy-MM-dd}");
        }

        if (toDate.HasValue)
        {
            queryParams.Add($"to_date={toDate.Value:yyyy-MM-dd}");
        }

        if (!string.IsNullOrEmpty(tag))
        {
            queryParams.Add($"tag={Uri.EscapeDataString(tag)}");
        }

        if (updatedSince.HasValue)
        {
            queryParams.Add($"updated_since={Uri.EscapeDataString(updatedSince.Value.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))}");
        }

        string queryString = queryParams.Count > 0 ? $"?{string.Join("&", queryParams)}" : string.Empty;

        List<JournalSetsRoot> response = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<JournalSetsRoot>(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{JournalSetsEndPoint}{queryString}"))
            .ConfigureAwait(false);

        return [.. response.SelectMany(x => x.JournalSets)];
    }

    /// <summary>
    /// Retrieves a specific journal set by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the journal set to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="JournalSet"/> object with the specified ID.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no journal set with the specified ID is found.</exception>
    /// <remarks>
    /// This method calls GET /v2/journal_sets/{id}.
    /// </remarks>
    public async Task<JournalSet> GetByIdAsync(string id)
    {
        await this.freeAgentClient.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{JournalSetsEndPoint}/{id}")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        JournalSetRoot? root = await response.Content.ReadFromJsonAsync<JournalSetRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return root?.JournalSet ?? throw new InvalidOperationException($"Journal set with ID {id} not found.");
    }

    /// <summary>
    /// Retrieves the opening balances journal set from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// opening balances <see cref="JournalSet"/> object.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the opening balances cannot be retrieved.</exception>
    /// <remarks>
    /// This method calls GET /v2/journal_sets/opening_balances to retrieve the special journal set
    /// used for migrating opening balances into FreeAgent.
    /// </remarks>
    public async Task<JournalSet> GetOpeningBalancesAsync()
    {
        await this.freeAgentClient.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{JournalSetsEndPoint}/opening_balances")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        JournalSetRoot? root = await response.Content.ReadFromJsonAsync<JournalSetRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return root?.JournalSet ?? throw new InvalidOperationException("Failed to retrieve opening balances.");
    }

    /// <summary>
    /// Creates a new journal set in FreeAgent.
    /// </summary>
    /// <param name="journalSet">The <see cref="JournalSet"/> object containing the journal set details and entries to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="JournalSet"/> object with server-assigned values (e.g., URL).
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/journal_sets. The journal set must contain balanced journal entries
    /// where total debits equal total credits. Journal sets are used for manual accounting adjustments
    /// and corrections that cannot be made through standard transactions.
    /// </remarks>
    public async Task<JournalSet> CreateAsync(JournalSet journalSet)
    {
        await this.freeAgentClient.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        JournalSetRoot root = new() { JournalSet = journalSet };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, JournalSetsEndPoint),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        JournalSetRoot? result = await response.Content.ReadFromJsonAsync<JournalSetRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.JournalSet ?? throw new InvalidOperationException("Failed to deserialize journal set response.");
    }

    /// <summary>
    /// Updates an existing journal set in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the journal set to update.</param>
    /// <param name="journalSet">The <see cref="JournalSet"/> object containing the updated journal set details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="JournalSet"/> object as returned by the API.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// <para>
    /// This method calls PUT /v2/journal_sets/{id} to update the journal set. The updated journal entries
    /// must still balance (total debits equal total credits).
    /// </para>
    /// <para>
    /// To remove journal entries during an update, set the <see cref="JournalEntry.Destroy"/> property to true
    /// for the entries to be removed. Tagged journal sets cannot be edited by users in the FreeAgent app.
    /// </para>
    /// </remarks>
    public async Task<JournalSet> UpdateAsync(string id, JournalSet journalSet)
    {
        await this.freeAgentClient.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        JournalSetRoot root = new() { JournalSet = journalSet };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{JournalSetsEndPoint}/{id}"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        JournalSetRoot? result = await response.Content.ReadFromJsonAsync<JournalSetRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.JournalSet ?? throw new InvalidOperationException("Failed to deserialize journal set response.");
    }

    /// <summary>
    /// Deletes a journal set from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the journal set to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls DELETE /v2/journal_sets/{id} to delete the journal set and all its associated
    /// journal entries.
    /// </remarks>
    public async Task DeleteAsync(string id)
    {
        await this.freeAgentClient.InitializeAndAuthorizeAsync().ConfigureAwait(false);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.DeleteAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{JournalSetsEndPoint}/{id}")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
    }
}