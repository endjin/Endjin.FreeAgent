// <copyright file="BillsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class BillsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private Bills bills = null!;
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
        this.bills = new Bills(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task CreateAsync_WithValidBill_ReturnsCreatedBill()
    {
        // Arrange
        Bill inputBill = new()
        {
            Contact = new Uri("https://api.freeagent.com/v2/contacts/789"),
            DatedOn = new DateOnly(2024, 1, 10),
            DueOn = new DateOnly(2024, 2, 10),
            Reference = "BILL-001",
            TotalValue = 600.00m
        };

        Bill responseBill = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/bills/456"),
            Contact = new Uri("https://api.freeagent.com/v2/contacts/789"),
            Reference = "BILL-001",
            Status = "Open",
            TotalValue = 600.00m
        };

        BillRoot responseRoot = new() { Bill = responseBill };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Bill result = await this.bills.CreateAsync(inputBill);

        // Assert
        result.ShouldNotBeNull();
        result.Reference.ShouldBe("BILL-001");
        result.Status.ShouldBe("Open");
        result.TotalValue.ShouldBe(600.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bills");
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllBills()
    {
        // Arrange
        List<Bill> billsList =
        [
            new() { Reference = "BILL-001", Status = "Open", TotalValue = 500.00m },
            new() { Reference = "BILL-002", Status = "Paid", TotalValue = 750.00m }
        ];

        BillsRoot responseRoot = new() { Bills = billsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Bill> result = await this.bills.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);
        result.Any(b => b.Reference == "BILL-001").ShouldBeTrue();
        result.Any(b => b.Reference == "BILL-002").ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bills?view=all");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsBill()
    {
        // Arrange
        Bill bill = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/bills/456"),
            Reference = "BILL-001",
            Status = "Open",
            TotalValue = 600.00m
        };

        BillRoot responseRoot = new() { Bill = bill };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Bill result = await this.bills.GetByIdAsync("456");

        // Assert
        result.ShouldNotBeNull();
        result.Reference.ShouldBe("BILL-001");
        result.Status.ShouldBe("Open");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bills/456");
    }

    [TestMethod]
    public async Task UpdateAsync_WithValidChanges_ReturnsUpdatedBill()
    {
        // Arrange
        Bill updateBill = new()
        {
            Reference = "BILL-001-UPDATED"
        };

        Bill responseBill = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/bills/456"),
            Reference = "BILL-001-UPDATED",
            Status = "Open"
        };

        BillRoot responseRoot = new() { Bill = responseBill };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Bill result = await this.bills.UpdateAsync("456", updateBill);

        // Assert
        result.ShouldNotBeNull();
        result.Reference.ShouldBe("BILL-001-UPDATED");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bills/456");
    }

    [TestMethod]
    public async Task DeleteAsync_WithValidId_DeletesSuccessfully()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        await this.bills.DeleteAsync("456");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bills/456");
    }
}
