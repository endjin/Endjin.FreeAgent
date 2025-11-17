// <copyright file="StockItemsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class StockItemsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private StockItems stockItems = null!;
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
        this.stockItems = new StockItems(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task CreateAsync_WithValidItem_ReturnsCreatedItem()
    {
        // Arrange
        StockItem inputItem = new()
        {
            Description = "Widget Model A",
            OpeningQuantity = 100.00m,
            OpeningBalance = 1000.00m,
            StockItemType = "Product",
            CostOfSaleCategory = new Uri("https://api.freeagent.com/v2/categories/100"),
            StockOnHandCategory = new Uri("https://api.freeagent.com/v2/categories/101")
        };

        StockItem responseItem = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/stock_items/123"),
            Description = "Widget Model A",
            OpeningQuantity = 100.00m,
            OpeningBalance = 1000.00m,
            StockItemType = "Product",
            CostOfSaleCategory = new Uri("https://api.freeagent.com/v2/categories/100"),
            StockOnHandCategory = new Uri("https://api.freeagent.com/v2/categories/101")
        };

        StockItemRoot responseRoot = new() { StockItem = responseItem };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        StockItem result = await this.stockItems.CreateAsync(inputItem);

        // Assert
        result.ShouldNotBeNull();
        result.Url.ShouldNotBeNull();
        result.Description.ShouldBe("Widget Model A");
        result.OpeningQuantity.ShouldBe(100.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/stock_items");
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllItems()
    {
        // Arrange
        List<StockItem> itemsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/stock_items/1"),
                Description = "Widget Model A",
                OpeningQuantity = 100.00m,
                OpeningBalance = 1000.00m
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/stock_items/2"),
                Description = "Widget Model B",
                OpeningQuantity = 200.00m,
                OpeningBalance = 2500.00m
            }
        ];

        StockItemsRoot responseRoot = new() { StockItems = itemsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<StockItem> result = await this.stockItems.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/stock_items");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsItem()
    {
        // Arrange
        StockItem item = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/stock_items/456"),
            Description = "Premium Coffee Beans 1kg",
            OpeningQuantity = 50.00m,
            OpeningBalance = 750.00m,
            StockItemType = "Product"
        };

        StockItemRoot responseRoot = new() { StockItem = item };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        StockItem result = await this.stockItems.GetByIdAsync("456");

        // Assert
        result.ShouldNotBeNull();
        result.Description.ShouldBe("Premium Coffee Beans 1kg");
        result.OpeningQuantity.ShouldBe(50.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/stock_items/456");
    }

    [TestMethod]
    public async Task UpdateAsync_WithValidItem_ReturnsUpdatedItem()
    {
        // Arrange
        StockItem updatedItem = new()
        {
            Description = "Widget Model A - Updated",
            OpeningQuantity = 150.00m,
            OpeningBalance = 1500.00m
        };

        StockItem responseItem = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/stock_items/789"),
            Description = "Widget Model A - Updated",
            OpeningQuantity = 150.00m,
            OpeningBalance = 1500.00m,
            StockItemType = "Product"
        };

        StockItemRoot responseRoot = new() { StockItem = responseItem };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        StockItem result = await this.stockItems.UpdateAsync("789", updatedItem);

        // Assert
        result.ShouldNotBeNull();
        result.Description.ShouldBe("Widget Model A - Updated");
        result.OpeningQuantity.ShouldBe(150.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/stock_items/789");
    }

    [TestMethod]
    public async Task DeleteAsync_WithValidId_DeletesItem()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        await this.stockItems.DeleteAsync("999");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/stock_items/999");
    }
}
