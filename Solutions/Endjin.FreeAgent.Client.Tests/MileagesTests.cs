// <copyright file="MileagesTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class MileagesTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private Mileages mileages = null!;
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
        this.mileages = new Mileages(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task CreateAsync_WithValidMileage_ReturnsCreatedMileage()
    {
        // Arrange
        Mileage inputMileage = new()
        {
            User = new Uri("https://api.freeagent.com/v2/users/123"),
            DatedOn = new DateOnly(2024, 1, 15),
            Miles = 125.5m,
            Description = "Client meeting in London"
        };

        Mileage responseMileage = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/mileages/456"),
            User = new Uri("https://api.freeagent.com/v2/users/123"),
            DatedOn = new DateOnly(2024, 1, 15),
            Miles = 125.5m,
            Description = "Client meeting in London"
        };

        MileageRoot responseRoot = new() { Mileage = responseMileage };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Mileage result = await this.mileages.CreateAsync(inputMileage);

        // Assert
        result.ShouldNotBeNull();
        result.Url.ShouldNotBeNull();
        result.Miles.ShouldBe(125.5m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/mileages");
    }

    [TestMethod]
    public async Task GetAllAsync_WithoutFilters_ReturnsAllMileages()
    {
        // Arrange
        List<Mileage> mileagesList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/mileages/1"),
                DatedOn = new DateOnly(2024, 1, 15),
                Miles = 125.5m
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/mileages/2"),
                DatedOn = new DateOnly(2024, 1, 20),
                Miles = 87.3m
            }
        ];

        MileagesRoot responseRoot = new() { Mileages = mileagesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Mileage> result = await this.mileages.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/mileages");
    }

    [TestMethod]
    public async Task GetAllAsync_WithUserFilter_ReturnsFilteredMileages()
    {
        // Arrange
        List<Mileage> mileagesList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/mileages/1"),
                User = new Uri("https://api.freeagent.com/v2/users/123"),
                Miles = 125.5m
            }
        ];

        MileagesRoot responseRoot = new() { Mileages = mileagesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Mileage> result = await this.mileages.GetAllAsync(userId: "123");

        // Assert
        result.Count().ShouldBe(1);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/mileages?user=123");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsMileage()
    {
        // Arrange
        Mileage mileage = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/mileages/789"),
            DatedOn = new DateOnly(2024, 1, 15),
            Miles = 125.5m,
            Description = "Client meeting"
        };

        MileageRoot responseRoot = new() { Mileage = mileage };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Mileage result = await this.mileages.GetByIdAsync("789");

        // Assert
        result.ShouldNotBeNull();
        result.Miles.ShouldBe(125.5m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/mileages/789");
    }

    [TestMethod]
    public async Task UpdateAsync_WithValidMileage_ReturnsUpdatedMileage()
    {
        // Arrange
        Mileage updatedMileage = new()
        {
            Miles = 130.0m,
            Description = "Client meeting in London - Updated"
        };

        Mileage responseMileage = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/mileages/999"),
            DatedOn = new DateOnly(2024, 1, 15),
            Miles = 130.0m,
            Description = "Client meeting in London - Updated"
        };

        MileageRoot responseRoot = new() { Mileage = responseMileage };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Mileage result = await this.mileages.UpdateAsync("999", updatedMileage);

        // Assert
        result.ShouldNotBeNull();
        result.Miles.ShouldBe(130.0m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/mileages/999");
    }

    [TestMethod]
    public async Task DeleteAsync_WithValidId_DeletesMileage()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        await this.mileages.DeleteAsync("888");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/mileages/888");
    }
}
