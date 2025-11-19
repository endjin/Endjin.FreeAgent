// <copyright file="StockItemsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Corvus.Retry.Policies;
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

        // Disable retries for tests to speed them up
        this.freeAgentClient.RetryPolicy = Substitute.For<IRetryPolicy>();

        this.stockItems = new StockItems(this.freeAgentClient, this.cache);
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
                OpeningQuantity = 100,
                OpeningBalance = 1000.00m,
                CostOfSaleCategory = new Uri("https://api.freeagent.com/v2/categories/100"),
                StockOnHand = 95,
                CreatedAt = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 6, 20, 14, 45, 0, DateTimeKind.Utc)
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/stock_items/2"),
                Description = "Widget Model B",
                OpeningQuantity = 200,
                OpeningBalance = 2500.00m,
                CostOfSaleCategory = new Uri("https://api.freeagent.com/v2/categories/101"),
                StockOnHand = 180,
                CreatedAt = new DateTime(2024, 2, 10, 9, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 7, 5, 16, 30, 0, DateTimeKind.Utc)
            }
        ];

        StockItemsRoot responseRoot = new() { StockItems = itemsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.SourceGenOptions);

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
            OpeningQuantity = 50,
            OpeningBalance = 750.00m,
            CostOfSaleCategory = new Uri("https://api.freeagent.com/v2/categories/100"),
            StockOnHand = 45,
            CreatedAt = new DateTime(2024, 3, 1, 8, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2024, 8, 15, 12, 0, 0, DateTimeKind.Utc)
        };

        StockItemRoot responseRoot = new() { StockItem = item };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.SourceGenOptions);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        StockItem result = await this.stockItems.GetByIdAsync("456");

        // Assert
        result.ShouldNotBeNull();
        result.Description.ShouldBe("Premium Coffee Beans 1kg");
        result.OpeningQuantity.ShouldBe(50);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/stock_items/456");
    }

    [TestMethod]
    public async Task GetAllAsync_WithSortParameter_PassesSortToApi()
    {
        // Arrange
        List<StockItem> itemsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/stock_items/1"),
                Description = "Widget A",
                OpeningQuantity = 100,
                OpeningBalance = 1000.00m,
                CostOfSaleCategory = new Uri("https://api.freeagent.com/v2/categories/100"),
                StockOnHand = 95,
                CreatedAt = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 6, 20, 14, 45, 0, DateTimeKind.Utc)
            }
        ];

        StockItemsRoot responseRoot = new() { StockItems = itemsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.SourceGenOptions);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<StockItem> result = await this.stockItems.GetAllAsync("created_at");

        // Assert
        result.Count().ShouldBe(1);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/stock_items?sort=created_at");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithNullId_ThrowsArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () => await this.stockItems.GetByIdAsync(null!));
    }

    [TestMethod]
    public async Task GetByIdAsync_WithEmptyId_ThrowsArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () => await this.stockItems.GetByIdAsync(string.Empty));
    }

    [TestMethod]
    public async Task GetByIdAsync_WithWhitespaceId_ThrowsArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () => await this.stockItems.GetByIdAsync("   "));
    }

    [TestMethod]
    public async Task GetByIdAsync_WhenItemNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        StockItemRoot responseRoot = new() { StockItem = null };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.SourceGenOptions);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(async () => await this.stockItems.GetByIdAsync("999"));
    }

    [TestMethod]
    public async Task GetByIdAsync_ReturnsItemWithAllFields()
    {
        // Arrange
        DateTime createdAt = new(2024, 3, 1, 8, 0, 0, DateTimeKind.Utc);
        DateTime updatedAt = new(2024, 8, 15, 12, 0, 0, DateTimeKind.Utc);

        StockItem item = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/stock_items/789"),
            Description = "Complete Stock Item",
            OpeningQuantity = 100,
            OpeningBalance = 5000.00m,
            CostOfSaleCategory = new Uri("https://api.freeagent.com/v2/categories/200"),
            StockOnHand = 85,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        StockItemRoot responseRoot = new() { StockItem = item };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.SourceGenOptions);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        StockItem result = await this.stockItems.GetByIdAsync("789");

        // Assert
        result.ShouldNotBeNull();
        result.Url.ShouldBe(new Uri("https://api.freeagent.com/v2/stock_items/789"));
        result.Description.ShouldBe("Complete Stock Item");
        result.OpeningQuantity.ShouldBe(100);
        result.OpeningBalance.ShouldBe(5000.00m);
        result.CostOfSaleCategory.ShouldBe(new Uri("https://api.freeagent.com/v2/categories/200"));
        result.StockOnHand.ShouldBe(85);
        result.CreatedAt.ShouldBe(createdAt);
        result.UpdatedAt.ShouldBe(updatedAt);
    }

    [TestMethod]
    public async Task GetAllAsync_WithDescendingSortParameter_PassesSortToApi()
    {
        // Arrange
        List<StockItem> itemsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/stock_items/1"),
                Description = "Widget A",
                OpeningQuantity = 100,
                OpeningBalance = 1000.00m,
                CostOfSaleCategory = new Uri("https://api.freeagent.com/v2/categories/100"),
                StockOnHand = 95,
                CreatedAt = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 6, 20, 14, 45, 0, DateTimeKind.Utc)
            }
        ];

        StockItemsRoot responseRoot = new() { StockItems = itemsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.SourceGenOptions);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<StockItem> result = await this.stockItems.GetAllAsync("-created_at");

        // Assert
        result.Count().ShouldBe(1);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/stock_items?sort=-created_at");
    }

    [TestMethod]
    public async Task GetAllAsync_WhenNoItems_ReturnsEmptyCollection()
    {
        // Arrange
        StockItemsRoot responseRoot = new() { StockItems = [] };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.SourceGenOptions);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<StockItem> result = await this.stockItems.GetAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [TestMethod]
    public async Task GetAllAsync_CachesResults()
    {
        // Arrange
        List<StockItem> itemsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/stock_items/1"),
                Description = "Widget A",
                OpeningQuantity = 100,
                OpeningBalance = 1000.00m,
                CostOfSaleCategory = new Uri("https://api.freeagent.com/v2/categories/100"),
                StockOnHand = 95,
                CreatedAt = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 6, 20, 14, 45, 0, DateTimeKind.Utc)
            }
        ];

        StockItemsRoot responseRoot = new() { StockItems = itemsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.SourceGenOptions);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<StockItem> firstResult = await this.stockItems.GetAllAsync();
        IEnumerable<StockItem> secondResult = await this.stockItems.GetAllAsync();

        // Assert
        firstResult.Count().ShouldBe(1);
        secondResult.Count().ShouldBe(1);

        // Mock Verification - API should only be called once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task GetByIdAsync_CachesResults()
    {
        // Arrange
        StockItem item = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/stock_items/123"),
            Description = "Cached Item",
            OpeningQuantity = 50,
            OpeningBalance = 500.00m,
            CostOfSaleCategory = new Uri("https://api.freeagent.com/v2/categories/100"),
            StockOnHand = 45,
            CreatedAt = new DateTime(2024, 3, 1, 8, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2024, 8, 15, 12, 0, 0, DateTimeKind.Utc)
        };

        StockItemRoot responseRoot = new() { StockItem = item };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.SourceGenOptions);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        StockItem firstResult = await this.stockItems.GetByIdAsync("123");
        StockItem secondResult = await this.stockItems.GetByIdAsync("123");

        // Assert
        firstResult.Description.ShouldBe("Cached Item");
        secondResult.Description.ShouldBe("Cached Item");

        // Mock Verification - API should only be called once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task GetAllAsync_WhenApiReturnsError_ThrowsHttpRequestException()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("Internal Server Error", Encoding.UTF8, "text/plain")
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(async () => await this.stockItems.GetAllAsync());
    }

    [TestMethod]
    public async Task GetByIdAsync_WhenApiReturnsError_ThrowsHttpRequestException()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("Internal Server Error", Encoding.UTF8, "text/plain")
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(async () => await this.stockItems.GetByIdAsync("123"));
    }
}
