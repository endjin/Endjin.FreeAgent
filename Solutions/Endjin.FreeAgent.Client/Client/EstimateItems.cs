// <copyright file="EstimateItems.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing individual estimate line items via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides access to FreeAgent estimate line item operations, allowing
/// granular control over individual items within estimates. These endpoints complement the
/// nested item management available through the main Estimates API.
/// </para>
/// <para>
/// Line items can be created, updated, and deleted independently of their parent estimate,
/// which is useful for incremental estimate modifications without replacing the entire estimate.
/// </para>
/// </remarks>
/// <seealso cref="EstimateItem"/>
/// <seealso cref="Estimate"/>
/// <seealso cref="Estimates"/>
public class EstimateItems
{
    private const string EstimateItemsEndPoint = "v2/estimate_items";
    private readonly FreeAgentClient freeAgentClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="EstimateItems"/> class.
    /// </summary>
    /// <param name="freeAgentClient">The FreeAgent HTTP client for making API requests.</param>
    public EstimateItems(FreeAgentClient freeAgentClient)
    {
        this.freeAgentClient = freeAgentClient;
    }

    /// <summary>
    /// Creates a new estimate line item in FreeAgent.
    /// </summary>
    /// <param name="item">The <see cref="EstimateItem"/> object containing the line item details to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// created <see cref="EstimateItem"/> object with server-assigned values.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls POST /v2/estimate_items to create a new line item.
    /// The item must be associated with an existing estimate.
    /// </remarks>
    public async Task<EstimateItem> CreateAsync(EstimateItem item)
    {
        EstimateItemRoot root = new() { EstimateItem = item };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PostAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, EstimateItemsEndPoint),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        EstimateItemRoot? result = await response.Content.ReadFromJsonAsync<EstimateItemRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.EstimateItem ?? throw new InvalidOperationException("Failed to deserialize estimate item response.");
    }

    /// <summary>
    /// Updates an existing estimate line item in FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the estimate item to update.</param>
    /// <param name="item">The <see cref="EstimateItem"/> object containing the updated line item details.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// updated <see cref="EstimateItem"/> object as returned by the API.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API response cannot be deserialized.</exception>
    /// <remarks>
    /// This method calls PUT /v2/estimate_items/{id} to update the line item.
    /// </remarks>
    public async Task<EstimateItem> UpdateAsync(string id, EstimateItem item)
    {
        EstimateItemRoot root = new() { EstimateItem = item };
        using JsonContent content = JsonContent.Create(root, options: SharedJsonOptions.SourceGenOptions);

        HttpResponseMessage response = await this.freeAgentClient.HttpClient.PutAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{EstimateItemsEndPoint}/{id}"),
            content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        EstimateItemRoot? result = await response.Content.ReadFromJsonAsync<EstimateItemRoot>(SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return result?.EstimateItem ?? throw new InvalidOperationException("Failed to deserialize estimate item response.");
    }

    /// <summary>
    /// Deletes an estimate line item from FreeAgent.
    /// </summary>
    /// <param name="id">The unique identifier of the estimate item to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls DELETE /v2/estimate_items/{id} to delete the line item.
    /// The parent estimate must be in draft status.
    /// </remarks>
    public async Task DeleteAsync(string id)
    {
        HttpResponseMessage response = await this.freeAgentClient.HttpClient.DeleteAsync(
            new Uri(this.freeAgentClient.ApiBaseUrl, $"{EstimateItemsEndPoint}/{id}")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
    }
}
