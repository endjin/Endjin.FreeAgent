// <copyright file="RecurringInvoicesTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class RecurringInvoicesTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private RecurringInvoices recurringInvoices = null!;
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
        this.recurringInvoices = new RecurringInvoices(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task CreateAsync_WithValidRecurringInvoice_ReturnsCreatedRecurringInvoice()
    {
        // Arrange
        RecurringInvoice inputInvoice = new()
        {
            Contact = new Uri("https://api.freeagent.com/v2/contacts/123"),
            Frequency = "Monthly",
            RecurringStartsOn = new DateOnly(2024, 1, 1)
        };

        RecurringInvoice responseInvoice = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/recurring_invoices/456"),
            Contact = new Uri("https://api.freeagent.com/v2/contacts/123"),
            Frequency = "Monthly",
            RecurringStartsOn = new DateOnly(2024, 1, 1),
            Status = "Active"
        };

        RecurringInvoiceRoot responseRoot = new() { RecurringInvoice = responseInvoice };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        RecurringInvoice result = await this.recurringInvoices.CreateAsync(inputInvoice);

        // Assert
        result.ShouldNotBeNull();
        result.Url.ShouldNotBeNull();
        result.Frequency.ShouldBe("Monthly");
        result.Status.ShouldBe("Active");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
    }

    [TestMethod]
    public async Task GetAllAsync_WithoutView_ReturnsActiveRecurringInvoices()
    {
        // Arrange
        List<RecurringInvoice> invoicesList =
        [
            new() { Frequency = "Monthly", Status = "Active" },
            new() { Frequency = "Quarterly", Status = "Active" }
        ];

        RecurringInvoicesRoot responseRoot = new() { RecurringInvoices = invoicesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<RecurringInvoice> result = await this.recurringInvoices.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);
        result.All(i => i.Status == "Active").ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task GetAllAsync_WithAllView_ReturnsAllRecurringInvoices()
    {
        // Arrange
        List<RecurringInvoice> invoicesList =
        [
            new() { Frequency = "Monthly", Status = "Active" },
            new() { Frequency = "Annually", Status = "Inactive" }
        ];

        RecurringInvoicesRoot responseRoot = new() { RecurringInvoices = invoicesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<RecurringInvoice> result = await this.recurringInvoices.GetAllAsync("all");

        // Assert
        result.Count().ShouldBe(2);
        result.Any(i => i.Status == "Active").ShouldBeTrue();
        result.Any(i => i.Status == "Inactive").ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task GetAllAsync_CachesResults()
    {
        // Arrange
        List<RecurringInvoice> invoicesList =
        [
            new() { Frequency = "Monthly", Status = "Active" }
        ];

        RecurringInvoicesRoot responseRoot = new() { RecurringInvoices = invoicesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        IEnumerable<RecurringInvoice> result1 = await this.recurringInvoices.GetAllAsync();
        IEnumerable<RecurringInvoice> result2 = await this.recurringInvoices.GetAllAsync();

        // Assert
        result1.Count().ShouldBe(1);
        result2.Count().ShouldBe(1);

        // Mock Verification - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsRecurringInvoice()
    {
        // Arrange
        RecurringInvoice invoice = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/recurring_invoices/789"),
            Frequency = "Quarterly",
            Status = "Active",
            RecurringStartsOn = new DateOnly(2024, 1, 1)
        };

        RecurringInvoiceRoot responseRoot = new() { RecurringInvoice = invoice };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        RecurringInvoice result = await this.recurringInvoices.GetByIdAsync("789");

        // Assert
        result.ShouldNotBeNull();
        result.Frequency.ShouldBe("Quarterly");
        result.Status.ShouldBe("Active");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task GetByIdAsync_CachesResult()
    {
        // Arrange
        RecurringInvoice invoice = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/recurring_invoices/999"),
            Frequency = "Monthly",
            Status = "Active"
        };

        RecurringInvoiceRoot responseRoot = new() { RecurringInvoice = invoice };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        RecurringInvoice result1 = await this.recurringInvoices.GetByIdAsync("999");
        RecurringInvoice result2 = await this.recurringInvoices.GetByIdAsync("999");

        // Assert
        result1.ShouldNotBeNull();
        result2.ShouldNotBeNull();

        // Mock Verification - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task UpdateAsync_WithValidRecurringInvoice_ReturnsUpdatedRecurringInvoice()
    {
        // Arrange
        RecurringInvoice updatedInvoice = new()
        {
            Frequency = "Annually",
            RecurringEndsOn = new DateOnly(2025, 12, 31)
        };

        RecurringInvoice responseInvoice = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/recurring_invoices/555"),
            Frequency = "Annually",
            RecurringEndsOn = new DateOnly(2025, 12, 31),
            Status = "Active"
        };

        RecurringInvoiceRoot responseRoot = new() { RecurringInvoice = responseInvoice };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        RecurringInvoice result = await this.recurringInvoices.UpdateAsync("555", updatedInvoice);

        // Assert
        result.ShouldNotBeNull();
        result.Frequency.ShouldBe("Annually");
        result.RecurringEndsOn.ShouldBe(new DateOnly(2025, 12, 31));

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
    }

    [TestMethod]
    public async Task DeleteAsync_WithValidId_DeletesRecurringInvoice()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        await this.recurringInvoices.DeleteAsync("666");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
    }

    [TestMethod]
    public async Task ActivateAsync_WithValidId_ReturnsActivatedRecurringInvoice()
    {
        // Arrange
        RecurringInvoice responseInvoice = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/recurring_invoices/333"),
            Frequency = "Monthly",
            Status = "Active"
        };

        RecurringInvoiceRoot responseRoot = new() { RecurringInvoice = responseInvoice };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        RecurringInvoice result = await this.recurringInvoices.ActivateAsync("333");

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe("Active");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
    }

    [TestMethod]
    public async Task DeactivateAsync_WithValidId_ReturnsDeactivatedRecurringInvoice()
    {
        // Arrange
        RecurringInvoice responseInvoice = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/recurring_invoices/444"),
            Frequency = "Monthly",
            Status = "Inactive"
        };

        RecurringInvoiceRoot responseRoot = new() { RecurringInvoice = responseInvoice };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        RecurringInvoice result = await this.recurringInvoices.DeactivateAsync("444");

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe("Inactive");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
    }
}
