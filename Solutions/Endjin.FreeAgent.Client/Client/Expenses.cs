// <copyright file="Expenses.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing expenses via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent expenses, which represent business-related costs
/// incurred by employees or the business itself. Expenses can include mileage, receipts, and other
/// claimable costs.
/// </para>
/// <para>
/// Note: This service does not use caching as expense operations are typically transactional in nature.
/// </para>
/// </remarks>
/// <seealso cref="Expense"/>
/// <seealso cref="Category"/>
public class Expenses
{
    private const string ExpensesEndPoint = "v2/expenses";
    private readonly FreeAgentClient freeAgentClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="Expenses"/> class.
    /// </summary>
    /// <param name="freeAgentClient">The FreeAgent HTTP client for making API requests.</param>
    public Expenses(FreeAgentClient freeAgentClient)
    {
        this.freeAgentClient = freeAgentClient;
    }

    /// <summary>
    /// Creates a new expense in FreeAgent.
    /// </summary>
    /// <param name="expense">The <see cref="Expense"/> object containing the expense details to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="Expense"/> object with server-assigned values (e.g., ID, URL).
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/expenses to create a new expense. Expenses can be associated with
    /// users, projects, and categories for proper accounting and expense tracking.
    /// </remarks>
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