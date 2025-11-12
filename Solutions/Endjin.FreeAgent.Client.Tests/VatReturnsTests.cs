// <copyright file="VatReturnsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class VatReturnsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private VatReturns vatReturns = null!;
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
        this.vatReturns = new VatReturns(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllVatReturns()
    {
        // Arrange
        List<VatReturn> returnsList =
        [
            new()
            {
                PeriodStartsOn = new DateOnly(2024, 1, 1),
                PeriodEndsOn = new DateOnly(2024, 3, 31),
                Status = "draft",
                Box5NetVatDue = 17500.00m
            },
            new()
            {
                PeriodStartsOn = new DateOnly(2023, 10, 1),
                PeriodEndsOn = new DateOnly(2023, 12, 31),
                Status = "filed",
                Box5NetVatDue = 12500.00m
            }
        ];

        VatReturnsRoot responseRoot = new() { VatReturns = returnsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<VatReturn> result = await this.vatReturns.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);
        result.First().Box5NetVatDue.ShouldBe(17500.00m);
        result.Last().Box5NetVatDue.ShouldBe(12500.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/vat_returns");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsVatReturn()
    {
        // Arrange
        VatReturn vatReturn = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/vat_returns/101"),
            PeriodStartsOn = new DateOnly(2024, 1, 1),
            PeriodEndsOn = new DateOnly(2024, 3, 31),
            Status = "draft",
            Box1VatDueOnSales = 25000.00m,
            Box4VatReclaimed = 8000.00m,
            Box5NetVatDue = 17500.00m
        };

        VatReturnRoot responseRoot = new() { VatReturn = vatReturn };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        VatReturn result = await this.vatReturns.GetByIdAsync("101");

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe("draft");
        result.Box5NetVatDue.ShouldBe(17500.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/vat_returns/101");
    }

    [TestMethod]
    public async Task MarkAsFiledAsync_WithValidDetails_UpdatesVatReturnAsFiled()
    {
        // Arrange
        DateOnly filedOn = new(2024, 4, 15);
        string hmrcReference = "123456789012";

        VatReturn responseReturn = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/vat_returns/101"),
            Status = "filed",
            FiledOn = filedOn,
            FiledOnline = true,
            HmrcReference = hmrcReference
        };

        VatReturnRoot responseRoot = new() { VatReturn = responseReturn };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        VatReturn result = await this.vatReturns.MarkAsFiledAsync("101", filedOn, true, hmrcReference);

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe("filed");
        result.FiledOn.ShouldBe(filedOn);
        result.FiledOnline.ShouldBe(true);
        result.HmrcReference.ShouldBe(hmrcReference);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/vat_returns/101/mark_as_filed");
    }
}
