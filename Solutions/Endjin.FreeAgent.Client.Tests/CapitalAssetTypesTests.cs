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
                Description = "Plant & Machinery",
                AllowanceType = "Main Rate Pool",
                Rate = 0.18m
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/capital_asset_types/2"),
                Description = "Fixtures & Fittings",
                AllowanceType = "Main Rate Pool",
                Rate = 0.18m
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/capital_asset_types/3"),
                Description = "Motor Vehicles",
                AllowanceType = "Main Rate Pool",
                Rate = 0.18m
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
        result.First().Description.ShouldBe("Plant & Machinery");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/capital_asset_types");
    }
}
