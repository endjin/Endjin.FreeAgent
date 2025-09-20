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
                DebitValue = "1000.00",
                Category = "https://api.freeagent.com/v2/categories/250"
            },
            new JournalEntry
            {
                Description = "Bank payment",
                DebitValue = "-1000.00", // Credit is negative debit
                Category = "https://api.freeagent.com/v2/categories/901"
            }
        );

        JournalSet inputJournalSet = new()
        {
            DatedOn = "2024-01-15",
            Description = "Monthly rent payment",
            JournalEntries = entries
        };

        JournalSet responseJournalSet = new()
        {
            DatedOn = "2024-01-15",
            Description = "Monthly rent payment",
            JournalEntries = entries,
            Url = "https://api.freeagent.com/v2/journal_sets/12345"
        };

        JournalSetRoot responseRoot = new() { JournalSet = responseJournalSet };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        bool result = await this.journalSets.CreateAsync(inputJournalSet);

        // Assert
        result.ShouldBeTrue();
        // CreateAsync only returns bool, not the created entity
    }

    [TestMethod]
    public async Task CreateAsync_WithUnbalancedJournalSet_ThrowsHttpRequestException()
    {
        // Arrange
        ImmutableList<JournalEntry> entries = ImmutableList.Create(
            new JournalEntry
            {
                Description = "Office supplies",
                DebitValue = "500.00",
                Category = "https://api.freeagent.com/v2/categories/250"
            },
            new JournalEntry
            {
                Description = "Bank payment",
                DebitValue = "-300.00", // Unbalanced!
                Category = "https://api.freeagent.com/v2/categories/901"
            }
        );

        JournalSet inputJournalSet = new()
        {
            DatedOn = "2024-01-15",
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
                DebitValue = "2000.00",
                Category = "https://api.freeagent.com/v2/categories/001"
            },
            new JournalEntry
            {
                Description = "Installation fee",
                DebitValue = "500.00",
                Category = "https://api.freeagent.com/v2/categories/250"
            },
            new JournalEntry
            {
                Description = "Supplier invoice",
                DebitValue = "-2100.00", // Credit
                Category = "https://api.freeagent.com/v2/categories/300"
            },
            new JournalEntry
            {
                Description = "VAT",
                DebitValue = "-400.00", // Credit
                Category = "https://api.freeagent.com/v2/categories/301"
            }
        );

        JournalSet inputJournalSet = new()
        {
            DatedOn = "2024-01-15",
            Description = "Equipment purchase with VAT",
            JournalEntries = entries
        };

        JournalSet responseJournalSet = new()
        {
            DatedOn = "2024-01-15",
            Description = "Equipment purchase with VAT",
            JournalEntries = entries,
            Url = "https://api.freeagent.com/v2/journal_sets/12346"
        };

        JournalSetRoot responseRoot = new() { JournalSet = responseJournalSet };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        bool result = await this.journalSets.CreateAsync(inputJournalSet);

        // Assert
        result.ShouldBeTrue();
        // CreateAsync only returns bool, verification of balance can be done before submission
        decimal totalDebits = inputJournalSet.JournalEntries
            .Where(e => decimal.Parse(e.DebitValue) > 0)
            .Sum(e => decimal.Parse(e.DebitValue));
        decimal totalCredits = Math.Abs(inputJournalSet.JournalEntries
            .Where(e => decimal.Parse(e.DebitValue) < 0)
            .Sum(e => decimal.Parse(e.DebitValue)));

        totalDebits.ShouldBe(2500.00m);
        totalCredits.ShouldBe(2500.00m);
    }

    [TestMethod]
    public async Task CreateAsync_WithEmptyJournalEntries_ThrowsException()
    {
        // Arrange
        JournalSet inputJournalSet = new()
        {
            DatedOn = "2024-01-15",
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

    [TestCleanup]
    public void Cleanup()
    {
        this.cache?.Dispose();
        this.httpClient?.Dispose();
        this.messageHandler?.Dispose();
    }

}
