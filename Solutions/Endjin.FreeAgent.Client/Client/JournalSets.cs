// <copyright file="JournalSets.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class JournalSets
{
    private const string JournalSetsEndPoint = "v2/journal_sets";
    private readonly FreeAgentClient freeAgentClient;


    public JournalSets(FreeAgentClient client)
    {
        this.freeAgentClient = client;
    }

    public async Task<bool> CreateAsync(JournalSet journalSet)
    {
        JournalSetRoot root = new() { JournalSet = journalSet };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(new Uri(this.freeAgentClient.ApiBaseUrl, JournalSetsEndPoint), content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        return true;
    }
}
