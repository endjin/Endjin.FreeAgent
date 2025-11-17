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

        await TestHelper.SetupForTestingAsync(this.freeAgentClient, this.httpClientFactory);
        this.tasks = new Tasks(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task CreateAsync_WithValidTask_ReturnsCreatedTask()
    {
        // Arrange
        TaskItem inputTask = new()
        {
            Name = "Development Work",
            Project = "https://api.freeagent.com/v2/projects/123",
            IsBillable = true,
            BillingRate = 75.00m,
            BillingPeriod = "hour",
            Status = "Active"
        };

        TaskItem responseTask = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/tasks/456"),
            Name = "Development Work",
            Project = "https://api.freeagent.com/v2/projects/123",
            IsBillable = true,
            BillingRate = 75.00m,
            BillingPeriod = "hour",
            Status = "Active",
            CreatedAt = new DateTimeOffset(2024, 3, 15, 10, 0, 0, TimeSpan.Zero)
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
        result.Url.ShouldNotBeNull();
        result.Name.ShouldBe("Development Work");
        result.BillingRate.ShouldBe(75.00m);
        result.IsBillable.ShouldBe(true);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
    }

    [TestMethod]
    public async Task CreateAsync_WithNonBillableTask_CreatesSuccessfully()
    {
        // Arrange
        TaskItem inputTask = new()
        {
            Name = "Internal Meeting",
            Project = "https://api.freeagent.com/v2/projects/789",
            IsBillable = false,
            Status = "Active"
        };

        TaskItem responseTask = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/tasks/999"),
            Name = "Internal Meeting",
            Project = "https://api.freeagent.com/v2/projects/789",
            IsBillable = false,
            Status = "Active"
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
        result.IsBillable.ShouldBe(false);
        result.BillingRate.ShouldBeNull();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
    }

    [TestMethod]
    public async Task GetAllByProjectUrlAsync_WithValidProjectUrl_ReturnsTasks()
    {
        // Arrange
        Uri projectUrl = new("https://api.freeagent.com/v2/projects/123");
        List<TaskItem> tasksList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/tasks/1"),
                Name = "Development",
                Project = "https://api.freeagent.com/v2/projects/123",
                IsBillable = true,
                BillingRate = 100.00m,
                BillingPeriod = "hour"
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/tasks/2"),
                Name = "Testing",
                Project = "https://api.freeagent.com/v2/projects/123",
                IsBillable = true,
                BillingRate = 75.00m,
                BillingPeriod = "hour"
            }
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
        result.Count().ShouldBe(2);
        result.Any(t => t.Name == "Development").ShouldBeTrue();
        result.Any(t => t.BillingRate == 75.00m).ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task GetAllByProjectUrlAsync_CachesResults()
    {
        // Arrange
        Uri projectUrl = new("https://api.freeagent.com/v2/projects/456");
        List<TaskItem> tasksList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/tasks/10"),
                Name = "Cached Task",
                Project = "https://api.freeagent.com/v2/projects/456"
            }
        ];

        TasksRoot responseRoot = new() { Tasks = tasksList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        IEnumerable<TaskItem> result1 = await this.tasks.GetAllByProjectUrlAsync(projectUrl);
        IEnumerable<TaskItem> result2 = await this.tasks.GetAllByProjectUrlAsync(projectUrl);

        // Assert
        result1.Count().ShouldBe(1);
        result2.Count().ShouldBe(1);

        // Mock Verification - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task UpdateTaskAsync_WithValidTask_ReturnsUpdatedTask()
    {
        // Arrange
        TaskItem updatedTask = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/tasks/777"),
            Name = "Updated Task Name",
            Project = "https://api.freeagent.com/v2/projects/123",
            BillingRate = 125.00m,
            BillingPeriod = "hour",
            Status = "Active"
        };

        TaskRoot responseRoot = new() { TaskItem = updatedTask };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        TaskItem result = await this.tasks.UpdateTaskAsync(updatedTask);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Updated Task Name");
        result.BillingRate.ShouldBe(125.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/tasks/777");
    }

    [TestMethod]
    public async Task UpdateTaskAsync_MarkAsCompleted_UpdatesStatus()
    {
        // Arrange
        TaskItem completedTask = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/tasks/888"),
            Name = "Completed Task",
            Project = "https://api.freeagent.com/v2/projects/123",
            Status = "Completed"
        };

        TaskRoot responseRoot = new() { TaskItem = completedTask };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        TaskItem result = await this.tasks.UpdateTaskAsync(completedTask);

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe("Completed");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
    }
}
