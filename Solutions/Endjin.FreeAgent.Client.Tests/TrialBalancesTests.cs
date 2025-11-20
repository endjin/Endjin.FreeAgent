// <copyright file="TrialBalancesTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class TrialBalancesTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private TrialBalances trialBalances = null!;
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
        this.trialBalances = new TrialBalances(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetSummaryAsync_ReturnsTrialBalanceSummary()
    {
        // Arrange
        List<TrialBalanceSummaryEntry> entries =
        [
            new TrialBalanceSummaryEntry
            {
                Category = new Uri("https://api.freeagent.com/v2/categories/001"),
                NominalCode = "001",
                DisplayNominalCode = "001",
                Name = "Sales",
                Total = 10000.00m
            },
            new TrialBalanceSummaryEntry
            {
                Category = new Uri("https://api.freeagent.com/v2/categories/100"),
                NominalCode = "100",
                DisplayNominalCode = "100",
                Name = "Cost of Sales",
                Total = 5000.00m
            }
        ];

        TrialBalanceSummaryRoot responseRoot = new() { TrialBalanceSummary = entries };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<TrialBalanceSummaryEntry> result = await this.trialBalances.GetSummaryAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count().ShouldBe(2);
        result.First().Name.ShouldBe("Sales");
        result.First().Total.ShouldBe(10000.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.LastRequest!.RequestUri!.AbsolutePath.ShouldEndWith("/v2/accounting/trial_balance/summary");
    }

    [TestMethod]
    public async Task GetSummaryAsync_WithToDateOnly_ReturnsTrialBalanceSummaryForPeriod()
    {
        // Arrange
        TrialBalanceSummaryRoot responseRoot = new() { TrialBalanceSummary = [] };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<TrialBalanceSummaryEntry> result = await this.trialBalances.GetSummaryAsync(
            toDate: new DateOnly(2024, 3, 31));

        // Assert
        result.ShouldNotBeNull();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.LastRequest!.RequestUri!.Query.ShouldContain("to_date=2024-03-31");
        this.messageHandler.LastRequest!.RequestUri!.Query.ShouldNotContain("from_date");
    }

    [TestMethod]
    public async Task GetSummaryAsync_WithDateRange_ReturnsTrialBalanceSummaryForRange()
    {
        // Arrange
        TrialBalanceSummaryRoot responseRoot = new() { TrialBalanceSummary = [] };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<TrialBalanceSummaryEntry> result = await this.trialBalances.GetSummaryAsync(
            fromDate: new DateOnly(2024, 1, 1),
            toDate: new DateOnly(2024, 3, 31));

        // Assert
        result.ShouldNotBeNull();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.LastRequest!.RequestUri!.Query.ShouldContain("from_date=2024-01-01");
        this.messageHandler.LastRequest!.RequestUri!.Query.ShouldContain("to_date=2024-03-31");
    }

    [TestMethod]
    public async Task GetSummaryAsync_WithBankAccountCategory_ReturnsBankAccountUrl()
    {
        // Arrange
        List<TrialBalanceSummaryEntry> entries =
        [
            new TrialBalanceSummaryEntry
            {
                Category = new Uri("https://api.freeagent.com/v2/categories/750-12345"),
                NominalCode = "750-12345",
                DisplayNominalCode = "750-12345",
                Name = "Business Bank Account",
                Total = 15000.00m,
                BankAccount = new Uri("https://api.freeagent.com/v2/bank_accounts/12345")
            }
        ];

        TrialBalanceSummaryRoot responseRoot = new() { TrialBalanceSummary = entries };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<TrialBalanceSummaryEntry> result = await this.trialBalances.GetSummaryAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count().ShouldBe(1);
        result.First().BankAccount.ShouldNotBeNull();
        result.First().BankAccount!.ToString().ShouldContain("bank_accounts/12345");
    }

    [TestMethod]
    public async Task GetSummaryAsync_WithUserCategory_ReturnsUserUrl()
    {
        // Arrange
        List<TrialBalanceSummaryEntry> entries =
        [
            new TrialBalanceSummaryEntry
            {
                Category = new Uri("https://api.freeagent.com/v2/categories/900-67890"),
                NominalCode = "900-67890",
                DisplayNominalCode = "900-67890",
                Name = "Director's Loan Account",
                Total = 2500.00m,
                User = new Uri("https://api.freeagent.com/v2/users/67890")
            }
        ];

        TrialBalanceSummaryRoot responseRoot = new() { TrialBalanceSummary = entries };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<TrialBalanceSummaryEntry> result = await this.trialBalances.GetSummaryAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count().ShouldBe(1);
        result.First().User.ShouldNotBeNull();
        result.First().User!.ToString().ShouldContain("users/67890");
    }

    [TestMethod]
    public async Task GetOpeningBalancesAsync_ReturnsOpeningBalances()
    {
        // Arrange
        List<TrialBalanceSummaryEntry> entries =
        [
            new TrialBalanceSummaryEntry
            {
                Category = new Uri("https://api.freeagent.com/v2/categories/001"),
                NominalCode = "001",
                DisplayNominalCode = "001",
                Name = "Sales",
                Total = 0.00m
            }
        ];

        TrialBalanceSummaryRoot responseRoot = new() { TrialBalanceSummary = entries };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<TrialBalanceSummaryEntry> result = await this.trialBalances.GetOpeningBalancesAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count().ShouldBe(1);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.LastRequest!.RequestUri!.AbsolutePath.ShouldEndWith("/v2/accounting/trial_balance/summary/opening_balances");
    }

    [TestMethod]
    public async Task GetSummaryAsync_CachesResult()
    {
        // Arrange
        TrialBalanceSummaryRoot responseRoot = new() { TrialBalanceSummary = [] };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        await this.trialBalances.GetSummaryAsync();
        await this.trialBalances.GetSummaryAsync();

        // Assert - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task GetOpeningBalancesAsync_CachesResult()
    {
        // Arrange
        TrialBalanceSummaryRoot responseRoot = new() { TrialBalanceSummary = [] };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        await this.trialBalances.GetOpeningBalancesAsync();
        await this.trialBalances.GetOpeningBalancesAsync();

        // Assert - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task TrialBalanceSummaryEntry_Serialization_RoundTrips()
    {
        // Arrange
        TrialBalanceSummaryEntry entry = new()
        {
            Category = new Uri("https://api.freeagent.com/v2/categories/001"),
            NominalCode = "001",
            DisplayNominalCode = "001-A",
            Name = "Sales",
            Total = 10000.50m,
            BankAccount = new Uri("https://api.freeagent.com/v2/bank_accounts/12345"),
            User = new Uri("https://api.freeagent.com/v2/users/67890")
        };

        // Act
        string json = JsonSerializer.Serialize(entry, SharedJsonOptions.Instance);
        TrialBalanceSummaryEntry? deserialized = JsonSerializer.Deserialize<TrialBalanceSummaryEntry>(json, SharedJsonOptions.Instance);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.Category.ShouldBe(entry.Category);
        deserialized.NominalCode.ShouldBe(entry.NominalCode);
        deserialized.DisplayNominalCode.ShouldBe(entry.DisplayNominalCode);
        deserialized.Name.ShouldBe(entry.Name);
        deserialized.Total.ShouldBe(entry.Total);
        deserialized.BankAccount.ShouldBe(entry.BankAccount);
        deserialized.User.ShouldBe(entry.User);
    }

    [TestMethod]
    public async Task TrialBalanceSummaryEntry_Serialization_UsesCorrectJsonPropertyNames()
    {
        // Arrange
        TrialBalanceSummaryEntry entry = new()
        {
            Category = new Uri("https://api.freeagent.com/v2/categories/001"),
            NominalCode = "001",
            DisplayNominalCode = "001-A",
            Name = "Sales",
            Total = 10000.50m,
            BankAccount = new Uri("https://api.freeagent.com/v2/bank_accounts/12345"),
            User = new Uri("https://api.freeagent.com/v2/users/67890")
        };

        // Act
        string json = JsonSerializer.Serialize(entry, SharedJsonOptions.Instance);

        // Assert
        json.ShouldContain("\"category\":");
        json.ShouldContain("\"nominal_code\":");
        json.ShouldContain("\"display_nominal_code\":");
        json.ShouldContain("\"name\":");
        json.ShouldContain("\"total\":");
        json.ShouldContain("\"bank_account\":");
        json.ShouldContain("\"user\":");
    }

    [TestMethod]
    public async Task TrialBalanceSummaryRoot_Serialization_UsesCorrectJsonPropertyName()
    {
        // Arrange
        TrialBalanceSummaryRoot root = new()
        {
            TrialBalanceSummary =
            [
                new TrialBalanceSummaryEntry
                {
                    Category = new Uri("https://api.freeagent.com/v2/categories/001"),
                    NominalCode = "001",
                    DisplayNominalCode = "001",
                    Name = "Test",
                    Total = 0m
                }
            ]
        };

        // Act
        string json = JsonSerializer.Serialize(root, SharedJsonOptions.Instance);

        // Assert
        json.ShouldContain("\"trial_balance_summary\":");
    }
}
