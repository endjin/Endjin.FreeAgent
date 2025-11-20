// <copyright file="PriceListItemsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class PriceListItemsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private PriceListItems priceListItems = null!;
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
        this.priceListItems = new PriceListItems(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task CreateAsync_WithValidItem_ReturnsCreatedItem()
    {
        // Arrange
        PriceListItem inputItem = new()
        {
            Code = "CONSULT-HR",
            Quantity = 1.00m,
            ItemType = "Hours",
            Description = "Consulting Services (hourly)",
            Price = 150.00m,
            Category = new Uri("https://api.freeagent.com/v2/categories/200")
        };

        PriceListItem responseItem = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/price_list_items/123"),
            Code = "CONSULT-HR",
            Quantity = 1.00m,
            ItemType = "Hours",
            Description = "Consulting Services (hourly)",
            Price = 150.00m,
            Category = new Uri("https://api.freeagent.com/v2/categories/200"),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        PriceListItemRoot responseRoot = new() { PriceListItem = responseItem };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        PriceListItem result = await this.priceListItems.CreateAsync(inputItem);

        // Assert
        result.ShouldNotBeNull();
        result.Url.ShouldNotBeNull();
        result.Code.ShouldBe("CONSULT-HR");
        result.Quantity.ShouldBe(1.00m);
        result.ItemType.ShouldBe("Hours");
        result.Description.ShouldBe("Consulting Services (hourly)");
        result.Price.ShouldBe(150.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/price_list_items");
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllItems()
    {
        // Arrange
        List<PriceListItem> itemsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/price_list_items/1"),
                Code = "CONSULT-HR",
                Quantity = 1.00m,
                ItemType = "Hours",
                Description = "Consulting Services (hourly)",
                Price = 150.00m
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/price_list_items/2"),
                Code = "WIDGET-A",
                Quantity = 10.00m,
                ItemType = "Products",
                Description = "Widget Model A",
                Price = 25.00m
            }
        ];

        PriceListItemsRoot responseRoot = new() { PriceListItems = itemsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<PriceListItem> result = await this.priceListItems.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/price_list_items");
    }

    [TestMethod]
    public async Task GetAllAsync_WithSortParameter_PassesSortToApi()
    {
        // Arrange
        List<PriceListItem> itemsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/price_list_items/1"),
                Code = "AAA-FIRST",
                Quantity = 1.00m,
                ItemType = "Hours",
                Description = "First item alphabetically"
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/price_list_items/2"),
                Code = "ZZZ-LAST",
                Quantity = 1.00m,
                ItemType = "Products",
                Description = "Last item alphabetically"
            }
        ];

        PriceListItemsRoot responseRoot = new() { PriceListItems = itemsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<PriceListItem> result = await this.priceListItems.GetAllAsync(sort: "code");

        // Assert
        result.Count().ShouldBe(2);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/price_list_items?sort=code");
    }

    [TestMethod]
    public async Task GetAllAsync_WithDescendingSortParameter_PassesSortToApi()
    {
        // Arrange
        List<PriceListItem> itemsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/price_list_items/2"),
                Code = "ZZZ-LAST",
                Quantity = 1.00m,
                ItemType = "Products",
                Description = "Last item alphabetically"
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/price_list_items/1"),
                Code = "AAA-FIRST",
                Quantity = 1.00m,
                ItemType = "Hours",
                Description = "First item alphabetically"
            }
        ];

        PriceListItemsRoot responseRoot = new() { PriceListItems = itemsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<PriceListItem> result = await this.priceListItems.GetAllAsync(sort: "-code");

        // Assert
        result.Count().ShouldBe(2);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/price_list_items?sort=-code");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsItem()
    {
        // Arrange
        PriceListItem item = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/price_list_items/456"),
            Code = "PREMIUM-SVC",
            Quantity = 5.00m,
            ItemType = "Services",
            Description = "Premium Service Package",
            Price = 500.00m,
            VatStatus = "standard"
        };

        PriceListItemRoot responseRoot = new() { PriceListItem = item };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        PriceListItem result = await this.priceListItems.GetByIdAsync("456");

        // Assert
        result.ShouldNotBeNull();
        result.Code.ShouldBe("PREMIUM-SVC");
        result.Quantity.ShouldBe(5.00m);
        result.ItemType.ShouldBe("Services");
        result.Description.ShouldBe("Premium Service Package");
        result.Price.ShouldBe(500.00m);
        result.VatStatus.ShouldBe("standard");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/price_list_items/456");
    }

    [TestMethod]
    public async Task UpdateAsync_WithValidItem_ReturnsUpdatedItem()
    {
        // Arrange
        PriceListItem updatedItem = new()
        {
            Code = "CONSULT-HR",
            Quantity = 2.00m,
            ItemType = "Hours",
            Description = "Consulting Services (hourly) - Updated",
            Price = 175.00m
        };

        PriceListItem responseItem = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/price_list_items/789"),
            Code = "CONSULT-HR",
            Quantity = 2.00m,
            ItemType = "Hours",
            Description = "Consulting Services (hourly) - Updated",
            Price = 175.00m,
            UpdatedAt = DateTime.UtcNow
        };

        PriceListItemRoot responseRoot = new() { PriceListItem = responseItem };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        PriceListItem result = await this.priceListItems.UpdateAsync("789", updatedItem);

        // Assert
        result.ShouldNotBeNull();
        result.Code.ShouldBe("CONSULT-HR");
        result.Quantity.ShouldBe(2.00m);
        result.Description.ShouldBe("Consulting Services (hourly) - Updated");
        result.Price.ShouldBe(175.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/price_list_items/789");
    }

    [TestMethod]
    public async Task DeleteAsync_WithValidId_DeletesItem()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        await this.priceListItems.DeleteAsync("999");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/price_list_items/999");
    }

    [TestMethod]
    public async Task CreateAsync_WithStockItem_ReturnsCreatedItemWithStockReference()
    {
        // Arrange
        PriceListItem inputItem = new()
        {
            Code = "STOCK-ITEM-A",
            Quantity = 5.00m,
            ItemType = "Stock",
            Description = "Stock Item from Inventory",
            Price = 45.00m,
            StockItem = new Uri("https://api.freeagent.com/v2/stock_items/100")
        };

        PriceListItem responseItem = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/price_list_items/321"),
            Code = "STOCK-ITEM-A",
            Quantity = 5.00m,
            ItemType = "Stock",
            Description = "Stock Item from Inventory",
            Price = 45.00m,
            StockItem = new Uri("https://api.freeagent.com/v2/stock_items/100"),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        PriceListItemRoot responseRoot = new() { PriceListItem = responseItem };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        PriceListItem result = await this.priceListItems.CreateAsync(inputItem);

        // Assert
        result.ShouldNotBeNull();
        result.ItemType.ShouldBe("Stock");
        result.StockItem.ShouldNotBeNull();
        result.StockItem.ToString().ShouldContain("stock_items/100");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/price_list_items");
    }

    [TestMethod]
    public async Task CreateAsync_WithSalesTaxRate_ReturnsCreatedItemWithTaxRate()
    {
        // Arrange
        PriceListItem inputItem = new()
        {
            Code = "US-PRODUCT",
            Quantity = 1.00m,
            ItemType = "Products",
            Description = "US Product with Sales Tax",
            Price = 99.99m,
            SalesTaxRate = 8.25m
        };

        PriceListItem responseItem = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/price_list_items/555"),
            Code = "US-PRODUCT",
            Quantity = 1.00m,
            ItemType = "Products",
            Description = "US Product with Sales Tax",
            Price = 99.99m,
            SalesTaxRate = 8.25m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        PriceListItemRoot responseRoot = new() { PriceListItem = responseItem };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        PriceListItem result = await this.priceListItems.CreateAsync(inputItem);

        // Assert
        result.ShouldNotBeNull();
        result.SalesTaxRate.ShouldBe(8.25m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
    }

    [TestMethod]
    public async Task CreateAsync_WithNullItem_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await this.priceListItems.CreateAsync(null!));
    }

    [TestMethod]
    public async Task GetByIdAsync_WithNullId_ThrowsArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.priceListItems.GetByIdAsync(null!));
    }

    [TestMethod]
    public async Task GetByIdAsync_WithEmptyId_ThrowsArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.priceListItems.GetByIdAsync(string.Empty));
    }

    [TestMethod]
    public async Task GetByIdAsync_WithWhitespaceId_ThrowsArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.priceListItems.GetByIdAsync("   "));
    }

    [TestMethod]
    public async Task UpdateAsync_WithNullId_ThrowsArgumentException()
    {
        // Arrange
        PriceListItem item = new()
        {
            Code = "TEST",
            Quantity = 1.00m,
            ItemType = "Hours",
            Description = "Test"
        };

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.priceListItems.UpdateAsync(null!, item));
    }

    [TestMethod]
    public async Task UpdateAsync_WithEmptyId_ThrowsArgumentException()
    {
        // Arrange
        PriceListItem item = new()
        {
            Code = "TEST",
            Quantity = 1.00m,
            ItemType = "Hours",
            Description = "Test"
        };

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.priceListItems.UpdateAsync(string.Empty, item));
    }

    [TestMethod]
    public async Task UpdateAsync_WithNullItem_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await this.priceListItems.UpdateAsync("123", null!));
    }

    [TestMethod]
    public async Task DeleteAsync_WithNullId_ThrowsArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.priceListItems.DeleteAsync(null!));
    }

    [TestMethod]
    public async Task DeleteAsync_WithEmptyId_ThrowsArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.priceListItems.DeleteAsync(string.Empty));
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsCachedDataOnSecondCall()
    {
        // Arrange
        List<PriceListItem> itemsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/price_list_items/1"),
                Code = "CACHED-ITEM",
                Quantity = 1.00m,
                ItemType = "Hours",
                Description = "Cached item"
            }
        ];

        PriceListItemsRoot responseRoot = new() { PriceListItems = itemsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<PriceListItem> firstResult = await this.priceListItems.GetAllAsync();
        IEnumerable<PriceListItem> secondResult = await this.priceListItems.GetAllAsync();

        // Assert
        firstResult.Count().ShouldBe(1);
        secondResult.Count().ShouldBe(1);

        // Mock Verification - Should only be called once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task GetByIdAsync_ReturnsCachedDataOnSecondCall()
    {
        // Arrange
        PriceListItem item = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/price_list_items/123"),
            Code = "CACHED-BY-ID",
            Quantity = 1.00m,
            ItemType = "Hours",
            Description = "Cached by ID"
        };

        PriceListItemRoot responseRoot = new() { PriceListItem = item };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        PriceListItem firstResult = await this.priceListItems.GetByIdAsync("123");
        PriceListItem secondResult = await this.priceListItems.GetByIdAsync("123");

        // Assert
        firstResult.Code.ShouldBe("CACHED-BY-ID");
        secondResult.Code.ShouldBe("CACHED-BY-ID");

        // Mock Verification - Should only be called once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task CreateAsync_InvalidatesGetAllCache()
    {
        // Arrange - First populate the cache with GetAllAsync
        List<PriceListItem> initialList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/price_list_items/1"),
                Code = "INITIAL",
                Quantity = 1.00m,
                ItemType = "Hours",
                Description = "Initial item"
            }
        ];

        PriceListItemsRoot initialRoot = new() { PriceListItems = initialList };
        string initialJson = JsonSerializer.Serialize(initialRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(initialJson, Encoding.UTF8, "application/json")
        };

        await this.priceListItems.GetAllAsync();

        // Now create a new item
        PriceListItem newItem = new()
        {
            Code = "NEW-ITEM",
            Quantity = 1.00m,
            ItemType = "Hours",
            Description = "New item"
        };

        PriceListItem responseItem = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/price_list_items/2"),
            Code = "NEW-ITEM",
            Quantity = 1.00m,
            ItemType = "Hours",
            Description = "New item"
        };

        PriceListItemRoot createRoot = new() { PriceListItem = responseItem };
        string createJson = JsonSerializer.Serialize(createRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(createJson, Encoding.UTF8, "application/json")
        };

        await this.priceListItems.CreateAsync(newItem);

        // Now call GetAllAsync again - should make API call since cache was invalidated
        List<PriceListItem> updatedList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/price_list_items/1"),
                Code = "INITIAL",
                Quantity = 1.00m,
                ItemType = "Hours",
                Description = "Initial item"
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/price_list_items/2"),
                Code = "NEW-ITEM",
                Quantity = 1.00m,
                ItemType = "Hours",
                Description = "New item"
            }
        ];

        PriceListItemsRoot updatedRoot = new() { PriceListItems = updatedList };
        string updatedJson = JsonSerializer.Serialize(updatedRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(updatedJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<PriceListItem> result = await this.priceListItems.GetAllAsync();

        // Assert - Should have 3 calls: GetAll, Create, GetAll (cache was invalidated)
        result.Count().ShouldBe(2);
        this.messageHandler.CallCount.ShouldBe(3);
    }

    [TestMethod]
    public async Task UpdateAsync_InvalidatesGetAllCache()
    {
        // Arrange - First populate the cache with GetAllAsync
        List<PriceListItem> initialList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/price_list_items/1"),
                Code = "TO-UPDATE",
                Quantity = 1.00m,
                ItemType = "Hours",
                Description = "Item to update"
            }
        ];

        PriceListItemsRoot initialRoot = new() { PriceListItems = initialList };
        string initialJson = JsonSerializer.Serialize(initialRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(initialJson, Encoding.UTF8, "application/json")
        };

        await this.priceListItems.GetAllAsync();

        // Now update the item
        PriceListItem updatedItem = new()
        {
            Code = "UPDATED",
            Quantity = 2.00m,
            ItemType = "Hours",
            Description = "Updated item"
        };

        PriceListItem responseItem = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/price_list_items/1"),
            Code = "UPDATED",
            Quantity = 2.00m,
            ItemType = "Hours",
            Description = "Updated item"
        };

        PriceListItemRoot updateRoot = new() { PriceListItem = responseItem };
        string updateJson = JsonSerializer.Serialize(updateRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(updateJson, Encoding.UTF8, "application/json")
        };

        await this.priceListItems.UpdateAsync("1", updatedItem);

        // Now call GetAllAsync again - should make API call since cache was invalidated
        List<PriceListItem> refreshedList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/price_list_items/1"),
                Code = "UPDATED",
                Quantity = 2.00m,
                ItemType = "Hours",
                Description = "Updated item"
            }
        ];

        PriceListItemsRoot refreshedRoot = new() { PriceListItems = refreshedList };
        string refreshedJson = JsonSerializer.Serialize(refreshedRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(refreshedJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<PriceListItem> result = await this.priceListItems.GetAllAsync();

        // Assert - Should have 3 calls: GetAll, Update, GetAll (cache was invalidated)
        result.Count().ShouldBe(1);
        result.First().Code.ShouldBe("UPDATED");
        this.messageHandler.CallCount.ShouldBe(3);
    }

    [TestMethod]
    public async Task DeleteAsync_InvalidatesGetAllCache()
    {
        // Arrange - First populate the cache with GetAllAsync
        List<PriceListItem> initialList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/price_list_items/1"),
                Code = "TO-DELETE",
                Quantity = 1.00m,
                ItemType = "Hours",
                Description = "Item to delete"
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/price_list_items/2"),
                Code = "REMAINING",
                Quantity = 1.00m,
                ItemType = "Products",
                Description = "Remaining item"
            }
        ];

        PriceListItemsRoot initialRoot = new() { PriceListItems = initialList };
        string initialJson = JsonSerializer.Serialize(initialRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(initialJson, Encoding.UTF8, "application/json")
        };

        await this.priceListItems.GetAllAsync();

        // Now delete the item
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);

        await this.priceListItems.DeleteAsync("1");

        // Now call GetAllAsync again - should make API call since cache was invalidated
        List<PriceListItem> remainingList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/price_list_items/2"),
                Code = "REMAINING",
                Quantity = 1.00m,
                ItemType = "Products",
                Description = "Remaining item"
            }
        ];

        PriceListItemsRoot remainingRoot = new() { PriceListItems = remainingList };
        string remainingJson = JsonSerializer.Serialize(remainingRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(remainingJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<PriceListItem> result = await this.priceListItems.GetAllAsync();

        // Assert - Should have 3 calls: GetAll, Delete, GetAll (cache was invalidated)
        result.Count().ShouldBe(1);
        result.First().Code.ShouldBe("REMAINING");
        this.messageHandler.CallCount.ShouldBe(3);
    }

    [TestMethod]
    public async Task CreateAsync_WithSecondSalesTaxRate_ReturnsCreatedItemWithSecondTaxRate()
    {
        // Arrange
        PriceListItem inputItem = new()
        {
            Code = "DUAL-TAX-ITEM",
            Quantity = 1.00m,
            ItemType = "Products",
            Description = "Item with dual sales tax",
            Price = 199.99m,
            SalesTaxRate = 8.25m,
            SecondSalesTaxRate = 2.50m
        };

        PriceListItem responseItem = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/price_list_items/777"),
            Code = "DUAL-TAX-ITEM",
            Quantity = 1.00m,
            ItemType = "Products",
            Description = "Item with dual sales tax",
            Price = 199.99m,
            SalesTaxRate = 8.25m,
            SecondSalesTaxRate = 2.50m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        PriceListItemRoot responseRoot = new() { PriceListItem = responseItem };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        PriceListItem result = await this.priceListItems.CreateAsync(inputItem);

        // Assert
        result.ShouldNotBeNull();
        result.SalesTaxRate.ShouldBe(8.25m);
        result.SecondSalesTaxRate.ShouldBe(2.50m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
    }

    [TestMethod]
    public async Task CreateAsync_WithOutOfScopeVatStatus_ReturnsCreatedItem()
    {
        // Arrange
        PriceListItem inputItem = new()
        {
            Code = "OUT-OF-SCOPE",
            Quantity = 1.00m,
            ItemType = "Services",
            Description = "Out of scope service",
            Price = 100.00m,
            VatStatus = "out_of_scope"
        };

        PriceListItem responseItem = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/price_list_items/801"),
            Code = "OUT-OF-SCOPE",
            Quantity = 1.00m,
            ItemType = "Services",
            Description = "Out of scope service",
            Price = 100.00m,
            VatStatus = "out_of_scope",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        PriceListItemRoot responseRoot = new() { PriceListItem = responseItem };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        PriceListItem result = await this.priceListItems.CreateAsync(inputItem);

        // Assert
        result.ShouldNotBeNull();
        result.VatStatus.ShouldBe("out_of_scope");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
    }

    [TestMethod]
    public async Task CreateAsync_WithReducedVatStatus_ReturnsCreatedItem()
    {
        // Arrange
        PriceListItem inputItem = new()
        {
            Code = "REDUCED-VAT",
            Quantity = 1.00m,
            ItemType = "Products",
            Description = "Reduced VAT product",
            Price = 50.00m,
            VatStatus = "reduced"
        };

        PriceListItem responseItem = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/price_list_items/802"),
            Code = "REDUCED-VAT",
            Quantity = 1.00m,
            ItemType = "Products",
            Description = "Reduced VAT product",
            Price = 50.00m,
            VatStatus = "reduced",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        PriceListItemRoot responseRoot = new() { PriceListItem = responseItem };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        PriceListItem result = await this.priceListItems.CreateAsync(inputItem);

        // Assert
        result.ShouldNotBeNull();
        result.VatStatus.ShouldBe("reduced");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
    }

    [TestMethod]
    public async Task CreateAsync_WithZeroVatStatus_ReturnsCreatedItem()
    {
        // Arrange
        PriceListItem inputItem = new()
        {
            Code = "ZERO-VAT",
            Quantity = 1.00m,
            ItemType = "Products",
            Description = "Zero VAT product",
            Price = 75.00m,
            VatStatus = "zero"
        };

        PriceListItem responseItem = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/price_list_items/803"),
            Code = "ZERO-VAT",
            Quantity = 1.00m,
            ItemType = "Products",
            Description = "Zero VAT product",
            Price = 75.00m,
            VatStatus = "zero",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        PriceListItemRoot responseRoot = new() { PriceListItem = responseItem };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        PriceListItem result = await this.priceListItems.CreateAsync(inputItem);

        // Assert
        result.ShouldNotBeNull();
        result.VatStatus.ShouldBe("zero");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
    }

    [TestMethod]
    public async Task GetAllAsync_WithUpdatedAtSortParameter_PassesSortToApi()
    {
        // Arrange
        List<PriceListItem> itemsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/price_list_items/1"),
                Code = "OLDEST",
                Quantity = 1.00m,
                ItemType = "Hours",
                Description = "Oldest updated item",
                UpdatedAt = DateTime.UtcNow.AddDays(-7)
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/price_list_items/2"),
                Code = "NEWEST",
                Quantity = 1.00m,
                ItemType = "Products",
                Description = "Newest updated item",
                UpdatedAt = DateTime.UtcNow
            }
        ];

        PriceListItemsRoot responseRoot = new() { PriceListItems = itemsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<PriceListItem> result = await this.priceListItems.GetAllAsync(sort: "updated_at");

        // Assert
        result.Count().ShouldBe(2);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/price_list_items?sort=updated_at");
    }

    [TestMethod]
    public async Task GetAllAsync_WithDescendingUpdatedAtSortParameter_PassesSortToApi()
    {
        // Arrange
        List<PriceListItem> itemsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/price_list_items/2"),
                Code = "NEWEST",
                Quantity = 1.00m,
                ItemType = "Products",
                Description = "Newest updated item",
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/price_list_items/1"),
                Code = "OLDEST",
                Quantity = 1.00m,
                ItemType = "Hours",
                Description = "Oldest updated item",
                UpdatedAt = DateTime.UtcNow.AddDays(-7)
            }
        ];

        PriceListItemsRoot responseRoot = new() { PriceListItems = itemsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<PriceListItem> result = await this.priceListItems.GetAllAsync(sort: "-updated_at");

        // Assert
        result.Count().ShouldBe(2);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/price_list_items?sort=-updated_at");
    }
}
