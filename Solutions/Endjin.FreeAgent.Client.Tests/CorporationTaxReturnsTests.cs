// <copyright file="CorporationTaxReturnsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class CorporationTaxReturnsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private CorporationTaxReturns corporationTaxReturns = null!;
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
        this.corporationTaxReturns = new CorporationTaxReturns(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllReturns()
    {
        // Arrange
        List<CorporationTaxReturn> returnsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/corporation_tax_returns/1"),
                PeriodStartsOn = new DateOnly(2023, 4, 1),
                PeriodEndsOn = new DateOnly(2024, 3, 31)
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/corporation_tax_returns/2"),
                PeriodStartsOn = new DateOnly(2024, 4, 1),
                PeriodEndsOn = new DateOnly(2025, 3, 31)
            }
        ];

        CorporationTaxReturnsRoot responseRoot = new() { CorporationTaxReturns = returnsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<CorporationTaxReturn> result = await this.corporationTaxReturns.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/corporation_tax_returns");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsReturn()
    {
        // Arrange
        CorporationTaxReturn taxReturn = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/corporation_tax_returns/123"),
            PeriodStartsOn = new DateOnly(2023, 4, 1),
            PeriodEndsOn = new DateOnly(2024, 3, 31)
        };

        CorporationTaxReturnRoot responseRoot = new() { CorporationTaxReturn = taxReturn };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        CorporationTaxReturn result = await this.corporationTaxReturns.GetByIdAsync("123");

        // Assert
        result.ShouldNotBeNull();
        result.PeriodStartsOn.ShouldBe(new DateOnly(2023, 4, 1));

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/corporation_tax_returns/123");
    }

    [TestMethod]
    public async Task MarkAsFiledAsync_MarksReturnAsFiled()
    {
        // Arrange
        CorporationTaxReturn taxReturn = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/corporation_tax_returns/456"),
            PeriodStartsOn = new DateOnly(2023, 4, 1),
            PeriodEndsOn = new DateOnly(2024, 3, 31)
        };

        CorporationTaxReturnRoot responseRoot = new() { CorporationTaxReturn = taxReturn };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        CorporationTaxReturn result = await this.corporationTaxReturns.MarkAsFiledAsync("456", new DateOnly(2024, 4, 15));

        // Assert
        result.ShouldNotBeNull();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/corporation_tax_returns/456");
    }
}
