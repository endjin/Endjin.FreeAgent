// <copyright file="OpeningBalancesTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class OpeningBalancesTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private OpeningBalances openingBalances = null!;
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
        this.openingBalances = new OpeningBalances(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetAsync_ReturnsOpeningBalance()
    {
        // Arrange
        OpeningBalance openingBalance = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/opening_balances"),
            CreatedAt = new DateTime(2024, 1, 1, 10, 0, 0)
        };

        OpeningBalanceRoot responseRoot = new() { OpeningBalance = openingBalance };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        OpeningBalance result = await this.openingBalances.GetAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Url.ShouldNotBeNull();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/opening_balances");
    }

    [TestMethod]
    public async Task UpdateAsync_WithValidBalance_ReturnsUpdatedBalance()
    {
        // Arrange
        OpeningBalance updatedBalance = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/opening_balances")
        };

        OpeningBalance responseBalance = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/opening_balances"),
            UpdatedAt = new DateTime(2024, 1, 15, 14, 30, 0)
        };

        OpeningBalanceRoot responseRoot = new() { OpeningBalance = responseBalance };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        OpeningBalance result = await this.openingBalances.UpdateAsync(updatedBalance);

        // Assert
        result.ShouldNotBeNull();
        result.Url.ShouldNotBeNull();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/opening_balances");
    }
}
