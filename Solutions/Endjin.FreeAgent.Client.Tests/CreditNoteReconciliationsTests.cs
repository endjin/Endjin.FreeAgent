// <copyright file="CreditNoteReconciliationsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class CreditNoteReconciliationsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private CreditNoteReconciliations creditNoteReconciliations = null!;
    private HttpClient httpClient = null!;
    private TestHttpMessageHandler messageHandler = null!;

    [TestInitialize]
    public async Task Setup()
    {
        this.cache = new MemoryCache(new MemoryCacheOptions());
        this.messageHandler = new TestHttpMessageHandler();
        this.httpClient = new HttpClient(this.messageHandler);
        this.httpClientFactory = Substitute.For<IHttpClientFactory>();
        this.httpClientFactory.CreateClient(Arg.Any<string>()).Returns(this.httpClient);
        this.loggerFactory = Substitute.For<ILoggerFactory>();

        this.freeAgentClient = new FreeAgentClient(
            new FreeAgentOptionsBuilder().Build(),
            this.cache,
            this.httpClientFactory,
            this.loggerFactory);

        await TestHelper.SetupForTestingAsync(this.freeAgentClient, this.httpClientFactory);
        this.creditNoteReconciliations = new CreditNoteReconciliations(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task CreateAsync_WithValidReconciliation_ReturnsCreatedReconciliation()
    {
        // Arrange
        CreditNoteReconciliation inputReconciliation = new()
        {
            GrossValue = 100.00m,
            DatedOn = new DateOnly(2024, 1, 20),
            Currency = "GBP",
            Invoice = new Uri("https://api.freeagent.com/v2/invoices/123"),
            CreditNote = new Uri("https://api.freeagent.com/v2/credit_notes/456")
        };

        CreditNoteReconciliation responseReconciliation = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/credit_note_reconciliations/789"),
            GrossValue = 100.00m,
            DatedOn = new DateOnly(2024, 1, 20),
            Currency = "GBP",
            Invoice = new Uri("https://api.freeagent.com/v2/invoices/123"),
            CreditNote = new Uri("https://api.freeagent.com/v2/credit_notes/456"),
            CreatedAt = new DateTime(2024, 1, 20, 10, 30, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2024, 1, 20, 10, 30, 0, DateTimeKind.Utc)
        };

        CreditNoteReconciliationRoot responseRoot = new() { CreditNoteReconciliation = responseReconciliation };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        CreditNoteReconciliation result = await this.creditNoteReconciliations.CreateAsync(inputReconciliation);

        // Assert
        result.ShouldNotBeNull();
        result.Url.ShouldBe(new Uri("https://api.freeagent.com/v2/credit_note_reconciliations/789"));
        result.GrossValue.ShouldBe(100.00m);
        result.Currency.ShouldBe("GBP");
        result.DatedOn.ShouldBe(new DateOnly(2024, 1, 20));

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/credit_note_reconciliations");
    }

    [TestMethod]
    public async Task CreateAsync_InvalidatesListCache()
    {
        // Arrange - First, populate the cache by calling GetAllAsync
        List<CreditNoteReconciliation> initialList =
        [
            new() { Url = new Uri("https://api.freeagent.com/v2/credit_note_reconciliations/1"), GrossValue = 50.00m }
        ];

        CreditNoteReconciliationsRoot listResponse = new() { CreditNoteReconciliations = initialList };
        string listJson = JsonSerializer.Serialize(listResponse, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(listJson, Encoding.UTF8, "application/json")
        };

        await this.creditNoteReconciliations.GetAllAsync();
        this.messageHandler.Reset();

        // Setup response for CreateAsync
        CreditNoteReconciliation newReconciliation = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/credit_note_reconciliations/2"),
            GrossValue = 100.00m,
            Currency = "GBP"
        };

        CreditNoteReconciliationRoot createResponse = new() { CreditNoteReconciliation = newReconciliation };
        string createJson = JsonSerializer.Serialize(createResponse, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(createJson, Encoding.UTF8, "application/json")
        };

        // Act - Create a new reconciliation
        await this.creditNoteReconciliations.CreateAsync(new CreditNoteReconciliation
        {
            GrossValue = 100.00m,
            Currency = "GBP",
            Invoice = new Uri("https://api.freeagent.com/v2/invoices/123"),
            CreditNote = new Uri("https://api.freeagent.com/v2/credit_notes/456")
        });
        this.messageHandler.Reset();

        // Setup response for second GetAllAsync call
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(listJson, Encoding.UTF8, "application/json")
        };

        // Assert - GetAllAsync should make a new request since cache was invalidated
        await this.creditNoteReconciliations.GetAllAsync();
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllReconciliations()
    {
        // Arrange
        List<CreditNoteReconciliation> reconciliationsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/credit_note_reconciliations/1"),
                GrossValue = 100.00m,
                Currency = "GBP"
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/credit_note_reconciliations/2"),
                GrossValue = 200.00m,
                Currency = "USD"
            }
        ];

        CreditNoteReconciliationsRoot responseRoot = new() { CreditNoteReconciliations = reconciliationsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<CreditNoteReconciliation> result = await this.creditNoteReconciliations.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);
        result.Any(r => r.GrossValue == 100.00m && r.Currency == "GBP").ShouldBeTrue();
        result.Any(r => r.GrossValue == 200.00m && r.Currency == "USD").ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task GetAllAsync_WithFilters_IncludesQueryParameters()
    {
        // Arrange
        List<CreditNoteReconciliation> reconciliationsList = [new() { GrossValue = 100.00m }];
        CreditNoteReconciliationsRoot responseRoot = new() { CreditNoteReconciliations = reconciliationsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        await this.creditNoteReconciliations.GetAllAsync(
            updatedSince: new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
            fromDate: "2024-01-01",
            toDate: "2024-12-31");

        // Assert
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveQueryParameter("updated_since", "2024-01-01T00:00:00.000Z");
        this.messageHandler.ShouldHaveQueryParameter("from_date", "2024-01-01");
        this.messageHandler.ShouldHaveQueryParameter("to_date", "2024-12-31");
    }

    [TestMethod]
    public async Task GetAllAsync_UsesCacheOnSecondCall()
    {
        // Arrange
        List<CreditNoteReconciliation> reconciliationsList = [new() { GrossValue = 100.00m }];
        CreditNoteReconciliationsRoot responseRoot = new() { CreditNoteReconciliations = reconciliationsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        await this.creditNoteReconciliations.GetAllAsync();
        await this.creditNoteReconciliations.GetAllAsync();

        // Assert - Should only make one HTTP request due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task GetByIdAsync_ReturnsReconciliation()
    {
        // Arrange
        CreditNoteReconciliation responseReconciliation = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/credit_note_reconciliations/789"),
            GrossValue = 100.00m,
            DatedOn = new DateOnly(2024, 1, 20),
            Currency = "GBP",
            ExchangeRate = 1.0m,
            Invoice = new Uri("https://api.freeagent.com/v2/invoices/123"),
            CreditNote = new Uri("https://api.freeagent.com/v2/credit_notes/456")
        };

        CreditNoteReconciliationRoot responseRoot = new() { CreditNoteReconciliation = responseReconciliation };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        CreditNoteReconciliation result = await this.creditNoteReconciliations.GetByIdAsync("789");

        // Assert
        result.ShouldNotBeNull();
        result.Url.ShouldBe(new Uri("https://api.freeagent.com/v2/credit_note_reconciliations/789"));
        result.GrossValue.ShouldBe(100.00m);
        result.ExchangeRate.ShouldBe(1.0m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/credit_note_reconciliations/789");
    }

    [TestMethod]
    public async Task GetByIdAsync_UsesCacheOnSecondCall()
    {
        // Arrange
        CreditNoteReconciliation responseReconciliation = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/credit_note_reconciliations/789"),
            GrossValue = 100.00m
        };

        CreditNoteReconciliationRoot responseRoot = new() { CreditNoteReconciliation = responseReconciliation };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice with same ID
        await this.creditNoteReconciliations.GetByIdAsync("789");
        await this.creditNoteReconciliations.GetByIdAsync("789");

        // Assert - Should only make one HTTP request due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task UpdateAsync_WithValidReconciliation_ReturnsUpdatedReconciliation()
    {
        // Arrange
        CreditNoteReconciliation inputReconciliation = new()
        {
            GrossValue = 150.00m,
            DatedOn = new DateOnly(2024, 2, 1),
            Currency = "GBP"
        };

        CreditNoteReconciliation responseReconciliation = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/credit_note_reconciliations/789"),
            GrossValue = 150.00m,
            DatedOn = new DateOnly(2024, 2, 1),
            Currency = "GBP",
            UpdatedAt = new DateTime(2024, 2, 1, 10, 30, 0, DateTimeKind.Utc)
        };

        CreditNoteReconciliationRoot responseRoot = new() { CreditNoteReconciliation = responseReconciliation };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        CreditNoteReconciliation result = await this.creditNoteReconciliations.UpdateAsync("789", inputReconciliation);

        // Assert
        result.ShouldNotBeNull();
        result.GrossValue.ShouldBe(150.00m);
        result.DatedOn.ShouldBe(new DateOnly(2024, 2, 1));

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/credit_note_reconciliations/789");
    }

    [TestMethod]
    public async Task UpdateAsync_InvalidatesCache()
    {
        // Arrange - First, cache the item
        CreditNoteReconciliation cachedReconciliation = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/credit_note_reconciliations/789"),
            GrossValue = 100.00m
        };

        CreditNoteReconciliationRoot getResponse = new() { CreditNoteReconciliation = cachedReconciliation };
        string getJson = JsonSerializer.Serialize(getResponse, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(getJson, Encoding.UTF8, "application/json")
        };

        await this.creditNoteReconciliations.GetByIdAsync("789");
        this.messageHandler.Reset();

        // Update the reconciliation
        CreditNoteReconciliation updatedReconciliation = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/credit_note_reconciliations/789"),
            GrossValue = 150.00m
        };

        CreditNoteReconciliationRoot updateResponse = new() { CreditNoteReconciliation = updatedReconciliation };
        string updateJson = JsonSerializer.Serialize(updateResponse, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(updateJson, Encoding.UTF8, "application/json")
        };

        await this.creditNoteReconciliations.UpdateAsync("789", new CreditNoteReconciliation { GrossValue = 150.00m });
        this.messageHandler.Reset();

        // Setup response for GetByIdAsync
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(updateJson, Encoding.UTF8, "application/json")
        };

        // Act - GetByIdAsync should make a new request since cache was invalidated
        await this.creditNoteReconciliations.GetByIdAsync("789");

        // Assert
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task DeleteAsync_WithValidId_DeletesSuccessfully()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK);

        // Act
        await this.creditNoteReconciliations.DeleteAsync("789");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/credit_note_reconciliations/789");
    }

    [TestMethod]
    public async Task DeleteAsync_InvalidatesCache()
    {
        // Arrange - First, cache the item
        CreditNoteReconciliation cachedReconciliation = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/credit_note_reconciliations/789"),
            GrossValue = 100.00m
        };

        CreditNoteReconciliationRoot getResponse = new() { CreditNoteReconciliation = cachedReconciliation };
        string getJson = JsonSerializer.Serialize(getResponse, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(getJson, Encoding.UTF8, "application/json")
        };

        await this.creditNoteReconciliations.GetByIdAsync("789");
        this.messageHandler.Reset();

        // Delete the reconciliation
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK);
        await this.creditNoteReconciliations.DeleteAsync("789");
        this.messageHandler.Reset();

        // Setup response for GetByIdAsync (would return 404 in real scenario)
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(getJson, Encoding.UTF8, "application/json")
        };

        // Act - GetByIdAsync should make a new request since cache was invalidated
        await this.creditNoteReconciliations.GetByIdAsync("789");

        // Assert
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task GetByIdAsync_WhenNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        CreditNoteReconciliationRoot responseRoot = new() { CreditNoteReconciliation = null };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act & Assert
        InvalidOperationException exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await this.creditNoteReconciliations.GetByIdAsync("nonexistent"));

        exception.Message.ShouldContain("nonexistent");
        exception.Message.ShouldContain("not found");
    }

    [TestMethod]
    public async Task CreateAsync_WhenDeserializationFails_ThrowsInvalidOperationException()
    {
        // Arrange
        CreditNoteReconciliationRoot responseRoot = new() { CreditNoteReconciliation = null };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        CreditNoteReconciliation inputReconciliation = new()
        {
            GrossValue = 100.00m,
            Currency = "GBP",
            Invoice = new Uri("https://api.freeagent.com/v2/invoices/123"),
            CreditNote = new Uri("https://api.freeagent.com/v2/credit_notes/456")
        };

        // Act & Assert
        InvalidOperationException exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await this.creditNoteReconciliations.CreateAsync(inputReconciliation));

        exception.Message.ShouldContain("Failed to deserialize");
    }

    [TestMethod]
    public async Task UpdateAsync_WhenDeserializationFails_ThrowsInvalidOperationException()
    {
        // Arrange
        CreditNoteReconciliationRoot responseRoot = new() { CreditNoteReconciliation = null };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        CreditNoteReconciliation inputReconciliation = new()
        {
            GrossValue = 150.00m,
            Currency = "GBP"
        };

        // Act & Assert
        InvalidOperationException exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await this.creditNoteReconciliations.UpdateAsync("789", inputReconciliation));

        exception.Message.ShouldContain("Failed to deserialize");
    }
}
