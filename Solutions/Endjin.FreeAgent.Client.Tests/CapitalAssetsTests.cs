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
    public async Task GetAllAsync_WithoutParameters_ReturnsAllAssets()
    {
        // Arrange
        List<CapitalAsset> assetsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/capital_assets/1"),
                Description = "Dell Laptop",
                AssetType = "Computer Equipment",
                PurchasedOn = new DateOnly(2024, 1, 15)
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/capital_assets/2"),
                Description = "Office Furniture",
                AssetType = "Fixtures and Fittings",
                PurchasedOn = new DateOnly(2024, 2, 10)
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
    public async Task GetAllAsync_WithViewParameter_ReturnsFilteredAssets()
    {
        // Arrange
        List<CapitalAsset> assetsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/capital_assets/1"),
                Description = "Disposed Laptop",
                AssetType = "Computer Equipment",
                PurchasedOn = new DateOnly(2024, 1, 15),
                DisposedOn = new DateOnly(2024, 12, 31)
            }
        ];

        CapitalAssetsRoot responseRoot = new() { CapitalAssets = assetsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<CapitalAsset> result = await this.capitalAssets.GetAllAsync(view: "disposed");

        // Assert
        result.Count().ShouldBe(1);
        result.First().DisposedOn.ShouldNotBeNull();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/capital_assets?view=disposed");
    }

    [TestMethod]
    public async Task GetAllAsync_WithIncludeHistory_ReturnsAssetsWithHistory()
    {
        // Arrange
        List<CapitalAsset> assetsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/capital_assets/1"),
                Description = "Dell Laptop",
                AssetType = "Computer Equipment",
                PurchasedOn = new DateOnly(2024, 1, 15),
                CapitalAssetHistory =
                [
                    new()
                    {
                        Type = "purchase",
                        Description = "Initial purchase",
                        Date = new DateOnly(2024, 1, 15),
                        Value = 1200.00m
                    }
                ]
            }
        ];

        CapitalAssetsRoot responseRoot = new() { CapitalAssets = assetsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<CapitalAsset> result = await this.capitalAssets.GetAllAsync(includeHistory: true);

        // Assert
        result.Count().ShouldBe(1);
        result.First().CapitalAssetHistory.ShouldNotBeNull();
        result.First().CapitalAssetHistory!.Length.ShouldBe(1);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/capital_assets?include_history=true");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithoutHistory_ReturnsAsset()
    {
        // Arrange
        CapitalAsset asset = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/capital_assets/456"),
            Description = "Company Vehicle",
            AssetType = "Motor Vehicles",
            PurchasedOn = new DateOnly(2024, 1, 10),
#pragma warning disable CS0618 // Type or member is obsolete
            AssetLifeYears = 5
#pragma warning restore CS0618 // Type or member is obsolete
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

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/capital_assets/456");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithHistory_ReturnsAssetWithHistory()
    {
        // Arrange
        CapitalAsset asset = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/capital_assets/456"),
            Description = "Company Vehicle",
            AssetType = "Motor Vehicles",
            PurchasedOn = new DateOnly(2024, 1, 10),
            CapitalAssetHistory =
            [
                new()
                {
                    Type = "purchase",
                    Description = "Vehicle purchase",
                    Date = new DateOnly(2024, 1, 10),
                    Value = 25000.00m
                },
                new()
                {
                    Type = "depreciation",
                    Description = "Annual depreciation",
                    Date = new DateOnly(2024, 12, 31),
                    Value = 5000.00m
                }
            ]
        };

        CapitalAssetRoot responseRoot = new() { CapitalAsset = asset };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        CapitalAsset result = await this.capitalAssets.GetByIdAsync("456", includeHistory: true);

        // Assert
        result.ShouldNotBeNull();
        result.Description.ShouldBe("Company Vehicle");
        result.CapitalAssetHistory.ShouldNotBeNull();
        result.CapitalAssetHistory!.Length.ShouldBe(2);
        result.CapitalAssetHistory[0].Type.ShouldBe("purchase");
        result.CapitalAssetHistory[1].Type.ShouldBe("depreciation");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/capital_assets/456?include_history=true");
    }

    [TestMethod]
    public async Task GetAllAsync_WithViewParameterAll_ReturnsAllAssets()
    {
        // Arrange
        List<CapitalAsset> assetsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/capital_assets/1"),
                Description = "Active Laptop",
                AssetType = "Computer Equipment",
                PurchasedOn = new DateOnly(2024, 1, 15)
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/capital_assets/2"),
                Description = "Disposed Furniture",
                AssetType = "Fixtures and Fittings",
                PurchasedOn = new DateOnly(2023, 1, 10),
                DisposedOn = new DateOnly(2024, 12, 31)
            }
        ];

        CapitalAssetsRoot responseRoot = new() { CapitalAssets = assetsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<CapitalAsset> result = await this.capitalAssets.GetAllAsync(view: "all");

        // Assert
        result.Count().ShouldBe(2);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/capital_assets?view=all");
    }

    [TestMethod]
    public async Task GetAllAsync_WithViewParameterDisposable_ReturnsDisposableAssets()
    {
        // Arrange
        List<CapitalAsset> assetsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/capital_assets/1"),
                Description = "Fully Depreciated Equipment",
                AssetType = "Computer Equipment",
                PurchasedOn = new DateOnly(2020, 1, 15)
            }
        ];

        CapitalAssetsRoot responseRoot = new() { CapitalAssets = assetsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<CapitalAsset> result = await this.capitalAssets.GetAllAsync(view: "disposable");

        // Assert
        result.Count().ShouldBe(1);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/capital_assets?view=disposable");
    }

    [TestMethod]
    public async Task GetAllAsync_WithViewAndIncludeHistory_ReturnsCombinedResults()
    {
        // Arrange
        List<CapitalAsset> assetsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/capital_assets/1"),
                Description = "Disposed Laptop",
                AssetType = "Computer Equipment",
                PurchasedOn = new DateOnly(2024, 1, 15),
                DisposedOn = new DateOnly(2024, 12, 31),
                CapitalAssetHistory =
                [
                    new()
                    {
                        Type = "purchase",
                        Description = "Initial purchase",
                        Date = new DateOnly(2024, 1, 15),
                        Value = 1200.00m
                    },
                    new()
                    {
                        Type = "disposal",
                        Description = "Asset disposal",
                        Date = new DateOnly(2024, 12, 31),
                        Value = 200.00m
                    }
                ]
            }
        ];

        CapitalAssetsRoot responseRoot = new() { CapitalAssets = assetsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<CapitalAsset> result = await this.capitalAssets.GetAllAsync(view: "disposed", includeHistory: true);

        // Assert
        result.Count().ShouldBe(1);
        result.First().DisposedOn.ShouldNotBeNull();
        result.First().CapitalAssetHistory.ShouldNotBeNull();
        result.First().CapitalAssetHistory!.Length.ShouldBe(2);
        result.First().CapitalAssetHistory![1].Type.ShouldBe("disposal");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/capital_assets?view=disposed&include_history=true");
    }
}
