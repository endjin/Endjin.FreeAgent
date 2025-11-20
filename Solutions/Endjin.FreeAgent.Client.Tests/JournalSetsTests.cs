// <copyright file="JournalSetsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Immutable;
using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class JournalSetsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private JournalSets journalSets = null!;
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

        // Use proper initialization with TestOAuth2Service
        await TestHelper.SetupForTestingAsync(this.freeAgentClient, this.httpClientFactory);

        this.journalSets = new JournalSets(this.freeAgentClient);
    }

    [TestMethod]
    public async Task CreateAsync_WithBalancedJournalSet_ReturnsCreatedJournalSet()
    {
        // Arrange
        ImmutableList<JournalEntry> entries = ImmutableList.Create(
            new JournalEntry
            {
                Description = "Office rent",
                DebitValue = 1000.00m,
                Category = new Uri("https://api.freeagent.com/v2/categories/250")
            },
            new JournalEntry
            {
                Description = "Bank payment",
                DebitValue = -1000.00m, // Credit is negative debit
                Category = new Uri("https://api.freeagent.com/v2/categories/901")
            }
        );

        JournalSet inputJournalSet = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Description = "Monthly rent payment",
            JournalEntries = entries
        };

        JournalSet responseJournalSet = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Description = "Monthly rent payment",
            JournalEntries = entries,
            Url = new Uri("https://api.freeagent.com/v2/journal_sets/12345")
        };

        JournalSetRoot responseRoot = new() { JournalSet = responseJournalSet };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        JournalSet result = await this.journalSets.CreateAsync(inputJournalSet);

        // Assert
        result.ShouldNotBeNull();
        result.Url.ShouldBe(new Uri("https://api.freeagent.com/v2/journal_sets/12345"));
        result.Description.ShouldBe("Monthly rent payment");
    }

    [TestMethod]
    public async Task CreateAsync_WithUnbalancedJournalSet_ThrowsHttpRequestException()
    {
        // Arrange
        ImmutableList<JournalEntry> entries = ImmutableList.Create(
            new JournalEntry
            {
                Description = "Office supplies",
                DebitValue = 500.00m,
                Category = new Uri("https://api.freeagent.com/v2/categories/250")
            },
            new JournalEntry
            {
                Description = "Bank payment",
                DebitValue = -300.00m, // Unbalanced!
                Category = new Uri("https://api.freeagent.com/v2/categories/901")
            }
        );

        JournalSet inputJournalSet = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Description = "Unbalanced entry",
            JournalEntries = entries
        };

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Journal entries must balance", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(async () =>
            await this.journalSets.CreateAsync(inputJournalSet));
    }

    [TestMethod]
    public async Task CreateAsync_WithMultipleDebitsCreditEntries_HandlesCorrectly()
    {
        // Arrange
        ImmutableList<JournalEntry> entries = ImmutableList.Create(
            new JournalEntry
            {
                Description = "Equipment purchase",
                DebitValue = 2000.00m,
                Category = new Uri("https://api.freeagent.com/v2/categories/001")
            },
            new JournalEntry
            {
                Description = "Installation fee",
                DebitValue = 500.00m,
                Category = new Uri("https://api.freeagent.com/v2/categories/250")
            },
            new JournalEntry
            {
                Description = "Supplier invoice",
                DebitValue = -2100.00m, // Credit
                Category = new Uri("https://api.freeagent.com/v2/categories/300")
            },
            new JournalEntry
            {
                Description = "VAT",
                DebitValue = -400.00m, // Credit
                Category = new Uri("https://api.freeagent.com/v2/categories/301")
            }
        );

        JournalSet inputJournalSet = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Description = "Equipment purchase with VAT",
            JournalEntries = entries
        };

        JournalSet responseJournalSet = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Description = "Equipment purchase with VAT",
            JournalEntries = entries,
            Url = new Uri("https://api.freeagent.com/v2/journal_sets/12346")
        };

        JournalSetRoot responseRoot = new() { JournalSet = responseJournalSet };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        JournalSet result = await this.journalSets.CreateAsync(inputJournalSet);

        // Assert
        result.ShouldNotBeNull();
        result.Url.ShouldBe(new Uri("https://api.freeagent.com/v2/journal_sets/12346"));

        // Verify balance calculation
        decimal totalDebits = inputJournalSet.JournalEntries
            .Where(e => e.DebitValue > 0)
            .Sum(e => e.DebitValue);
        decimal totalCredits = Math.Abs(inputJournalSet.JournalEntries
            .Where(e => e.DebitValue < 0)
            .Sum(e => e.DebitValue));

        totalDebits.ShouldBe(2500.00m);
        totalCredits.ShouldBe(2500.00m);
    }

    [TestMethod]
    public async Task CreateAsync_WithEmptyJournalEntries_ThrowsException()
    {
        // Arrange
        JournalSet inputJournalSet = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Description = "Empty journal set",
            JournalEntries = ImmutableList<JournalEntry>.Empty
        };

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Journal entries cannot be empty", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(async () =>
            await this.journalSets.CreateAsync(inputJournalSet));
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsJournalSets()
    {
        // Arrange
        JournalSet journalSet1 = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Description = "Test set 1",
            Url = new Uri("https://api.freeagent.com/v2/journal_sets/1")
        };

        JournalSet journalSet2 = new()
        {
            DatedOn = new DateOnly(2024, 1, 16),
            Description = "Test set 2",
            Url = new Uri("https://api.freeagent.com/v2/journal_sets/2")
        };

        JournalSetsRoot responseRoot = new() { JournalSets = [journalSet1, journalSet2] };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<JournalSet> result = await this.journalSets.GetAllAsync();

        // Assert
        List<JournalSet> resultList = result.ToList();
        resultList.Count.ShouldBe(2);
        resultList[0].Description.ShouldBe("Test set 1");
        resultList[1].Description.ShouldBe("Test set 2");
    }

    [TestMethod]
    public async Task GetAllAsync_WithDateFilters_FormatsQueryParametersCorrectly()
    {
        // Arrange
        DateOnly fromDate = new(2024, 1, 1);
        DateOnly toDate = new(2024, 12, 31);

        JournalSetsRoot responseRoot = new() { JournalSets = [] };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        await this.journalSets.GetAllAsync(fromDate, toDate);

        // Assert - verify the URL contains the correctly formatted date parameters
        string? requestUri = this.messageHandler.LastRequest?.RequestUri?.ToString();
        requestUri.ShouldNotBeNull();
        requestUri.ShouldContain("from_date=2024-01-01");
        requestUri.ShouldContain("to_date=2024-12-31");
    }

    [TestMethod]
    public async Task GetAllAsync_WithUpdatedSince_FormatsAsISO8601()
    {
        // Arrange
        DateTimeOffset updatedSince = new(2024, 5, 24, 9, 0, 0, TimeSpan.Zero);

        JournalSetsRoot responseRoot = new() { JournalSets = [] };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        await this.journalSets.GetAllAsync(updatedSince: updatedSince);

        // Assert - verify the URL contains ISO 8601 formatted timestamp
        string? requestUri = this.messageHandler.LastRequest?.RequestUri?.ToString();
        requestUri.ShouldNotBeNull();
        requestUri.ShouldContain("updated_since=2024-05-24T09%3A00%3A00.000Z");
    }

    [TestMethod]
    public async Task GetAllAsync_WithTag_EscapesTagParameter()
    {
        // Arrange
        string tag = "my-app/special tag";

        JournalSetsRoot responseRoot = new() { JournalSets = [] };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        await this.journalSets.GetAllAsync(tag: tag);

        // Assert - verify the URL contains properly URL-encoded tag
        string? requestUri = this.messageHandler.LastRequest?.RequestUri?.ToString();
        requestUri.ShouldNotBeNull();
        // The slash is encoded as %2F but spaces remain as spaces in the URI
        requestUri.ShouldContain("tag=my-app%2Fspecial tag");
    }

    [TestMethod]
    public async Task GetAllAsync_WithAllParameters_CombinesCorrectly()
    {
        // Arrange
        DateOnly fromDate = new(2024, 1, 1);
        DateOnly toDate = new(2024, 3, 31);
        DateTimeOffset updatedSince = new(2024, 1, 15, 14, 30, 0, TimeSpan.Zero);
        string tag = "test-tag";

        JournalSetsRoot responseRoot = new() { JournalSets = [] };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        await this.journalSets.GetAllAsync(fromDate, toDate, tag, updatedSince);

        // Assert - verify all parameters are present and correctly formatted
        string? requestUri = this.messageHandler.LastRequest?.RequestUri?.ToString();
        requestUri.ShouldNotBeNull();
        requestUri.ShouldContain("from_date=2024-01-01");
        requestUri.ShouldContain("to_date=2024-03-31");
        requestUri.ShouldContain("updated_since=2024-01-15T14%3A30%3A00.000Z");
        requestUri.ShouldContain("tag=test-tag");
    }

    [TestMethod]
    public async Task GetByIdAsync_ReturnsJournalSet()
    {
        // Arrange
        JournalSet journalSet = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Description = "Test set",
            Url = new Uri("https://api.freeagent.com/v2/journal_sets/12345")
        };

        JournalSetRoot responseRoot = new() { JournalSet = journalSet };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        JournalSet result = await this.journalSets.GetByIdAsync("12345");

        // Assert
        result.ShouldNotBeNull();
        result.Description.ShouldBe("Test set");
        result.Url.ShouldBe(new Uri("https://api.freeagent.com/v2/journal_sets/12345"));
    }

    [TestMethod]
    public async Task UpdateAsync_ReturnsUpdatedJournalSet()
    {
        // Arrange
        ImmutableList<JournalEntry> entries = ImmutableList.Create(
            new JournalEntry
            {
                Description = "Updated entry",
                DebitValue = 500.00m,
                Category = new Uri("https://api.freeagent.com/v2/categories/250")
            },
            new JournalEntry
            {
                DebitValue = -500.00m,
                Category = new Uri("https://api.freeagent.com/v2/categories/901")
            }
        );

        JournalSet inputJournalSet = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Description = "Updated description",
            JournalEntries = entries
        };

        JournalSet responseJournalSet = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Description = "Updated description",
            JournalEntries = entries,
            Url = new Uri("https://api.freeagent.com/v2/journal_sets/12345")
        };

        JournalSetRoot responseRoot = new() { JournalSet = responseJournalSet };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        JournalSet result = await this.journalSets.UpdateAsync("12345", inputJournalSet);

        // Assert
        result.ShouldNotBeNull();
        result.Description.ShouldBe("Updated description");
    }

    [TestMethod]
    public async Task DeleteAsync_SuccessfullyDeletes()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK);

        // Act & Assert - should not throw
        await this.journalSets.DeleteAsync("12345");
    }

    [TestMethod]
    public async Task GetOpeningBalancesAsync_ReturnsOpeningBalances()
    {
        // Arrange
        JournalSet openingBalances = new()
        {
            Description = "Opening Balances",
            Url = new Uri("https://api.freeagent.com/v2/journal_sets/opening_balances"),
            BankAccounts = [new BankAccountOpeningBalance { Url = new Uri("https://api.freeagent.com/v2/bank_accounts/1") }],
            StockItems = [new StockItemOpeningBalance { Url = new Uri("https://api.freeagent.com/v2/stock_items/1") }]
        };

        JournalSetRoot responseRoot = new() { JournalSet = openingBalances };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        JournalSet result = await this.journalSets.GetOpeningBalancesAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Description.ShouldBe("Opening Balances");
        result.BankAccounts.ShouldNotBeNull();
        result.BankAccounts.Count.ShouldBe(1);
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.cache?.Dispose();
        this.httpClient?.Dispose();
        this.messageHandler?.Dispose();
    }
}
