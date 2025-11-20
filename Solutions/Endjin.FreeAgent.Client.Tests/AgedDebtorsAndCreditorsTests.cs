// <copyright file="AgedDebtorsAndCreditorsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class AgedDebtorsAndCreditorsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private AgedDebtorsAndCreditors agedDebtorsAndCreditors = null!;
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
        this.agedDebtorsAndCreditors = new AgedDebtorsAndCreditors(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetSalesAgedDebtorsAsync_ReturnsDebtorsReport()
    {
        // Arrange
        SalesAgedDebtors report = new();

        SalesAgedDebtorsRoot responseRoot = new() { SalesAgedDebtors = report };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        SalesAgedDebtors result = await this.agedDebtorsAndCreditors.GetSalesAgedDebtorsAsync();

        // Assert
        result.ShouldNotBeNull();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task GetPurchaseAgedCreditorsAsync_ReturnsCreditorsReport()
    {
        // Arrange
        PurchaseAgedCreditors report = new();

        PurchaseAgedCreditorsRoot responseRoot = new() { PurchaseAgedCreditors = report };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        PurchaseAgedCreditors result = await this.agedDebtorsAndCreditors.GetPurchaseAgedCreditorsAsync();

        // Assert
        result.ShouldNotBeNull();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }
}
