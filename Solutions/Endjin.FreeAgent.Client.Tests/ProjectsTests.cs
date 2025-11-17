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

        await TestHelper.SetupForTestingAsync(this.freeAgentClient, this.httpClientFactory);
        this.projects = new Projects(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task CreateAsync_WithValidProject_ReturnsCreatedProject()
    {
        // Arrange
        Project inputProject = new()
        {
            Contact = new Uri("https://api.freeagent.com/v2/contacts/123"),
            Name = "Website Redesign",
            Status = "Active",
            Currency = "GBP",
            BudgetUnits = "Hours",
            Budget = 100.00m
        };

        Project responseProject = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/projects/456"),
            Contact = new Uri("https://api.freeagent.com/v2/contacts/123"),
            Name = "Website Redesign",
            Status = "Active",
            Currency = "GBP",
            BudgetUnits = "Hours",
            Budget = 100.00m,
            CreatedAt = new DateTimeOffset(2024, 3, 15, 10, 0, 0, TimeSpan.Zero)
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
        result.Url.ShouldNotBeNull();
        result.Name.ShouldBe("Website Redesign");
        result.Budget.ShouldBe(100.00m);
        result.BudgetUnits.ShouldBe("Hours");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/projects");
    }

    [TestMethod]
    public async Task CreateAsync_WithMonetaryBudget_CreatesSuccessfully()
    {
        // Arrange
        Project inputProject = new()
        {
            Contact = new Uri("https://api.freeagent.com/v2/contacts/789"),
            Name = "Software Development",
            Status = "Active",
            Currency = "USD",
            BudgetUnits = "Monetary",
            Budget = 50000.00m,
            NormalBillingRate = 150.00m,
            BillingPeriod = "hour"
        };

        Project responseProject = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/projects/999"),
            Contact = new Uri("https://api.freeagent.com/v2/contacts/789"),
            Name = "Software Development",
            Status = "Active",
            Currency = "USD",
            BudgetUnits = "Monetary",
            Budget = 50000.00m,
            NormalBillingRate = 150.00m,
            BillingPeriod = "hour"
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
        result.BudgetUnits.ShouldBe("Monetary");
        result.Budget.ShouldBe(50000.00m);
        result.NormalBillingRate.ShouldBe(150.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllProjects()
    {
        // Arrange
        ImmutableList<Project> projectsList = ImmutableList.Create(
            new Project
            {
                Url = new Uri("https://api.freeagent.com/v2/projects/1"),
                Name = "Project A",
                Status = "Active",
                Currency = "GBP"
            },
            new Project
            {
                Url = new Uri("https://api.freeagent.com/v2/projects/2"),
                Name = "Project B",
                Status = "Completed",
                Currency = "USD"
            }
        );

        ProjectsRoot responseRoot = new() { Projects = projectsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Project> result = await this.projects.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);
        result.Any(p => p.Name == "Project A").ShouldBeTrue();
        result.Any(p => p.Currency == "USD").ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/projects");
    }

    [TestMethod]
    public async Task GetAllAsync_CachesResults()
    {
        // Arrange
        ImmutableList<Project> projectsList = ImmutableList.Create(
            new Project
            {
                Url = new Uri("https://api.freeagent.com/v2/projects/20"),
                Name = "Cached Project",
                Status = "Active",
                Currency = "GBP"
            }
        );

        ProjectsRoot responseRoot = new() { Projects = projectsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        IEnumerable<Project> result1 = await this.projects.GetAllAsync();
        IEnumerable<Project> result2 = await this.projects.GetAllAsync();

        // Assert
        result1.Count().ShouldBe(1);
        result2.Count().ShouldBe(1);

        // Mock Verification - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsProject()
    {
        // Arrange
        Project project = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/projects/30"),
            Contact = new Uri("https://api.freeagent.com/v2/contacts/50"),
            Name = "Specific Project",
            Status = "Active",
            Currency = "GBP",
            BudgetUnits = "Monetary",
            Budget = 50000.00m,
            NormalBillingRate = 100.00m,
            BillingPeriod = "hour",
            CreatedAt = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        ProjectRoot responseRoot = new() { Project = project };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Project result = await this.projects.GetByIdAsync("30");

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Specific Project");
        result.Budget.ShouldBe(50000.00m);
        result.NormalBillingRate.ShouldBe(100.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/projects/30");
    }

    [TestMethod]
    public async Task GetByIdAsync_CachesResult()
    {
        // Arrange
        Project project = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/projects/40"),
            Name = "Cached Single Project",
            Status = "Active",
            Currency = "GBP"
        };

        ProjectRoot responseRoot = new() { Project = project };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        Project result1 = await this.projects.GetByIdAsync("40");
        Project result2 = await this.projects.GetByIdAsync("40");

        // Assert
        result1.ShouldNotBeNull();
        result2.ShouldNotBeNull();
        result1.Name.ShouldBe("Cached Single Project");

        // Mock Verification - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task GetByNameAsync_WithValidName_ReturnsProject()
    {
        // Arrange
        ImmutableList<Project> projectsList = ImmutableList.Create(
            new Project
            {
                Url = new Uri("https://api.freeagent.com/v2/projects/1"),
                Name = "Design Work",
                Status = "Active",
                Currency = "GBP"
            },
            new Project
            {
                Url = new Uri("https://api.freeagent.com/v2/projects/2"),
                Name = "Development Work",
                Status = "Active",
                Currency = "GBP"
            }
        );

        ProjectsRoot responseRoot = new() { Projects = projectsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Project result = await this.projects.GetByNameAsync("Development Work");

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Development Work");
        result.Url.ShouldBe(new Uri("https://api.freeagent.com/v2/projects/2"));

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task GetByNameAsync_CaseInsensitive_ReturnsProject()
    {
        // Arrange
        ImmutableList<Project> projectsList = ImmutableList.Create(
            new Project
            {
                Url = new Uri("https://api.freeagent.com/v2/projects/10"),
                Name = "Mobile App Development",
                Status = "Active",
                Currency = "GBP"
            }
        );

        ProjectsRoot responseRoot = new() { Projects = projectsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Search with different casing
        Project result = await this.projects.GetByNameAsync("mobile app development");

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Mobile App Development");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }
}
