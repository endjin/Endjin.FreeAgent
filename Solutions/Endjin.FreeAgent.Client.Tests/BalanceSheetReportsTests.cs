// <copyright file="BalanceSheetReportsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class BalanceSheetReportsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private BalanceSheetReports balanceSheetReports = null!;
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
        this.balanceSheetReports = new BalanceSheetReports(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetAsync_WithDate_ReturnsBalanceSheet()
    {
        // Arrange
        BalanceSheet balanceSheet = new()
        {
            DatedOn = new DateOnly(2024, 3, 31),
            FixedAssets = 100000.00m,
            CurrentAssets = 50000.00m,
            CurrentLiabilities = 30000.00m
        };

        BalanceSheetRoot responseRoot = new() { BalanceSheet = balanceSheet };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        BalanceSheet result = await this.balanceSheetReports.GetAsync(new DateOnly(2024, 3, 31));

        // Assert
        result.ShouldNotBeNull();
        result.DatedOn.ShouldBe(new DateOnly(2024, 3, 31));

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }
}
