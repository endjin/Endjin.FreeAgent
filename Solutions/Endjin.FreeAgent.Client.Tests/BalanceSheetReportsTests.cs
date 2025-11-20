// <copyright file="BalanceSheetReportsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class BalanceSheetReportsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private BalanceSheetReports balanceSheetReports = null!;
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
        this.balanceSheetReports = new BalanceSheetReports(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetAsync_WithDate_ReturnsBalanceSheet()
    {
        // Arrange
        BalanceSheet balanceSheet = new()
        {
            AccountingPeriodStartDate = new DateOnly(2024, 1, 1),
            AsAtDate = new DateOnly(2024, 3, 31),
            Currency = "GBP",
            CapitalAssets = new CapitalAssetsSection
            {
                Accounts =
                [
                    new BalanceSheetAccount
                    {
                        Name = "Computer Equipment",
                        NominalCode = "0030",
                        TotalDebitValue = 5000
                    }
                ],
                NetBookValue = 5000
            },
            CurrentAssets = new AssetsSection
            {
                Accounts =
                [
                    new BalanceSheetAccount
                    {
                        Name = "Current Account",
                        NominalCode = "1200",
                        TotalDebitValue = 45000
                    }
                ]
            },
            CurrentLiabilities = new LiabilitiesSection
            {
                Accounts =
                [
                    new BalanceSheetAccount
                    {
                        Name = "Accounts Payable",
                        NominalCode = "2100",
                        TotalDebitValue = -10000
                    }
                ]
            },
            NetCurrentAssets = 35000,
            TotalAssets = 40000,
            OwnersEquity = new OwnersEquitySection
            {
                Accounts =
                [
                    new BalanceSheetAccount
                    {
                        Name = "Share Capital",
                        NominalCode = "3000",
                        TotalDebitValue = 10000
                    }
                ],
                RetainedProfit = 30000
            },
            TotalOwnersEquity = 40000
        };

        BalanceSheetRoot responseRoot = new() { BalanceSheet = balanceSheet };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        BalanceSheet result = await this.balanceSheetReports.GetAsync(new DateOnly(2024, 3, 31));

        // Assert
        result.ShouldNotBeNull();
        result.AsAtDate.ShouldBe(new DateOnly(2024, 3, 31));
        result.AccountingPeriodStartDate.ShouldBe(new DateOnly(2024, 1, 1));
        result.Currency.ShouldBe("GBP");
        result.TotalAssets.ShouldBe(40000);
        result.TotalOwnersEquity.ShouldBe(40000);
        result.NetCurrentAssets.ShouldBe(35000);

        // Verify capital assets section
        result.CapitalAssets.ShouldNotBeNull();
        result.CapitalAssets.NetBookValue.ShouldBe(5000);
        result.CapitalAssets.Accounts.ShouldNotBeNull();
        result.CapitalAssets.Accounts.Count.ShouldBe(1);
        result.CapitalAssets.Accounts[0].Name.ShouldBe("Computer Equipment");
        result.CapitalAssets.Accounts[0].NominalCode.ShouldBe("0030");
        result.CapitalAssets.Accounts[0].TotalDebitValue.ShouldBe(5000);

        // Verify current assets section
        result.CurrentAssets.ShouldNotBeNull();
        result.CurrentAssets.Accounts.ShouldNotBeNull();
        result.CurrentAssets.Accounts.Count.ShouldBe(1);
        result.CurrentAssets.Accounts[0].Name.ShouldBe("Current Account");

        // Verify owners equity section
        result.OwnersEquity.ShouldNotBeNull();
        result.OwnersEquity.RetainedProfit.ShouldBe(30000);
        result.OwnersEquity.Accounts.ShouldNotBeNull();
        result.OwnersEquity.Accounts.Count.ShouldBe(1);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task GetOpeningBalancesAsync_ReturnsBalanceSheet()
    {
        // Arrange
        BalanceSheet balanceSheet = new()
        {
            // Opening balances don't have AccountingPeriodStartDate or AsAtDate
            Currency = "GBP",
            CapitalAssets = new CapitalAssetsSection
            {
                Accounts = [],
                NetBookValue = 0
            },
            CurrentAssets = new AssetsSection
            {
                Accounts = []
            },
            CurrentLiabilities = new LiabilitiesSection
            {
                Accounts = []
            },
            NetCurrentAssets = 0,
            TotalAssets = 0,
            OwnersEquity = new OwnersEquitySection
            {
                Accounts = [],
                RetainedProfit = 0
            },
            TotalOwnersEquity = 0
        };

        BalanceSheetRoot responseRoot = new() { BalanceSheet = balanceSheet };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        BalanceSheet result = await this.balanceSheetReports.GetOpeningBalancesAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Currency.ShouldBe("GBP");
        result.AccountingPeriodStartDate.ShouldBeNull(); // Opening balances don't have this
        result.AsAtDate.ShouldBeNull(); // Opening balances don't have this

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }
}
