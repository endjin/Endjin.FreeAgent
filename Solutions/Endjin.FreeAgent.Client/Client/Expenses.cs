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
/// Results are cached for 5 minutes to improve performance. Cache entries are invalidated automatically
/// when expense records are created, updated, or deleted.
/// </para>
/// </remarks>
/// <seealso cref="Expense"/>
/// <seealso cref="Category"/>
public class Expenses
{
    private const string ExpensesEndPoint = "v2/expenses";
    private readonly FreeAgentClient client;
    private readonly IMemoryCache cache;
    private readonly HashSet<string> expenseCacheKeys = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="Expenses"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing expense data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> or <paramref name="cache"/> is null.</exception>
    public Expenses(FreeAgentClient client, IMemoryCache cache)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Creates a new expense in FreeAgent.
    /// </summary>
    /// <param name="expense">The <see cref="Expense"/> object containing the expense details to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="Expense"/> object with server-assigned values (e.g., ID, URL).
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="expense"/> is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/expenses to create a new expense. Expenses can be associated with
    /// users, projects, and categories for proper accounting and expense tracking. The cache is invalidated
    /// to ensure subsequent queries return up-to-date data.
    /// </remarks>
    public async Task<Expense> CreateAsync(Expense expense)
    {
        ArgumentNullException.ThrowIfNull(expense);
        await this.client.InitializeAndAuthorizeAsync();

        ExpenseRoot root = new() { Expense = expense };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);
        HttpResponseMessage response = await this.client.HttpClient.PostAsync(new Uri(this.client.ApiBaseUrl, $"/{ExpensesEndPoint}"), content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        ExpenseRoot? result = await response.Content.ReadFromJsonAsync<ExpenseRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.InvalidateExpenseCache();

        return result?.Expense ?? throw new InvalidOperationException("Failed to deserialize expense response.");
    }

    /// <summary>
    /// Creates multiple expenses in FreeAgent in a single request.
    /// </summary>
    /// <param name="expenses">A collection of <see cref="Expense"/> objects containing the expense details to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection
    /// of created <see cref="Expense"/> objects with server-assigned values (e.g., ID, URL).
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="expenses"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="expenses"/> is empty.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/expenses with an array of expenses for batch creation. This is more
    /// efficient than creating expenses individually when multiple expenses need to be created at once.
    /// The cache is invalidated to ensure subsequent queries return up-to-date data.
    /// </remarks>
    public async Task<IEnumerable<Expense>> CreateBatchAsync(IEnumerable<Expense> expenses)
    {
        ArgumentNullException.ThrowIfNull(expenses);

        List<Expense> expensesList = expenses.ToList();
        if (expensesList.Count == 0)
        {
            throw new ArgumentException("Expenses collection cannot be empty.", nameof(expenses));
        }

        await this.client.InitializeAndAuthorizeAsync();

        ExpensesRoot root = new() { Expenses = expensesList };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);
        HttpResponseMessage response = await this.client.HttpClient.PostAsync(new Uri(this.client.ApiBaseUrl, $"/{ExpensesEndPoint}"), content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        ExpensesRoot? result = await response.Content.ReadFromJsonAsync<ExpensesRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.InvalidateExpenseCache();

        return result?.Expenses ?? throw new InvalidOperationException("Failed to deserialize expenses response.");
    }

    /// <summary>
    /// Retrieves expenses from FreeAgent with optional filters.
    /// </summary>
    /// <param name="view">Optional view filter: "recent" for recent expenses or "recurring" for recurring expenses.</param>
    /// <param name="fromDate">Optional start date to filter expenses from this date onwards (YYYY-MM-DD).</param>
    /// <param name="toDate">Optional end date to filter expenses up to this date (YYYY-MM-DD).</param>
    /// <param name="updatedSince">Optional filter for expenses updated since the specified date-time (ISO 8601).</param>
    /// <param name="project">Optional project URI to filter expenses associated with a specific project.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="Expense"/> objects matching the specified filters.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/expenses with optional query parameters and caches the result for 5 minutes.
    /// Different filter combinations create separate cache entries.
    /// </remarks>
    public async Task<IEnumerable<Expense>> GetAllAsync(
        string? view = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        DateTimeOffset? updatedSince = null,
        Uri? project = null)
    {
        await this.client.InitializeAndAuthorizeAsync();

        List<string> queryParams = [];

        if (!string.IsNullOrWhiteSpace(view))
        {
            queryParams.Add($"view={Uri.EscapeDataString(view)}");
        }

        if (fromDate.HasValue)
        {
            queryParams.Add($"from_date={fromDate.Value:yyyy-MM-dd}");
        }

        if (toDate.HasValue)
        {
            queryParams.Add($"to_date={toDate.Value:yyyy-MM-dd}");
        }

        if (updatedSince.HasValue)
        {
            queryParams.Add($"updated_since={updatedSince.Value.UtcDateTime:O}");
        }

        if (project != null)
        {
            queryParams.Add($"project={Uri.EscapeDataString(project.ToString())}");
        }

        string url = $"/{ExpensesEndPoint}";
        if (queryParams.Count > 0)
        {
            url += "?" + string.Join("&", queryParams);
        }

        string cacheKey = $"expenses_{view ?? "all"}_{fromDate?.ToString("yyyyMMdd")}_{toDate?.ToString("yyyyMMdd")}_{updatedSince?.ToString("yyyyMMddHHmmss")}_{project?.ToString() ?? ""}";

        if (this.cache.TryGetValue(cacheKey, out IEnumerable<Expense>? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, url));
        response.EnsureSuccessStatusCode();

        ExpensesRoot? root = await response.Content.ReadFromJsonAsync<ExpensesRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        IEnumerable<Expense> expenses = root?.Expenses ?? [];

        this.cache.Set(cacheKey, expenses, TimeSpan.FromMinutes(5));
        this.expenseCacheKeys.Add(cacheKey);

        return expenses;
    }

    /// <summary>
    /// Retrieves a specific expense by its ID from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the expense to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="Expense"/> object with the specified ID.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no expense with the specified ID is found.</exception>
    /// <remarks>
    /// This method calls GET /v2/expenses/{id} and caches the result for 5 minutes.
    /// </remarks>
    public async Task<Expense> GetByIdAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        await this.client.InitializeAndAuthorizeAsync();

        string cacheKey = $"expense_{id}";

        if (this.cache.TryGetValue(cacheKey, out Expense? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, $"/{ExpensesEndPoint}/{id}"));
        response.EnsureSuccessStatusCode();

        ExpenseRoot? root = await response.Content.ReadFromJsonAsync<ExpenseRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        Expense expense = root?.Expense ?? throw new InvalidOperationException($"Expense {id} not found");

        this.cache.Set(cacheKey, expense, TimeSpan.FromMinutes(5));
        this.expenseCacheKeys.Add(cacheKey);

        return expense;
    }

    /// <summary>
    /// Updates an existing expense in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the expense to update.</param>
    /// <param name="expense">The <see cref="Expense"/> object containing the updated expense details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="Expense"/> object as returned by the API.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="expense"/> is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/expenses/{id} to update the expense. The cache entries for this
    /// expense and all expense queries are invalidated after a successful update.
    /// </remarks>
    public async Task<Expense> UpdateAsync(string id, Expense expense)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(expense);
        await this.client.InitializeAndAuthorizeAsync();

        ExpenseRoot data = new() { Expense = expense };
        using JsonContent content = JsonContent.Create(data, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.client.HttpClient.PutAsync(new Uri(this.client.ApiBaseUrl, $"/{ExpensesEndPoint}/{id}"), content);
        response.EnsureSuccessStatusCode();

        ExpenseRoot? root = await response.Content.ReadFromJsonAsync<ExpenseRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        this.InvalidateExpenseCache();

        return root?.Expense ?? throw new InvalidOperationException("Failed to update expense");
    }

    /// <summary>
    /// Deletes an expense from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the expense to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls DELETE /v2/expenses/{id} to delete the expense. The cache entries for this
    /// expense and all expense queries are invalidated after successful deletion.
    /// </remarks>
    public async Task DeleteAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.DeleteAsync(new Uri(this.client.ApiBaseUrl, $"/{ExpensesEndPoint}/{id}"));
        response.EnsureSuccessStatusCode();

        this.InvalidateExpenseCache();
    }

    /// <summary>
    /// Retrieves the current mileage settings from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="MileageSettings"/> including engine types, engine sizes, and mileage rates.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls GET /v2/expenses/mileage_settings to retrieve configuration data for mileage expenses,
    /// including available vehicle types, engine specifications, and current reimbursement rates.
    /// The settings are cached for 30 minutes as they change infrequently.
    /// </remarks>
    public async Task<MileageSettings> GetMileageSettingsAsync()
    {
        await this.client.InitializeAndAuthorizeAsync();

        string cacheKey = "expenses_mileage_settings";

        if (this.cache.TryGetValue(cacheKey, out MileageSettings? cached))
        {
            return cached!;
        }

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(new Uri(this.client.ApiBaseUrl, $"/{ExpensesEndPoint}/mileage_settings"));
        response.EnsureSuccessStatusCode();

        MileageSettingsRoot? root = await response.Content.ReadFromJsonAsync<MileageSettingsRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        MileageSettings settings = root?.MileageSettings ?? throw new InvalidOperationException("Failed to deserialize mileage settings response.");

        this.cache.Set(cacheKey, settings, TimeSpan.FromMinutes(30));
        this.expenseCacheKeys.Add(cacheKey);

        return settings;
    }

    /// <summary>
    /// Invalidates all expense-related cache entries.
    /// </summary>
    /// <remarks>
    /// This method clears all cached expenses, including individual expenses, filtered queries,
    /// and mileage settings. This ensures that any subsequent queries return fresh data from the API.
    /// </remarks>
    private void InvalidateExpenseCache()
    {
        foreach (string key in this.expenseCacheKeys)
        {
            this.cache.Remove(key);
        }

        this.expenseCacheKeys.Clear();
    }
}