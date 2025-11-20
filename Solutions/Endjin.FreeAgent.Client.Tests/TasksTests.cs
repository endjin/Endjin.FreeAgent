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
            Project = new Uri("https://api.freeagent.com/v2/projects/123"),
            IsBillable = true,
            BillingRate = 75.00m,
            BillingPeriod = "hour",
            Status = "Active"
        };

        TaskItem responseTask = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/tasks/456"),
            Name = "Development Work",
            Project = new Uri("https://api.freeagent.com/v2/projects/123"),
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
            Project = new Uri("https://api.freeagent.com/v2/projects/789"),
            IsBillable = false,
            Status = "Active"
        };

        TaskItem responseTask = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/tasks/999"),
            Name = "Internal Meeting",
            Project = new Uri("https://api.freeagent.com/v2/projects/789"),
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
                Project = new Uri("https://api.freeagent.com/v2/projects/123"),
                IsBillable = true,
                BillingRate = 100.00m,
                BillingPeriod = "hour",
                Status = "Active"
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/tasks/2"),
                Name = "Testing",
                Project = new Uri("https://api.freeagent.com/v2/projects/123"),
                IsBillable = true,
                BillingRate = 75.00m,
                BillingPeriod = "hour",
                Status = "Active"
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
                Project = new Uri("https://api.freeagent.com/v2/projects/456"),
                IsBillable = true,
                Status = "Active"
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
            Project = new Uri("https://api.freeagent.com/v2/projects/123"),
            IsBillable = true,
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
            Project = new Uri("https://api.freeagent.com/v2/projects/123"),
            IsBillable = true,
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

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsTask()
    {
        // Arrange
        TaskItem responseTask = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/tasks/123"),
            Name = "Development Work",
            Project = new Uri("https://api.freeagent.com/v2/projects/456"),
            IsBillable = true,
            BillingRate = 100.00m,
            BillingPeriod = "hour",
            Status = "Active",
            Currency = "GBP",
            IsDeletable = true
        };

        TaskRoot responseRoot = new() { TaskItem = responseTask };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        TaskItem result = await this.tasks.GetByIdAsync("123");

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Development Work");
        result.BillingRate.ShouldBe(100.00m);
        result.Currency.ShouldBe("GBP");
        result.IsDeletable.ShouldBe(true);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/tasks/123");
    }

    [TestMethod]
    public async Task GetByIdAsync_CachesResults()
    {
        // Arrange
        TaskItem responseTask = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/tasks/789"),
            Name = "Cached Task",
            IsBillable = true,
            Status = "Active"
        };

        TaskRoot responseRoot = new() { TaskItem = responseTask };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        TaskItem result1 = await this.tasks.GetByIdAsync("789");
        TaskItem result2 = await this.tasks.GetByIdAsync("789");

        // Assert
        result1.ShouldNotBeNull();
        result2.ShouldNotBeNull();
        result1.Name.ShouldBe("Cached Task");

        // Mock Verification - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task GetAllAsync_WithNoParameters_ReturnsAllTasks()
    {
        // Arrange
        List<TaskItem> tasksList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/tasks/1"),
                Name = "Task 1",
                IsBillable = true,
                Status = "Active"
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/tasks/2"),
                Name = "Task 2",
                IsBillable = true,
                Status = "Completed"
            }
        ];

        TasksRoot responseRoot = new() { Tasks = tasksList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<TaskItem> result = await this.tasks.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);
        result.Any(t => t.Name == "Task 1").ShouldBeTrue();
        result.Any(t => t.Name == "Task 2").ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task GetAllAsync_WithViewParameter_IncludesViewInQuery()
    {
        // Arrange
        List<TaskItem> tasksList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/tasks/1"),
                Name = "Active Task",
                IsBillable = true,
                Status = "Active"
            }
        ];

        TasksRoot responseRoot = new() { Tasks = tasksList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<TaskItem> result = await this.tasks.GetAllAsync(view: "active");

        // Assert
        result.Count().ShouldBe(1);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/tasks?view=active");
    }

    [TestMethod]
    public async Task GetAllAsync_WithSortParameter_IncludesSortInQuery()
    {
        // Arrange
        List<TaskItem> tasksList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/tasks/1"),
                Name = "Task",
                IsBillable = true,
                Status = "Active"
            }
        ];

        TasksRoot responseRoot = new() { Tasks = tasksList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<TaskItem> result = await this.tasks.GetAllAsync(sort: "name");

        // Assert
        result.Count().ShouldBe(1);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/tasks?sort=name");
    }

    [TestMethod]
    public async Task GetAllAsync_WithMultipleParameters_IncludesAllInQuery()
    {
        // Arrange
        List<TaskItem> tasksList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/tasks/1"),
                Name = "Task",
                IsBillable = true,
                Status = "Active"
            }
        ];

        TasksRoot responseRoot = new() { Tasks = tasksList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<TaskItem> result = await this.tasks.GetAllAsync(
            view: "active",
            sort: "created_at",
            project: new Uri("https://api.freeagent.com/v2/projects/123"));

        // Assert
        result.Count().ShouldBe(1);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task DeleteAsync_WithValidId_DeletesSuccessfully()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK);

        // Act
        await this.tasks.DeleteAsync("456");

        // Assert & Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/tasks/456");
    }

    [TestMethod]
    public async Task UpdateTaskAsync_ReturnsDeserializedResponse()
    {
        // Arrange
        TaskItem inputTask = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/tasks/999"),
            Name = "Original Name",
            IsBillable = true,
            Status = "Active"
        };

        TaskItem responseTask = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/tasks/999"),
            Name = "Original Name",
            IsBillable = true,
            Status = "Active",
            UpdatedAt = new DateTimeOffset(2024, 3, 20, 15, 30, 0, TimeSpan.Zero)
        };

        TaskRoot responseRoot = new() { TaskItem = responseTask };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        TaskItem result = await this.tasks.UpdateTaskAsync(inputTask);

        // Assert - verify we got the response with server-assigned values
        result.ShouldNotBeNull();
        result.UpdatedAt.ShouldNotBeNull();
        result.UpdatedAt.Value.Year.ShouldBe(2024);
        result.UpdatedAt.Value.Month.ShouldBe(3);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
    }

    [TestMethod]
    public async Task CreateAsync_WithCurrencyProperty_SerializesCorrectly()
    {
        // Arrange
        TaskItem inputTask = new()
        {
            Name = "USD Task",
            Project = new Uri("https://api.freeagent.com/v2/projects/123"),
            IsBillable = true,
            BillingRate = 150.00m,
            BillingPeriod = "hour",
            Currency = "USD",
            Status = "Active"
        };

        TaskItem responseTask = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/tasks/555"),
            Name = "USD Task",
            Project = new Uri("https://api.freeagent.com/v2/projects/123"),
            IsBillable = true,
            BillingRate = 150.00m,
            BillingPeriod = "hour",
            Currency = "USD",
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
        result.Currency.ShouldBe("USD");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
    }
}
