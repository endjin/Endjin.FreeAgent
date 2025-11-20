// <copyright file="ProfitAndLossReportsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class ProfitAndLossReportsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private ProfitAndLossReports profitAndLossReports = null!;
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
        this.profitAndLossReports = new ProfitAndLossReports(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetAsync_WithDateRange_ReturnsProfitAndLossSummary()
    {
        // Arrange
        ProfitAndLoss report = new()
        {
            From = new DateOnly(2024, 1, 1),
            To = new DateOnly(2024, 3, 31),
            Income = 50000.00m,
            Expenses = 15000.00m,
            OperatingProfit = 35000.00m,
            Less =
            [
                new ProfitAndLossDeduction { Title = "Corporation Tax", Total = 7000.00m }
            ],
            RetainedProfit = 28000.00m,
            RetainedProfitBroughtForward = 10000.00m,
            RetainedProfitCarriedForward = 38000.00m
        };

        ProfitAndLossRoot responseRoot = new() { ProfitAndLoss = report };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        ProfitAndLoss result = await this.profitAndLossReports.GetAsync(new DateOnly(2024, 1, 1), new DateOnly(2024, 3, 31));

        // Assert
        result.ShouldNotBeNull();
        result.From.ShouldBe(new DateOnly(2024, 1, 1));
        result.To.ShouldBe(new DateOnly(2024, 3, 31));
        result.Income.ShouldBe(50000.00m);
        result.Expenses.ShouldBe(15000.00m);
        result.OperatingProfit.ShouldBe(35000.00m);
        result.Less.ShouldNotBeNull();
        result.Less.Count.ShouldBe(1);
        result.Less[0].Title.ShouldBe("Corporation Tax");
        result.Less[0].Total.ShouldBe(7000.00m);
        result.RetainedProfit.ShouldBe(28000.00m);
        result.RetainedProfitBroughtForward.ShouldBe(10000.00m);
        result.RetainedProfitCarriedForward.ShouldBe(38000.00m);

        // Verify correct URL
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.LastRequest.ShouldNotBeNull();
        this.messageHandler.LastRequest!.RequestUri!.PathAndQuery.ShouldBe("/v2/accounting/profit_and_loss/summary?from_date=2024-01-01&to_date=2024-03-31");
    }

    [TestMethod]
    public async Task GetByAccountingPeriodAsync_ReturnsProfitAndLossSummary()
    {
        // Arrange
        ProfitAndLoss report = new()
        {
            From = new DateOnly(2023, 4, 1),
            To = new DateOnly(2024, 3, 31),
            Income = 120000.00m,
            Expenses = 45000.00m,
            OperatingProfit = 75000.00m,
            Less =
            [
                new ProfitAndLossDeduction { Title = "Corporation Tax", Total = 15000.00m },
                new ProfitAndLossDeduction { Title = "Dividends", Total = 20000.00m }
            ],
            RetainedProfit = 40000.00m,
            RetainedProfitBroughtForward = 25000.00m,
            RetainedProfitCarriedForward = 65000.00m
        };

        ProfitAndLossRoot responseRoot = new() { ProfitAndLoss = report };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        ProfitAndLoss result = await this.profitAndLossReports.GetByAccountingPeriodAsync("2023/24");

        // Assert
        result.ShouldNotBeNull();
        result.From.ShouldBe(new DateOnly(2023, 4, 1));
        result.To.ShouldBe(new DateOnly(2024, 3, 31));
        result.Income.ShouldBe(120000.00m);
        result.Expenses.ShouldBe(45000.00m);
        result.OperatingProfit.ShouldBe(75000.00m);
        result.Less.ShouldNotBeNull();
        result.Less.Count.ShouldBe(2);
        result.RetainedProfit.ShouldBe(40000.00m);
        result.RetainedProfitBroughtForward.ShouldBe(25000.00m);
        result.RetainedProfitCarriedForward.ShouldBe(65000.00m);

        // Verify correct URL
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.LastRequest.ShouldNotBeNull();
        this.messageHandler.LastRequest!.RequestUri!.PathAndQuery.ShouldBe("/v2/accounting/profit_and_loss/summary?accounting_period=2023%2F24");
    }

    [TestMethod]
    public async Task GetByAccountingPeriodAsync_WithNullPeriod_ThrowsArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.profitAndLossReports.GetByAccountingPeriodAsync(null!));
    }

    [TestMethod]
    public async Task GetByAccountingPeriodAsync_WithEmptyPeriod_ThrowsArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.profitAndLossReports.GetByAccountingPeriodAsync(string.Empty));
    }

    [TestMethod]
    public async Task GetCurrentYearToDateAsync_ReturnsProfitAndLossSummary()
    {
        // Arrange
        ProfitAndLoss report = new()
        {
            From = new DateOnly(2024, 4, 1),
            To = new DateOnly(2024, 11, 18),
            Income = 80000.00m,
            Expenses = 30000.00m,
            OperatingProfit = 50000.00m,
            Less = [],
            RetainedProfit = 50000.00m,
            RetainedProfitBroughtForward = 65000.00m,
            RetainedProfitCarriedForward = 115000.00m
        };

        ProfitAndLossRoot responseRoot = new() { ProfitAndLoss = report };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        ProfitAndLoss result = await this.profitAndLossReports.GetCurrentYearToDateAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Income.ShouldBe(80000.00m);
        result.Expenses.ShouldBe(30000.00m);
        result.OperatingProfit.ShouldBe(50000.00m);
        result.RetainedProfit.ShouldBe(50000.00m);
        result.RetainedProfitBroughtForward.ShouldBe(65000.00m);
        result.RetainedProfitCarriedForward.ShouldBe(115000.00m);

        // Verify correct URL (no query parameters)
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.LastRequest.ShouldNotBeNull();
        this.messageHandler.LastRequest!.RequestUri!.PathAndQuery.ShouldBe("/v2/accounting/profit_and_loss/summary");
    }

    [TestMethod]
    public async Task GetAsync_CachesResult()
    {
        // Arrange
        ProfitAndLoss report = new()
        {
            Income = 50000.00m,
            Expenses = 15000.00m,
            OperatingProfit = 35000.00m
        };

        ProfitAndLossRoot responseRoot = new() { ProfitAndLoss = report };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        DateOnly fromDate = new(2024, 1, 1);
        DateOnly toDate = new(2024, 3, 31);

        // Act - First call
        ProfitAndLoss result1 = await this.profitAndLossReports.GetAsync(fromDate, toDate);

        // Reset the handler to return a different response
        ProfitAndLoss report2 = new()
        {
            Income = 60000.00m,
            Expenses = 20000.00m,
            OperatingProfit = 40000.00m
        };
        ProfitAndLossRoot responseRoot2 = new() { ProfitAndLoss = report2 };
        string responseJson2 = JsonSerializer.Serialize(responseRoot2, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson2, Encoding.UTF8, "application/json")
        };

        // Act - Second call (should be cached)
        ProfitAndLoss result2 = await this.profitAndLossReports.GetAsync(fromDate, toDate);

        // Assert - Should return cached result (original values)
        result2.Income.ShouldBe(50000.00m);
        this.messageHandler.CallCount.ShouldBe(1); // Only one HTTP call should have been made
    }

    [TestMethod]
    public async Task GetAsync_WithNullResponse_ThrowsInvalidOperationException()
    {
        // Arrange
        ProfitAndLossRoot responseRoot = new() { ProfitAndLoss = null };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(async () =>
            await this.profitAndLossReports.GetAsync(new DateOnly(2024, 1, 1), new DateOnly(2024, 3, 31)));
    }
}
