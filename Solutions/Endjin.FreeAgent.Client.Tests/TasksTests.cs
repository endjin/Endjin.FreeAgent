// <copyright file="TasksTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class TasksTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private Tasks tasks = null!;
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

        this.tasks = new Tasks(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task CreateAsync_WithValidTask_ReturnsCreatedTask()
    {
        // Arrange
        TaskItem inputTask = new()
        {
            Name = "Development",
            Project = "https://api.freeagent.com/v2/projects/123",
            IsBillable = true,
            BillingRate = 100.00m,
            BillingPeriod = "hour",
            Status = "Active"
        };

        TaskItem responseTask = new()
        {
            Name = "Development",
            Project = "https://api.freeagent.com/v2/projects/123",
            IsBillable = true,
            BillingRate = 100.00m,
            BillingPeriod = "hour",
            Status = "Active",
            Url = new Uri("https://api.freeagent.com/v2/tasks/12345")
        };

        TaskRoot responseRoot = new() { TaskItem = responseTask };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        TaskItem result = await this.tasks.CreateAsync(inputTask);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Development");
        result.IsBillable.ShouldBe(true);
        result.BillingRate.ShouldBe(100.00m);
        result.BillingPeriod.ShouldBe("hour");
        result.Url?.ToString().ShouldBe("https://api.freeagent.com/v2/tasks/12345");
    }

    [TestMethod]
    public async Task GetAllByProjectUrlAsync_ReturnsProjectTasks()
    {
        // Arrange
        Uri projectUrl = new("https://api.freeagent.com/v2/projects/123");

        List<TaskItem> tasksList =
        [
            new() { Name = "Backend Development", Project = projectUrl.ToString(), IsBillable = true },
            new() { Name = "Frontend Development", Project = projectUrl.ToString(), IsBillable = true },
            new() { Name = "QA Testing", Project = projectUrl.ToString(), IsBillable = true }
        ];

        TasksRoot responseRoot = new() { Tasks = tasksList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<TaskItem> result = await this.tasks.GetAllByProjectUrlAsync(projectUrl);

        // Assert
        result.Count().ShouldBe(3);
        result.All(t => t.Project == projectUrl.ToString()).ShouldBeTrue();
        result.Any(t => t.Name == "Backend Development").ShouldBeTrue();
        result.Any(t => t.Name == "Frontend Development").ShouldBeTrue();
        result.Any(t => t.Name == "QA Testing").ShouldBeTrue();
    }

    [TestMethod]
    public async Task CreateAsync_WithNonBillableTask_CreatesSuccessfully()
    {
        // Arrange
        TaskItem inputTask = new()
        {
            Name = "Internal Meeting",
            Project = "https://api.freeagent.com/v2/projects/123",
            IsBillable = false,
            Status = "Active"
        };

        TaskItem responseTask = new()
        {
            Name = "Internal Meeting",
            Project = "https://api.freeagent.com/v2/projects/123",
            IsBillable = false,
            Status = "Active",
            Url = new Uri("https://api.freeagent.com/v2/tasks/12346")
        };

        TaskRoot responseRoot = new() { TaskItem = responseTask };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        TaskItem result = await this.tasks.CreateAsync(inputTask);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Internal Meeting");
        result.IsBillable.ShouldBe(false);
        result.BillingRate.ShouldBeNull();
    }

    [TestMethod]
    public async Task CreateAsync_WithoutProject_ThrowsArgumentException()
    {
        // Arrange
        TaskItem inputTask = new()
        {
            Name = "Task without project",
            IsBillable = true
        };

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.tasks.CreateAsync(inputTask));
    }

    [TestMethod]
    public async Task GetAllByProjectUrlAsync_WithCaching_UsesCachedResults()
    {
        // Arrange
        Uri projectUrl = new("https://api.freeagent.com/v2/projects/123");

        List<TaskItem> tasksList =
        [
            new() { Name = "Cached Task", Project = projectUrl.ToString(), IsBillable = true }
        ];

        TasksRoot responseRoot = new() { Tasks = tasksList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - First call
        IEnumerable<TaskItem> result1 = await this.tasks.GetAllByProjectUrlAsync(projectUrl);

        // Act - Second call (should use cache)
        IEnumerable<TaskItem> result2 = await this.tasks.GetAllByProjectUrlAsync(projectUrl);

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
