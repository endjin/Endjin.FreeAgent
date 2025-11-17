// <copyright file="SalesTaxPeriods.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net.Http.Json;
using Endjin.FreeAgent.Domain.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides methods for managing sales tax periods via the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This service class provides full CRUD access to FreeAgent sales tax periods, which define
/// the tax configuration for a company over specific time periods. Sales tax periods track
/// tax names, registration status, tax rates, and whether tax can be reclaimed.
/// </para>
/// <para>
/// Multiple periods can exist with different effective dates to handle changes in tax status
/// over time (e.g., registering for VAT, changing tax rates). Periods cannot have overlapping
/// effective dates.
/// </para>
/// <para>
/// Available for US and Universal companies only. Locked periods (those with associated
/// transactions) cannot be deleted.
/// </para>
/// <para>
/// Minimum Access Level: Estimates and Invoices (read), Full Access (create/update/delete).
/// </para>
/// </remarks>
/// <seealso cref="SalesTaxPeriod"/>
public class SalesTaxPeriods
{
    private readonly FreeAgentClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="SalesTaxPeriods"/> class.
    /// </summary>
    /// <param name="client">The FreeAgent HTTP client for making API requests.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> is null.</exception>
    public SalesTaxPeriods(FreeAgentClient client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    /// <summary>
    /// Retrieves all sales tax periods from FreeAgent.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a collection of
    /// all <see cref="SalesTaxPeriod"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/sales_tax_periods. Returns all sales tax periods for the company,
    /// ordered by effective date. Available for US and Universal companies only.
    /// </remarks>
    public async Task<IEnumerable<SalesTaxPeriod>> GetAllAsync()
    {
        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(
            new Uri(this.client.ApiBaseUrl, "/v2/sales_tax_periods"));
        response.EnsureSuccessStatusCode();

        SalesTaxPeriodsRoot? root = await response.Content.ReadFromJsonAsync<SalesTaxPeriodsRoot>(
            SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return root?.SalesTaxPeriods ?? [];
    }

    /// <summary>
    /// Retrieves a specific sales tax period by its ID.
    /// </summary>
    /// <param name="id">The ID of the sales tax period to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
    /// <see cref="SalesTaxPeriod"/> object.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// This method calls GET /v2/sales_tax_periods/:id. Available for US and Universal companies only.
    /// </remarks>
    public async Task<SalesTaxPeriod> GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Sales tax period ID cannot be null or whitespace.", nameof(id));
        }

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.GetAsync(
            new Uri(this.client.ApiBaseUrl, $"/v2/sales_tax_periods/{id}"));
        response.EnsureSuccessStatusCode();

        SalesTaxPeriodRoot? root = await response.Content.ReadFromJsonAsync<SalesTaxPeriodRoot>(
            SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return root?.SalesTaxPeriod ?? throw new InvalidOperationException("Response did not contain a sales tax period.");
    }

    /// <summary>
    /// Creates a new sales tax period.
    /// </summary>
    /// <param name="period">The sales tax period to create.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the created
    /// <see cref="SalesTaxPeriod"/> object.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="period"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when required fields are missing.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// <para>
    /// This method calls POST /v2/sales_tax_periods. Available for US and Universal companies only.
    /// Requires Full Access permission level.
    /// </para>
    /// <para>
    /// Required fields: SalesTaxName, SalesTaxRegistrationStatus, SalesTaxIsValueAdded, EffectiveDate.
    /// The effective date cannot overlap with existing periods.
    /// </para>
    /// </remarks>
    public async Task<SalesTaxPeriod> CreateAsync(SalesTaxPeriod period)
    {
        if (period == null)
        {
            throw new ArgumentNullException(nameof(period));
        }

        // Validate required fields
        if (string.IsNullOrWhiteSpace(period.SalesTaxName))
        {
            throw new ArgumentException("SalesTaxName is required when creating a sales tax period.", nameof(period));
        }

        if (period.SalesTaxRegistrationStatus == null)
        {
            throw new ArgumentException("SalesTaxRegistrationStatus is required when creating a sales tax period.", nameof(period));
        }

        if (period.SalesTaxIsValueAdded == null)
        {
            throw new ArgumentException("SalesTaxIsValueAdded is required when creating a sales tax period.", nameof(period));
        }

        if (period.EffectiveDate == null)
        {
            throw new ArgumentException("EffectiveDate is required when creating a sales tax period.", nameof(period));
        }

        await this.client.InitializeAndAuthorizeAsync();

        var payload = new SalesTaxPeriodRoot
        {
            SalesTaxPeriod = period,
        };

        HttpResponseMessage response = await this.client.HttpClient.PostAsJsonAsync(
            new Uri(this.client.ApiBaseUrl, "/v2/sales_tax_periods"),
            payload,
            SharedJsonOptions.SourceGenOptions);
        response.EnsureSuccessStatusCode();

        SalesTaxPeriodRoot? root = await response.Content.ReadFromJsonAsync<SalesTaxPeriodRoot>(
            SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return root?.SalesTaxPeriod ?? throw new InvalidOperationException("Response did not contain a sales tax period.");
    }

    /// <summary>
    /// Updates an existing sales tax period.
    /// </summary>
    /// <param name="id">The ID of the sales tax period to update.</param>
    /// <param name="period">The updated sales tax period data.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the updated
    /// <see cref="SalesTaxPeriod"/> object.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="period"/> is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// <para>
    /// This method calls PUT /v2/sales_tax_periods/:id. Available for US and Universal companies only.
    /// Requires Full Access permission level.
    /// </para>
    /// <para>
    /// Locked periods (those with associated transactions) may have restrictions on what can be updated.
    /// The effective date cannot be changed to overlap with other existing periods.
    /// </para>
    /// </remarks>
    public async Task<SalesTaxPeriod> UpdateAsync(string id, SalesTaxPeriod period)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Sales tax period ID cannot be null or whitespace.", nameof(id));
        }

        if (period == null)
        {
            throw new ArgumentNullException(nameof(period));
        }

        await this.client.InitializeAndAuthorizeAsync();

        var payload = new SalesTaxPeriodRoot
        {
            SalesTaxPeriod = period,
        };

        HttpResponseMessage response = await this.client.HttpClient.PutAsJsonAsync(
            new Uri(this.client.ApiBaseUrl, $"/v2/sales_tax_periods/{id}"),
            payload,
            SharedJsonOptions.SourceGenOptions);
        response.EnsureSuccessStatusCode();

        SalesTaxPeriodRoot? root = await response.Content.ReadFromJsonAsync<SalesTaxPeriodRoot>(
            SharedJsonOptions.SourceGenOptions).ConfigureAwait(false);

        return root?.SalesTaxPeriod ?? throw new InvalidOperationException("Response did not contain a sales tax period.");
    }

    /// <summary>
    /// Deletes a sales tax period.
    /// </summary>
    /// <param name="id">The ID of the sales tax period to delete.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <remarks>
    /// <para>
    /// This method calls DELETE /v2/sales_tax_periods/:id. Available for US and Universal companies only.
    /// Requires Full Access permission level.
    /// </para>
    /// <para>
    /// Locked periods (those with associated transactions) cannot be deleted. Attempting to delete
    /// a locked period will result in an error from the API.
    /// </para>
    /// </remarks>
    public async Task DeleteAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Sales tax period ID cannot be null or whitespace.", nameof(id));
        }

        await this.client.InitializeAndAuthorizeAsync();

        HttpResponseMessage response = await this.client.HttpClient.DeleteAsync(
            new Uri(this.client.ApiBaseUrl, $"/v2/sales_tax_periods/{id}"));
        response.EnsureSuccessStatusCode();
    }
}