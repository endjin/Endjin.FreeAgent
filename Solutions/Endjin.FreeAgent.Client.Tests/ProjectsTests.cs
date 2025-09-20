// <copyright file="ProjectsTests.cs" company="Endjin Limited">
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
public class ProjectsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private Projects projects = null!;
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

        this.projects = new Projects(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task CreateAsync_WithValidProject_ReturnsCreatedProject()
    {
        // Arrange
        Project inputProject = new()
        {
            Name = "New Website Development",
            Contact = new Uri("https://api.freeagent.com/v2/contacts/123"),
            Status = "Active",
            Currency = "GBP",
            BudgetUnits = "Hours",
            HoursPerDay = 8.0m,
            NormalBillingRate = 75.00m
        };

        Project responseProject = new()
        {
            Name = "New Website Development",
            Contact = new Uri("https://api.freeagent.com/v2/contacts/123"),
            Status = "Active",
            Currency = "GBP",
            BudgetUnits = "Hours",
            HoursPerDay = 8.0m,
            NormalBillingRate = 75.00m,
            Url = new Uri("https://api.freeagent.com/v2/projects/12345")
        };

        ProjectRoot responseRoot = new() { Project = responseProject };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Project result = await this.projects.CreateAsync(inputProject);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("New Website Development");
        result.Contact!.ToString().ShouldBe("https://api.freeagent.com/v2/contacts/123");
        result.Status.ShouldBe("Active");
        result.NormalBillingRate.ShouldBe(75.00m);
        result.Url!.ToString().ShouldBe("https://api.freeagent.com/v2/projects/12345");
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllProjects_AndCachesResult()
    {
        // Arrange
        ImmutableList<Project> projectsList = ImmutableList.Create(
            new Project { Name = "Project A", Status = "Active" },
            new Project { Name = "Project B", Status = "Completed" },
            new Project { Name = "Project C", Status = "Active" }
        );

        ProjectsRoot responseRoot = new() { Projects = projectsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - First call
        IEnumerable<Project> result1 = await this.projects.GetAllAsync();

        // Act - Second call (should use cache)
        IEnumerable<Project> result2 = await this.projects.GetAllAsync();

        // Assert
        result1.Count().ShouldBe(3);
        result1.ElementAt(0).Name.ShouldBe("Project A");
        result1.ElementAt(1).Name.ShouldBe("Project B");
        result1.ElementAt(2).Name.ShouldBe("Project C");

        result2.Count().ShouldBe(3);
        result2.ShouldBe(result1); // Should be same cached instance

        // Verify HTTP was called only once due to caching
        this.messageHandler.CallCount.ShouldBe(1);
    }

    [TestMethod]
    public async Task GetAllActiveAsync_ReturnsOnlyActiveProjects()
    {
        // Arrange
        ImmutableList<Project> projectsList = ImmutableList.Create(
            new Project { Name = "Active Project 1", Status = "Active" },
            new Project { Name = "Completed Project", Status = "Completed" },
            new Project { Name = "Active Project 2", Status = "Active" },
            new Project { Name = "Cancelled Project", Status = "Cancelled" }
        );

        ProjectsRoot responseRoot = new() { Projects = projectsList.Where(p => p.Status == "Active").ToImmutableList() };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Project> result = await this.projects.GetAllActiveAsync();

        // Assert
        result.Count().ShouldBe(2);
        result.All(p => p.Status == "Active").ShouldBeTrue();
        result.Any(p => p.Name == "Active Project 1").ShouldBeTrue();
        result.Any(p => p.Name == "Active Project 2").ShouldBeTrue();
    }

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsProject()
    {
        // Arrange
        string projectId = "12345";
        Project responseProject = new()
        {
            Name = "Specific Project",
            Status = "Active",
            Currency = "GBP",
            NormalBillingRate = 100.00m,
            Url = new Uri($"https://api.freeagent.com/v2/projects/{projectId}")
        };

        ProjectRoot responseRoot = new() { Project = responseProject };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        // Don't call InitializeAndAuthorizeAsync in tests
        Project? result = await this.projects.GetByIdAsync(projectId);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Specific Project");
        result.Status.ShouldBe("Active");
        result.NormalBillingRate.ShouldBe(100.00m);
    }

    [TestMethod]
    public async Task GetByIdAsync_WithInvalidId_ThrowsException()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NotFound);

        // Act & Assert
        // Don't call InitializeAndAuthorizeAsync in tests
        await Should.ThrowAsync<HttpRequestException>(async () =>
            await this.projects.GetByIdAsync("invalid-id"));
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.cache?.Dispose();
        this.httpClient?.Dispose();
        this.messageHandler?.Dispose();
    }

}
