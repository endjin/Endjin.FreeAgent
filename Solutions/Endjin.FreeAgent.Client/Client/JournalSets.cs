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
/// transactions. This service supports creating journal sets with their associated journal entries.
/// </para>
/// <para>
/// Journal sets are not cached as they are typically created once and not frequently retrieved. Each journal
/// set must balance (total debits must equal total credits) and is associated with a specific date.
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
    /// Creates a new journal set in FreeAgent.
    /// </summary>
    /// <param name="journalSet">The <see cref="JournalSet"/> object containing the journal set details and entries to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, returning true if the
    /// journal set was created successfully.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls POST /v2/journal_sets. The journal set must contain balanced journal entries
    /// where total debits equal total credits. Journal sets are used for manual accounting adjustments
    /// and corrections that cannot be made through standard transactions.
    /// </remarks>
    public async Task<bool> CreateAsync(JournalSet journalSet)
    {
        JournalSetRoot root = new() { JournalSet = journalSet };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(new Uri(this.freeAgentClient.ApiBaseUrl, JournalSetsEndPoint), content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        return true;
    }
}