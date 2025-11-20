// <copyright file="CapitalAssetTypesTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class CapitalAssetTypesTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private CapitalAssetTypes capitalAssetTypes = null!;
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
        this.capitalAssetTypes = new CapitalAssetTypes(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllAssetTypes()
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
                Name = "Fixtures and Fittings",
                SystemDefault = true,
                CreatedAt = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
                UpdatedAt = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/capital_asset_types/3"),
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
        IEnumerable<CapitalAssetType> result = await this.capitalAssetTypes.GetAllAsync();

        // Assert
        result.Count().ShouldBe(3);
        result.First().Name.ShouldBe("Computer Equipment");
        result.First().SystemDefault.ShouldBe(true);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/capital_asset_types");
    }

    [TestMethod]
    public async Task GetByIdAsync_ReturnsCapitalAssetType()
    {
        // Arrange
        CapitalAssetType assetType = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/capital_asset_types/123"),
            Name = "Custom Equipment",
            SystemDefault = false,
            CreatedAt = new DateTimeOffset(2024, 6, 15, 10, 30, 0, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2024, 6, 15, 10, 30, 0, TimeSpan.Zero)
        };

        CapitalAssetTypeRoot responseRoot = new() { CapitalAssetType = assetType };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        CapitalAssetType result = await this.capitalAssetTypes.GetByIdAsync("123");

        // Assert
        result.Name.ShouldBe("Custom Equipment");
        result.SystemDefault.ShouldBe(false);
        result.Url.ShouldBe(new Uri("https://api.freeagent.com/v2/capital_asset_types/123"));

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/capital_asset_types/123");
    }

    [TestMethod]
    public async Task GetByIdAsync_ThrowsArgumentException_WhenIdIsNullOrWhitespace()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.capitalAssetTypes.GetByIdAsync(string.Empty));

        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.capitalAssetTypes.GetByIdAsync("   "));
    }

    [TestMethod]
    public async Task CreateAsync_CreatesAndReturnsCapitalAssetType()
    {
        // Arrange
        string assetTypeName = "Laboratory Equipment";

        CapitalAssetType createdAssetType = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/capital_asset_types/456"),
            Name = assetTypeName,
            SystemDefault = false,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        CapitalAssetTypeRoot responseRoot = new() { CapitalAssetType = createdAssetType };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        CapitalAssetType result = await this.capitalAssetTypes.CreateAsync(assetTypeName);

        // Assert
        result.Name.ShouldBe(assetTypeName);
        result.SystemDefault.ShouldBe(false);
        result.Url.ShouldNotBeNull();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/capital_asset_types");
    }

    [TestMethod]
    public async Task CreateAsync_ThrowsArgumentException_WhenNameIsNullOrWhitespace()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.capitalAssetTypes.CreateAsync(string.Empty));

        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.capitalAssetTypes.CreateAsync("   "));
    }

    [TestMethod]
    public async Task UpdateAsync_UpdatesAndReturnsCapitalAssetType()
    {
        // Arrange
        string assetTypeId = "789";
        string updatedName = "Updated Equipment Name";

        CapitalAssetType updatedAssetType = new()
        {
            Url = new Uri($"https://api.freeagent.com/v2/capital_asset_types/{assetTypeId}"),
            Name = updatedName,
            SystemDefault = false,
            CreatedAt = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
            UpdatedAt = DateTimeOffset.UtcNow
        };

        CapitalAssetTypeRoot responseRoot = new() { CapitalAssetType = updatedAssetType };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        CapitalAssetType result = await this.capitalAssetTypes.UpdateAsync(assetTypeId, updatedName);

        // Assert
        result.Name.ShouldBe(updatedName);
        result.SystemDefault.ShouldBe(false);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri($"/v2/capital_asset_types/{assetTypeId}");
    }

    [TestMethod]
    public async Task UpdateAsync_ThrowsArgumentException_WhenIdIsNullOrWhitespace()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.capitalAssetTypes.UpdateAsync(string.Empty, "Valid Name"));

        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.capitalAssetTypes.UpdateAsync("   ", "Valid Name"));
    }

    [TestMethod]
    public async Task UpdateAsync_ThrowsArgumentException_WhenNameIsNullOrWhitespace()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.capitalAssetTypes.UpdateAsync("123", string.Empty));

        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.capitalAssetTypes.UpdateAsync("123", "   "));
    }

    [TestMethod]
    public async Task DeleteAsync_DeletesCapitalAssetType()
    {
        // Arrange
        string assetTypeId = "999";

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK);

        // Act
        await this.capitalAssetTypes.DeleteAsync(assetTypeId);

        // Assert
        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri($"/v2/capital_asset_types/{assetTypeId}");
    }

    [TestMethod]
    public async Task DeleteAsync_ThrowsArgumentException_WhenIdIsNullOrWhitespace()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.capitalAssetTypes.DeleteAsync(string.Empty));

        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.capitalAssetTypes.DeleteAsync("   "));
    }
}
