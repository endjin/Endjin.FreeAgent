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
    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        string cacheKey = CategoriesEndPoint;

        if (!this.cache.TryGetValue(cacheKey, out IEnumerable<Category>? results))
        {
            HttpResponseMessage response = await this.freeAgentClient.HttpClient.GetAsync(new Uri(freeAgentClient.ApiBaseUrl, CategoriesEndPoint)).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            CategoriesRoot? root = await response.Content.ReadFromJsonAsync<CategoriesRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

            results = root?.Categories ?? [];
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
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no category with the specified nominal code is found.</exception>
    /// <remarks>
    /// This method retrieves all categories (using <see cref="GetAllAsync"/>) and filters by nominal code.
    /// The result benefits from the 30-minute cache, making repeated lookups efficient.
    /// </remarks>
    public async Task<Category> GetByNominalCodeAsync(string nominalCode)
    {
        IEnumerable<Category> categories = await GetAllAsync().ConfigureAwait(false);
        return categories.FirstOrDefault(c => c.NominalCode == nominalCode) ?? throw new InvalidOperationException($"Category with nominal code {nominalCode} not found.");
    }
}