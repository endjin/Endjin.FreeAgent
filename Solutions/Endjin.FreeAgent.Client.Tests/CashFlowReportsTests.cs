// <copyright file="CashFlowReportsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class CashFlowReportsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private CashFlowReports cashFlowReports = null!;
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
        this.cashFlowReports = new CashFlowReports(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetAsync_WithDateRange_ReturnsCashFlow()
    {
        // Arrange
        CashFlow cashFlow = new()
        {
            From = "2024-01-01",
            To = "2024-03-31",
            Balance = 12593.21m,
            Incoming = new CashFlowDirection
            {
                Total = 68869.76m,
                Months = new List<CashFlowMonthly>
                {
                    new() { Month = 1, Year = 2024, Total = 22956.59m },
                    new() { Month = 2, Year = 2024, Total = 22956.59m },
                    new() { Month = 3, Year = 2024, Total = 22956.58m }
                }
            },
            Outgoing = new CashFlowDirection
            {
                Total = 56276.55m,
                Months = new List<CashFlowMonthly>
                {
                    new() { Month = 1, Year = 2024, Total = 18758.85m },
                    new() { Month = 2, Year = 2024, Total = 18758.85m },
                    new() { Month = 3, Year = 2024, Total = 18758.85m }
                }
            }
        };

        CashFlowRoot responseRoot = new() { CashFlow = cashFlow };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        CashFlow result = await this.cashFlowReports.GetAsync(new DateOnly(2024, 1, 1), new DateOnly(2024, 3, 31));

        // Assert
        result.ShouldNotBeNull();
        result.From.ShouldBe("2024-01-01");
        result.To.ShouldBe("2024-03-31");
        result.Balance.ShouldBe(12593.21m);
        result.Incoming.ShouldNotBeNull();
        result.Incoming.Total.ShouldBe(68869.76m);
        result.Incoming.Months.ShouldNotBeNull();
        result.Incoming.Months.Count.ShouldBe(3);
        result.Outgoing.ShouldNotBeNull();
        result.Outgoing.Total.ShouldBe(56276.55m);
        result.Outgoing.Months.ShouldNotBeNull();
        result.Outgoing.Months.Count.ShouldBe(3);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }
}
