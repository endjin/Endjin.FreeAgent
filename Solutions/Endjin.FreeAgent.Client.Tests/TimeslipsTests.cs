// <copyright file="TimeslipsTests.cs" company="Endjin Limited">
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
public class TimeslipsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private Timeslips timeslips = null!;
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

        this.timeslips = new Timeslips(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetByProjectUrlAsync_ReturnsTimeslipsForProject()
    {
        // Arrange
        string projectUrl = "https://api.freeagent.com/v2/projects/456";

        ImmutableList<Timeslip> timeslipsList = ImmutableList.Create(
            new Timeslip
            {
                DatedOn = DateTimeOffset.Parse("2024-01-15"),
                Hours = 7.5m,
                Comment = "Worked on feature implementation",
                User = new Uri("https://api.freeagent.com/v2/users/123"),
                Project = new Uri(projectUrl),
                Task = new Uri("https://api.freeagent.com/v2/tasks/789"),
                Url = new Uri("https://api.freeagent.com/v2/timeslips/1001")
            },
            new Timeslip
            {
                DatedOn = DateTimeOffset.Parse("2024-01-16"),
                Hours = 6.0m,
                Comment = "Code review and testing",
                User = new Uri("https://api.freeagent.com/v2/users/123"),
                Project = new Uri(projectUrl),
                Task = new Uri("https://api.freeagent.com/v2/tasks/790"),
                Url = new Uri("https://api.freeagent.com/v2/timeslips/1002")
            }
        );

        TimeslipsRoot responseRoot = new() { Timeslips = timeslipsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Timeslip> result = await this.timeslips.GetByProjectUrlAsync(projectUrl);

        // Assert
        result.Count().ShouldBe(2);
        result.All(t => t.Project?.ToString() == projectUrl).ShouldBeTrue();
        result.Sum(t => t.Hours ?? 0).ShouldBe(13.5m);
    }

    [TestMethod]
    public async Task GetAllByUserIdAndDateRangeAsync_ReturnsFilteredTimeslips()
    {
        // Arrange
        string userId = "123";
        string fromDate = "2024-01-01";
        string toDate = "2024-01-31";
        string userUrl = $"https://api.freeagent.com/v2/users/{userId}";

        ImmutableList<Timeslip> timeslipsList = ImmutableList.Create(
            new Timeslip
            {
                DatedOn = DateTimeOffset.Parse("2024-01-10"),
                Hours = 8m,
                Comment = "Day 1 work",
                User = new Uri(userUrl),
                Url = new Uri("https://api.freeagent.com/v2/timeslips/2001")
            },
            new Timeslip
            {
                DatedOn = DateTimeOffset.Parse("2024-01-15"),
                Hours = 7.5m,
                Comment = "Day 2 work",
                User = new Uri(userUrl),
                Url = new Uri("https://api.freeagent.com/v2/timeslips/2002")
            }
        );

        TimeslipsRoot responseRoot = new() { Timeslips = timeslipsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Timeslip> result = await this.timeslips.GetAllByUserIdAndDateRangeAsync(userId, fromDate, toDate);

        // Assert
        result.Count().ShouldBe(2);
        result.All(t => t.User?.ToString() == userUrl).ShouldBeTrue();
        result.Sum(t => t.Hours ?? 0).ShouldBe(15.5m);
    }

    [TestMethod]
    public async Task GetByProjectUrlAsync_WithCaching_UsesCachedResults()
    {
        // Arrange
        string projectUrl = "https://api.freeagent.com/v2/projects/456";

        ImmutableList<Timeslip> timeslipsList = ImmutableList.Create(
            new Timeslip
            {
                DatedOn = DateTimeOffset.Parse("2024-01-15"),
                Hours = 8m,
                Project = new Uri(projectUrl)
            }
        );

        TimeslipsRoot responseRoot = new() { Timeslips = timeslipsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - First call
        IEnumerable<Timeslip> result1 = await this.timeslips.GetByProjectUrlAsync(projectUrl);

        // Act - Second call (should use cache)
        IEnumerable<Timeslip> result2 = await this.timeslips.GetByProjectUrlAsync(projectUrl);

        // Assert
        result1.Count().ShouldBe(1);
        result2.Count().ShouldBe(1);
        result2.ShouldBe(result1); // Should be same cached instance

        // Verify HTTP was called only once due to caching
        this.messageHandler.CallCount.ShouldBe(1);
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.cache?.Dispose();
        this.httpClient?.Dispose();
        this.messageHandler?.Dispose();
    }

}
