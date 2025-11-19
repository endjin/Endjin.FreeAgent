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
    public async Task CreateAsync_ReturnsCreatedTimeslip()
    {
        // Arrange
        Timeslip newTimeslip = new()
        {
            DatedOn = DateTimeOffset.Parse("2024-01-15"),
            Hours = 7.5m,
            Comment = "Worked on feature implementation",
            User = new Uri("https://api.freeagent.com/v2/users/123"),
            Project = new Uri("https://api.freeagent.com/v2/projects/456"),
            Task = new Uri("https://api.freeagent.com/v2/tasks/789")
        };

        Timeslip createdTimeslip = newTimeslip with
        {
            Url = new Uri("https://api.freeagent.com/v2/timeslips/1001"),
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        TimeslipRoot responseRoot = new() { Timeslip = createdTimeslip };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Timeslip result = await this.timeslips.CreateAsync(newTimeslip);

        // Assert
        result.Url.ShouldNotBeNull();
        result.Hours.ShouldBe(7.5m);
        result.Comment.ShouldBe("Worked on feature implementation");
        this.messageHandler.LastRequest?.Method.ShouldBe(HttpMethod.Post);
        this.messageHandler.LastRequest?.RequestUri?.ToString().ShouldContain("v2/timeslips");
    }

    [TestMethod]
    public async Task CreateBatchAsync_ReturnsCreatedTimeslips()
    {
        // Arrange
        List<Timeslip> newTimeslips =
        [
            new()
            {
                DatedOn = DateTimeOffset.Parse("2024-01-15"),
                Hours = 4m,
                User = new Uri("https://api.freeagent.com/v2/users/123"),
                Project = new Uri("https://api.freeagent.com/v2/projects/456"),
                Task = new Uri("https://api.freeagent.com/v2/tasks/789")
            },
            new()
            {
                DatedOn = DateTimeOffset.Parse("2024-01-16"),
                Hours = 8m,
                User = new Uri("https://api.freeagent.com/v2/users/123"),
                Project = new Uri("https://api.freeagent.com/v2/projects/456"),
                Task = new Uri("https://api.freeagent.com/v2/tasks/790")
            }
        ];

        ImmutableList<Timeslip> createdTimeslips = ImmutableList.Create(
            newTimeslips[0] with { Url = new Uri("https://api.freeagent.com/v2/timeslips/1001") },
            newTimeslips[1] with { Url = new Uri("https://api.freeagent.com/v2/timeslips/1002") }
        );

        TimeslipsRoot responseRoot = new() { Timeslips = createdTimeslips };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Timeslip> result = await this.timeslips.CreateBatchAsync(newTimeslips);

        // Assert
        result.Count().ShouldBe(2);
        result.All(t => t.Url != null).ShouldBeTrue();
        this.messageHandler.LastRequest?.Method.ShouldBe(HttpMethod.Post);
    }

    [TestMethod]
    public async Task GetByIdAsync_ReturnsTimeslip()
    {
        // Arrange
        string timeslipId = "1001";
        Timeslip expectedTimeslip = new()
        {
            Url = new Uri($"https://api.freeagent.com/v2/timeslips/{timeslipId}"),
            DatedOn = DateTimeOffset.Parse("2024-01-15"),
            Hours = 7.5m,
            Comment = "Worked on feature implementation",
            User = new Uri("https://api.freeagent.com/v2/users/123"),
            Project = new Uri("https://api.freeagent.com/v2/projects/456"),
            Task = new Uri("https://api.freeagent.com/v2/tasks/789")
        };

        TimeslipRoot responseRoot = new() { Timeslip = expectedTimeslip };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Timeslip result = await this.timeslips.GetByIdAsync(timeslipId);

        // Assert
        result.Url?.ToString().ShouldContain(timeslipId);
        result.Hours.ShouldBe(7.5m);
        this.messageHandler.LastRequest?.RequestUri?.ToString().ShouldContain($"v2/timeslips/{timeslipId}");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithCaching_UsesCachedResults()
    {
        // Arrange
        string timeslipId = "1001";
        Timeslip expectedTimeslip = new()
        {
            Url = new Uri($"https://api.freeagent.com/v2/timeslips/{timeslipId}"),
            Hours = 8m
        };

        TimeslipRoot responseRoot = new() { Timeslip = expectedTimeslip };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - First call
        Timeslip result1 = await this.timeslips.GetByIdAsync(timeslipId);

        // Act - Second call (should use cache)
        Timeslip result2 = await this.timeslips.GetByIdAsync(timeslipId);

        // Assert
        result1.Hours.ShouldBe(8m);
        result2.Hours.ShouldBe(8m);
        result2.ShouldBe(result1);
        this.messageHandler.CallCount.ShouldBe(1);
    }

    [TestMethod]
    public async Task GetAllAsync_WithNoFilters_ReturnsAllTimeslips()
    {
        // Arrange
        ImmutableList<Timeslip> timeslipsList = ImmutableList.Create(
            new Timeslip { Url = new Uri("https://api.freeagent.com/v2/timeslips/1001"), Hours = 7.5m },
            new Timeslip { Url = new Uri("https://api.freeagent.com/v2/timeslips/1002"), Hours = 6.0m }
        );

        TimeslipsRoot responseRoot = new() { Timeslips = timeslipsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Timeslip> result = await this.timeslips.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);
    }

    [TestMethod]
    public async Task GetAllAsync_WithDateRange_UsesCorrectQueryParameters()
    {
        // Arrange
        ImmutableList<Timeslip> timeslipsList = ImmutableList.Create(
            new Timeslip { Url = new Uri("https://api.freeagent.com/v2/timeslips/1001"), Hours = 7.5m }
        );

        TimeslipsRoot responseRoot = new() { Timeslips = timeslipsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Timeslip> result = await this.timeslips.GetAllAsync(
            fromDate: "2024-01-01",
            toDate: "2024-01-31");

        // Assert
        result.Count().ShouldBe(1);
        this.messageHandler.LastRequest?.RequestUri?.ToString().ShouldContain("from_date=2024-01-01");
        this.messageHandler.LastRequest?.RequestUri?.ToString().ShouldContain("to_date=2024-01-31");
    }

    [TestMethod]
    public async Task GetAllAsync_WithViewFilter_UsesCorrectQueryParameter()
    {
        // Arrange
        ImmutableList<Timeslip> timeslipsList = ImmutableList.Create(
            new Timeslip { Url = new Uri("https://api.freeagent.com/v2/timeslips/1001"), Hours = 7.5m }
        );

        TimeslipsRoot responseRoot = new() { Timeslips = timeslipsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Timeslip> result = await this.timeslips.GetAllAsync(view: "unbilled");

        // Assert
        result.Count().ShouldBe(1);
        this.messageHandler.LastRequest?.RequestUri?.ToString().ShouldContain("view=unbilled");
    }

    [TestMethod]
    public async Task GetAllAsync_WithUserFilter_UsesCorrectQueryParameter()
    {
        // Arrange
        Uri userUri = new("https://api.freeagent.com/v2/users/123");
        ImmutableList<Timeslip> timeslipsList = ImmutableList.Create(
            new Timeslip { Url = new Uri("https://api.freeagent.com/v2/timeslips/1001"), Hours = 7.5m }
        );

        TimeslipsRoot responseRoot = new() { Timeslips = timeslipsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Timeslip> result = await this.timeslips.GetAllAsync(user: userUri);

        // Assert
        result.Count().ShouldBe(1);
        this.messageHandler.LastRequest?.RequestUri?.ToString().ShouldContain("user=");
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
    public async Task UpdateAsync_SendsPutRequest()
    {
        // Arrange
        Timeslip timeslipToUpdate = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/timeslips/1001"),
            Hours = 8m,
            Comment = "Updated comment"
        };

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK);

        // Act
        await this.timeslips.UpdateAsync(timeslipToUpdate);

        // Assert
        this.messageHandler.LastRequest?.Method.ShouldBe(HttpMethod.Put);
        this.messageHandler.LastRequest?.RequestUri?.ToString().ShouldContain("v2/timeslips/1001");
    }

    [TestMethod]
    public async Task UpdateAsync_WithNullUrl_ThrowsArgumentException()
    {
        // Arrange
        Timeslip timeslipToUpdate = new()
        {
            Url = null,
            Hours = 8m
        };

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(() => this.timeslips.UpdateAsync(timeslipToUpdate));
    }

    [TestMethod]
    public async Task DeleteAsync_SendsDeleteRequest()
    {
        // Arrange
        string timeslipId = "1001";

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK);

        // Act
        await this.timeslips.DeleteAsync(timeslipId);

        // Assert
        this.messageHandler.LastRequest?.Method.ShouldBe(HttpMethod.Delete);
        this.messageHandler.LastRequest?.RequestUri?.ToString().ShouldContain($"v2/timeslips/{timeslipId}");
    }

    [TestMethod]
    public async Task DeleteAsync_WithNullId_ThrowsArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(() => this.timeslips.DeleteAsync(null!));
    }

    [TestMethod]
    public async Task StartTimerAsync_ReturnsTimeslipWithTimer()
    {
        // Arrange
        string timeslipId = "1001";
        Timeslip timeslipWithTimer = new()
        {
            Url = new Uri($"https://api.freeagent.com/v2/timeslips/{timeslipId}"),
            Hours = 2m,
            Timer = new Domain.Timer
            {
                Running = true,
                StartFrom = DateTimeOffset.UtcNow.AddHours(-2)
            }
        };

        TimeslipRoot responseRoot = new() { Timeslip = timeslipWithTimer };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Timeslip result = await this.timeslips.StartTimerAsync(timeslipId);

        // Assert
        result.Timer.ShouldNotBeNull();
        result.Timer.Running.ShouldBeTrue();
        result.Timer.StartFrom.ShouldNotBeNull();
        this.messageHandler.LastRequest?.Method.ShouldBe(HttpMethod.Post);
        this.messageHandler.LastRequest?.RequestUri?.ToString().ShouldContain($"v2/timeslips/{timeslipId}/timer");
    }

    [TestMethod]
    public async Task StartTimerAsync_WithNullId_ThrowsArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(() => this.timeslips.StartTimerAsync(null!));
    }

    [TestMethod]
    public async Task StopTimerAsync_ReturnsTimeslipWithAccumulatedHours()
    {
        // Arrange
        string timeslipId = "1001";
        Timeslip timeslipWithoutTimer = new()
        {
            Url = new Uri($"https://api.freeagent.com/v2/timeslips/{timeslipId}"),
            Hours = 4.5m, // 2 hours previously + 2.5 hours from timer
            Timer = null
        };

        TimeslipRoot responseRoot = new() { Timeslip = timeslipWithoutTimer };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Timeslip result = await this.timeslips.StopTimerAsync(timeslipId);

        // Assert
        result.Timer.ShouldBeNull();
        result.Hours.ShouldBe(4.5m);
        this.messageHandler.LastRequest?.Method.ShouldBe(HttpMethod.Delete);
        this.messageHandler.LastRequest?.RequestUri?.ToString().ShouldContain($"v2/timeslips/{timeslipId}/timer");
    }

    [TestMethod]
    public async Task StopTimerAsync_WithNullId_ThrowsArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(() => this.timeslips.StopTimerAsync(null!));
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

    [TestMethod]
    public async Task GetAllAsync_WithNestedParameter_UsesCorrectQueryParameter()
    {
        // Arrange
        ImmutableList<Timeslip> timeslipsList = ImmutableList.Create(
            new Timeslip { Url = new Uri("https://api.freeagent.com/v2/timeslips/1001"), Hours = 7.5m }
        );

        TimeslipsRoot responseRoot = new() { Timeslips = timeslipsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Timeslip> result = await this.timeslips.GetAllAsync(nested: true);

        // Assert
        result.Count().ShouldBe(1);
        this.messageHandler.LastRequest?.RequestUri?.ToString().ShouldContain("nested=true");
    }

    [TestMethod]
    public async Task Timeslip_WithBilledOnInvoice_DeserializesCorrectly()
    {
        // Arrange
        string timeslipId = "1001";
        Timeslip expectedTimeslip = new()
        {
            Url = new Uri($"https://api.freeagent.com/v2/timeslips/{timeslipId}"),
            Hours = 7.5m,
            BilledOnInvoice = new Uri("https://api.freeagent.com/v2/invoices/5001")
        };

        TimeslipRoot responseRoot = new() { Timeslip = expectedTimeslip };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Timeslip result = await this.timeslips.GetByIdAsync(timeslipId);

        // Assert
        result.BilledOnInvoice.ShouldNotBeNull();
        result.BilledOnInvoice.ToString().ShouldContain("invoices/5001");
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.cache?.Dispose();
        this.httpClient?.Dispose();
        this.messageHandler?.Dispose();
    }
}
