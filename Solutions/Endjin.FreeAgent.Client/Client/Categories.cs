// <copyright file="Categories.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for accessing accounting categories (nominal codes) via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides read-only access to FreeAgent's chart of accounts, which consists of
/// categories (also known as nominal accounts or nominal codes). Categories are used to classify
/// financial transactions for reporting and tax purposes.
/// </para>
/// <para>
/// Results are cached for 30 minutes as categories rarely change during typical usage. The cache
/// improves performance when repeatedly accessing the category list or looking up individual categories
/// by nominal code.
/// </para>
/// </remarks>
/// <seealso cref="Category"/>
public class Categories
{
    private const string CategoriesEndPoint = "v2/categories";
    private readonly FreeAgentClient freeAgentClient;
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Categories"/> class.
    /// </summary>
    /// <param name="freeAgentClient">The FreeAgent HTTP client for making API requests.</param>
    /// <param name="cache">The memory cache for storing category data.</param>
    public Categories(FreeAgentClient freeAgentClient, IMemoryCache cache)
    {
        this.freeAgentClient = freeAgentClient;
        this.cache = cache;
        this.cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(30)); // Categories don't change often
    }

    /// <summary>
    /// Retrieves all accounting categories from FreeAgent.
    /// </summary>
    /// <param name="includeSubAccounts">
    /// If <c>true</c>, includes sub-accounts in the response. Defaults to <c>false</c>.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// <see cref="Category"/> objects representing the complete chart of accounts.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/categories and caches the result for 30 minutes. Categories include
    /// nominal codes for assets, liabilities, income, expenses, and other account types used in
    /// double-entry bookkeeping.
    /// </remarks>
    public async Task<IEnumerable<Category>> GetAllAsync(bool includeSubAccounts = false)
    {
        string cacheKey = $"{CategoriesEndPoint}?sub_accounts={includeSubAccounts}";
        string endpoint = includeSubAccounts ? $"{CategoriesEndPoint}?sub_accounts=true" : CategoriesEndPoint;

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<Category>? results))
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(new Uri(freeAgentClient.ApiBaseUrl, endpoint)).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            CategoriesRoot? root = await response.Content.ReadFromJsonAsync<CategoriesRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            // Combine all four category arrays into a single collection
            results = root is not null
                ? root.AdminExpensesCategories
                    .Concat(root.CostOfSalesCategories)
                    .Concat(root.IncomeCategories)
                    .Concat(root.GeneralCategories)
                    .ToList()
                : [];
            this.cache.Set(cacheKey, results, cacheEntryOptions);
        }

        return results ?? [];
    }

    /// <summary>
    /// Retrieves a specific accounting category by its nominal code.
    /// </summary>
    /// <param name="nominalCode">The nominal code of the category to retrieve (e.g., "001", "310").</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="Category"/> object with the specified nominal code.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails or the category is not found.</exception>
    /// <remarks>
    /// This method calls GET /v2/categories/:nominal_code directly to retrieve a specific category.
    /// Results are cached for 30 minutes, making repeated lookups efficient.
    /// </remarks>
    public async Task<Category> GetByNominalCodeAsync(string nominalCode)
    {
        string cacheKey = $"{CategoriesEndPoint}/{nominalCode}";
        string endpoint = $"{CategoriesEndPoint}/{nominalCode}";

        if (!this.cache.TryGetValue<Category>(cacheKey, out Category? result) || result is null)
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(new Uri(freeAgentClient.ApiBaseUrl, endpoint)).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            CategoryRoot? root = await response.Content.ReadFromJsonAsync<CategoryRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            result = root?.Category ?? throw new HttpRequestException($"Category with nominal code {nominalCode} not found.");
            this.cache.Set(cacheKey, result, cacheEntryOptions);
        }

        return result;
    }

    /// <summary>
    /// Creates a new category in FreeAgent.
    /// </summary>
    /// <param name="request">The category creation request containing the category details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// newly created <see cref="Category"/> object.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// <para>
    /// This method calls POST /v2/categories to create a new category.
    /// Supported category groups include: "income", "cost_of_sales", "admin_expenses", "current_assets", "liabilities", and "equities".
    /// </para>
    /// <para>
    /// The cache is invalidated after creating a category to ensure subsequent calls to <see cref="GetAllAsync"/> return the updated list.
    /// </para>
    /// </remarks>
    public async Task<Category> CreateAsync(CategoryCreateRequest request)
    {
        CategoryCreateRequestRoot requestRoot = new() { Category = request };

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsJsonAsync(
            new Uri(freeAgentClient.ApiBaseUrl, CategoriesEndPoint),
            requestRoot,
            SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        CategoryRoot? root = await response.Content.ReadFromJsonAsync<CategoryRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache to ensure GetAllAsync returns updated list
        this.cache.Remove(CategoriesEndPoint);
        this.cache.Remove($"{CategoriesEndPoint}?sub_accounts=true");

        return root?.Category ?? throw new HttpRequestException("Failed to create category.");
    }

    /// <summary>
    /// Updates an existing category in FreeAgent.
    /// </summary>
    /// <param name="nominalCode">The nominal code of the category to update.</param>
    /// <param name="request">The category update request containing the updated category details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="Category"/> object.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// <para>
    /// This method calls PUT /v2/categories/:nominal_code to update an existing category.
    /// Only empty categories (categories with no associated transactions) can be modified.
    /// </para>
    /// <para>
    /// The cache is invalidated after updating a category to ensure subsequent calls return the updated data.
    /// </para>
    /// </remarks>
    public async Task<Category> UpdateAsync(string nominalCode, CategoryUpdateRequest request)
    {
        string endpoint = $"{CategoriesEndPoint}/{nominalCode}";
        CategoryUpdateRequestRoot requestRoot = new() { Category = request };

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsJsonAsync(
            new Uri(freeAgentClient.ApiBaseUrl, endpoint),
            requestRoot,
            SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        CategoryRoot? root = await response.Content.ReadFromJsonAsync<CategoryRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache entries
        this.cache.Remove(CategoriesEndPoint);
        this.cache.Remove($"{CategoriesEndPoint}?sub_accounts=true");
        this.cache.Remove($"{CategoriesEndPoint}/{nominalCode}");

        return root?.Category ?? throw new HttpRequestException($"Failed to update category with nominal code {nominalCode}.");
    }

    /// <summary>
    /// Deletes a category from FreeAgent.
    /// </summary>
    /// <param name="nominalCode">The nominal code of the category to delete.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// deleted <see cref="Category"/> object.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// <para>
    /// This method calls DELETE /v2/categories/:nominal_code to delete a category.
    /// Only user-created, empty categories (categories with no associated transactions) can be deleted.
    /// </para>
    /// <para>
    /// The cache is invalidated after deleting a category to ensure subsequent calls return the updated list.
    /// </para>
    /// </remarks>
    public async Task<Category> DeleteAsync(string nominalCode)
    {
        string endpoint = $"{CategoriesEndPoint}/{nominalCode}";

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.DeleteAsync(
            new Uri(freeAgentClient.ApiBaseUrl, endpoint)).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        CategoryRoot? root = await response.Content.ReadFromJsonAsync<CategoryRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        // Invalidate cache entries
        this.cache.Remove(CategoriesEndPoint);
        this.cache.Remove($"{CategoriesEndPoint}?sub_accounts=true");
        this.cache.Remove($"{CategoriesEndPoint}/{nominalCode}");

        return root?.Category ?? throw new HttpRequestException($"Failed to delete category with nominal code {nominalCode}.");
    }
}