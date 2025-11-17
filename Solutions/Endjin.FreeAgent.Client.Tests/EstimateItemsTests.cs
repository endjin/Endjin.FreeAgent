// <copyright file="EstimateItemsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class EstimateItemsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private EstimateItems estimateItems = null!;
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
        this.estimateItems = new EstimateItems(this.freeAgentClient);
    }

    [TestMethod]
    public async Task CreateAsync_WithValidEstimateItem_ReturnsCreatedItem()
    {
        // Arrange
        EstimateItem inputItem = new()
        {
            Description = "Development Work",
            Quantity = 10m,
            Price = 100.00m,
            SalesTaxRate = 20.0m
        };

        EstimateItem responseItem = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/estimate_items/123"),
            Description = "Development Work",
            Quantity = 10m,
            Price = 100.00m,
            SalesTaxRate = 20.0m
        };

        EstimateItemRoot responseRoot = new() { EstimateItem = responseItem };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        EstimateItem result = await this.estimateItems.CreateAsync(inputItem);

        // Assert
        result.ShouldNotBeNull();
        result.Url.ShouldNotBeNull();
        result.Description.ShouldBe("Development Work");
        result.Quantity.ShouldBe(10m);
        result.Price.ShouldBe(100.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/estimate_items");
    }

    [TestMethod]
    public async Task UpdateAsync_WithValidEstimateItem_ReturnsUpdatedItem()
    {
        // Arrange
        EstimateItem updatedItem = new()
        {
            Description = "Updated Development Work",
            Quantity = 15m,
            Price = 125.00m
        };

        EstimateItem responseItem = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/estimate_items/456"),
            Description = "Updated Development Work",
            Quantity = 15m,
            Price = 125.00m,
            SalesTaxRate = 20.0m
        };

        EstimateItemRoot responseRoot = new() { EstimateItem = responseItem };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        EstimateItem result = await this.estimateItems.UpdateAsync("456", updatedItem);

        // Assert
        result.ShouldNotBeNull();
        result.Description.ShouldBe("Updated Development Work");
        result.Quantity.ShouldBe(15m);
        result.Price.ShouldBe(125.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/estimate_items/456");
    }

    [TestMethod]
    public async Task DeleteAsync_WithValidId_DeletesEstimateItem()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        await this.estimateItems.DeleteAsync("789");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/estimate_items/789");
    }
}
