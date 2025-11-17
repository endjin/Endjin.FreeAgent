// <copyright file="CapitalAssetsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class CapitalAssetsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private CapitalAssets capitalAssets = null!;
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
        this.capitalAssets = new CapitalAssets(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task CreateAsync_WithValidAsset_ReturnsCreatedAsset()
    {
        // Arrange
        CapitalAsset inputAsset = new()
        {
            Description = "Dell Laptop",
            PurchasedOn = new DateOnly(2024, 1, 15),
            PurchasePrice = 1200.00m,
            AssetLifeYears = 3,
            CapitalAssetType = new Uri("https://api.freeagent.com/v2/capital_asset_types/1")
        };

        CapitalAsset responseAsset = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/capital_assets/123"),
            Description = "Dell Laptop",
            PurchasedOn = new DateOnly(2024, 1, 15),
            PurchasePrice = 1200.00m,
            AssetLifeYears = 3,
            CapitalAssetType = new Uri("https://api.freeagent.com/v2/capital_asset_types/1")
        };

        CapitalAssetRoot responseRoot = new() { CapitalAsset = responseAsset };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        CapitalAsset result = await this.capitalAssets.CreateAsync(inputAsset);

        // Assert
        result.ShouldNotBeNull();
        result.Url.ShouldNotBeNull();
        result.Description.ShouldBe("Dell Laptop");
        result.PurchasePrice.ShouldBe(1200.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/capital_assets");
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllAssets()
    {
        // Arrange
        List<CapitalAsset> assetsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/capital_assets/1"),
                Description = "Dell Laptop",
                PurchasedOn = new DateOnly(2024, 1, 15),
                PurchasePrice = 1200.00m
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/capital_assets/2"),
                Description = "Office Furniture",
                PurchasedOn = new DateOnly(2024, 2, 10),
                PurchasePrice = 2500.00m
            }
        ];

        CapitalAssetsRoot responseRoot = new() { CapitalAssets = assetsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<CapitalAsset> result = await this.capitalAssets.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/capital_assets");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsAsset()
    {
        // Arrange
        CapitalAsset asset = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/capital_assets/456"),
            Description = "Company Vehicle",
            PurchasedOn = new DateOnly(2024, 1, 10),
            PurchasePrice = 25000.00m,
            AssetLifeYears = 5
        };

        CapitalAssetRoot responseRoot = new() { CapitalAsset = asset };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        CapitalAsset result = await this.capitalAssets.GetByIdAsync("456");

        // Assert
        result.ShouldNotBeNull();
        result.Description.ShouldBe("Company Vehicle");
        result.PurchasePrice.ShouldBe(25000.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/capital_assets/456");
    }

    [TestMethod]
    public async Task UpdateAsync_WithValidAsset_ReturnsUpdatedAsset()
    {
        // Arrange
        CapitalAsset updatedAsset = new()
        {
            Description = "Dell Laptop - Updated",
            DisposedOn = new DateOnly(2024, 12, 31)
        };

        CapitalAsset responseAsset = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/capital_assets/789"),
            Description = "Dell Laptop - Updated",
            PurchasedOn = new DateOnly(2024, 1, 15),
            DisposedOn = new DateOnly(2024, 12, 31),
            PurchasePrice = 1200.00m
        };

        CapitalAssetRoot responseRoot = new() { CapitalAsset = responseAsset };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        CapitalAsset result = await this.capitalAssets.UpdateAsync("789", updatedAsset);

        // Assert
        result.ShouldNotBeNull();
        result.Description.ShouldBe("Dell Laptop - Updated");
        result.DisposedOn.ShouldBe(new DateOnly(2024, 12, 31));

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/capital_assets/789");
    }

    [TestMethod]
    public async Task DeleteAsync_WithValidId_DeletesAsset()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        await this.capitalAssets.DeleteAsync("999");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/capital_assets/999");
    }

    [TestMethod]
    public async Task GetTypesAsync_ReturnsAllAssetTypes()
    {
        // Arrange
        List<CapitalAssetType> typesList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/capital_asset_types/1"),
                Name = "Computer Equipment",
                SystemDefault = true,
                CreatedAt = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
                UpdatedAt = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/capital_asset_types/2"),
                Name = "Motor Vehicles",
                SystemDefault = true,
                CreatedAt = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
                UpdatedAt = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
            }
        ];

        CapitalAssetTypesRoot responseRoot = new() { CapitalAssetTypes = typesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<CapitalAssetType> result = await this.capitalAssets.GetTypesAsync();

        // Assert
        result.Count().ShouldBe(2);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/capital_asset_types");
    }
}
