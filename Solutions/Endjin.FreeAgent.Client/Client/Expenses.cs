// <copyright file="Expenses.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

public class Expenses
{
    private const string ExpensesEndPoint = "v2/expenses";
    private readonly FreeAgentClient freeAgentClient;


    public Expenses(FreeAgentClient freeAgentClient)
    {
        this.freeAgentClient = freeAgentClient;
    }

    public async Task<Expense> CreateAsync(Expense expense)
    {
        ExpenseRoot root = new() { Expense = expense };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(new Uri(this.freeAgentClient.ApiBaseUrl, ExpensesEndPoint), content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        ExpenseRoot? result = await response.Content.ReadFromJsonAsync<ExpenseRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.Expense ?? throw new InvalidOperationException("Failed to deserialize expense response.");
    }
}