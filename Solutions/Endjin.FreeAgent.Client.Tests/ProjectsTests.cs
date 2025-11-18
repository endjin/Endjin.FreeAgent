// <copyright file="ProjectsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Immutable;
using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;
using NodaTime;

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
            Budget = 100.00m,
            UsesProjectInvoiceSequence = false
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
            UsesProjectInvoiceSequence = false,
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
            UsesProjectInvoiceSequence = false,
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
            UsesProjectInvoiceSequence = false,
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
                Contact = new Uri("https://api.freeagent.com/v2/contacts/1"),
                Name = "Project A",
                Status = "Active",
                Currency = "GBP",
                BudgetUnits = "Hours",
                Budget = 100.00m,
                UsesProjectInvoiceSequence = false
            },
            new Project
            {
                Url = new Uri("https://api.freeagent.com/v2/projects/2"),
                Contact = new Uri("https://api.freeagent.com/v2/contacts/2"),
                Name = "Project B",
                Status = "Completed",
                Currency = "USD",
                BudgetUnits = "Hours",
                Budget = 200.00m,
                UsesProjectInvoiceSequence = false
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
                Contact = new Uri("https://api.freeagent.com/v2/contacts/20"),
                Name = "Cached Project",
                Status = "Active",
                Currency = "GBP",
                BudgetUnits = "Hours",
                Budget = 100.00m,
                UsesProjectInvoiceSequence = false
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
            UsesProjectInvoiceSequence = false,
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
            Contact = new Uri("https://api.freeagent.com/v2/contacts/40"),
            Name = "Cached Single Project",
            Status = "Active",
            Currency = "GBP",
            BudgetUnits = "Hours",
            Budget = 100.00m,
            UsesProjectInvoiceSequence = false
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
                Contact = new Uri("https://api.freeagent.com/v2/contacts/1"),
                Name = "Design Work",
                Status = "Active",
                Currency = "GBP",
                BudgetUnits = "Hours",
                Budget = 100.00m,
                UsesProjectInvoiceSequence = false
            },
            new Project
            {
                Url = new Uri("https://api.freeagent.com/v2/projects/2"),
                Contact = new Uri("https://api.freeagent.com/v2/contacts/2"),
                Name = "Development Work",
                Status = "Active",
                Currency = "GBP",
                BudgetUnits = "Hours",
                Budget = 200.00m,
                UsesProjectInvoiceSequence = false
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
                Contact = new Uri("https://api.freeagent.com/v2/contacts/10"),
                Name = "Mobile App Development",
                Status = "Active",
                Currency = "GBP",
                BudgetUnits = "Hours",
                Budget = 100.00m,
                UsesProjectInvoiceSequence = false
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

    [TestMethod]
    public async Task UpdateAsync_WithValidProject_ReturnsUpdatedProject()
    {
        // Arrange
        Project inputProject = new()
        {
            Contact = new Uri("https://api.freeagent.com/v2/contacts/123"),
            Name = "Updated Project Name",
            Status = "Completed",
            Currency = "GBP",
            BudgetUnits = "Hours",
            Budget = 200.00m,
            UsesProjectInvoiceSequence = false
        };

        Project responseProject = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/projects/456"),
            Contact = new Uri("https://api.freeagent.com/v2/contacts/123"),
            Name = "Updated Project Name",
            Status = "Completed",
            Currency = "GBP",
            BudgetUnits = "Hours",
            Budget = 200.00m,
            UsesProjectInvoiceSequence = false,
            UpdatedAt = new DateTimeOffset(2024, 3, 20, 10, 0, 0, TimeSpan.Zero)
        };

        ProjectRoot responseRoot = new() { Project = responseProject };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Project result = await this.projects.UpdateAsync("456", inputProject);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Updated Project Name");
        result.Status.ShouldBe("Completed");
        result.Budget.ShouldBe(200.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/projects/456");
    }

    [TestMethod]
    public async Task DeleteAsync_WithValidId_Succeeds()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK);

        // Act & Assert (should not throw)
        await this.projects.DeleteAsync("123");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/projects/123");
    }

    [TestMethod]
    public async Task GetByContactAsync_ReturnsProjectsForContact()
    {
        // Arrange
        Uri contactUri = new("https://api.freeagent.com/v2/contacts/100");
        ImmutableList<Project> projectsList = ImmutableList.Create(
            new Project
            {
                Url = new Uri("https://api.freeagent.com/v2/projects/1"),
                Contact = contactUri,
                Name = "Project for Contact",
                Status = "Active",
                Currency = "GBP",
                BudgetUnits = "Hours",
                Budget = 100.00m,
                UsesProjectInvoiceSequence = false
            }
        );

        ProjectsRoot responseRoot = new() { Projects = projectsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Project> result = await this.projects.GetByContactAsync(contactUri);

        // Assert
        result.Count().ShouldBe(1);
        result.First().Name.ShouldBe("Project for Contact");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task GetAllCompletedAsync_ReturnsCompletedProjects()
    {
        // Arrange
        ImmutableList<Project> projectsList = ImmutableList.Create(
            new Project
            {
                Url = new Uri("https://api.freeagent.com/v2/projects/1"),
                Contact = new Uri("https://api.freeagent.com/v2/contacts/1"),
                Name = "Completed Project",
                Status = "Completed",
                Currency = "GBP",
                BudgetUnits = "Hours",
                Budget = 100.00m,
                UsesProjectInvoiceSequence = false
            }
        );

        ProjectsRoot responseRoot = new() { Projects = projectsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Project> result = await this.projects.GetAllCompletedAsync();

        // Assert
        result.Count().ShouldBe(1);
        result.First().Status.ShouldBe("Completed");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/projects?view=completed");
    }

    [TestMethod]
    public async Task GetAllCancelledAsync_ReturnsCancelledProjects()
    {
        // Arrange
        ImmutableList<Project> projectsList = ImmutableList.Create(
            new Project
            {
                Url = new Uri("https://api.freeagent.com/v2/projects/1"),
                Contact = new Uri("https://api.freeagent.com/v2/contacts/1"),
                Name = "Cancelled Project",
                Status = "Cancelled",
                Currency = "GBP",
                BudgetUnits = "Hours",
                Budget = 100.00m,
                UsesProjectInvoiceSequence = false
            }
        );

        ProjectsRoot responseRoot = new() { Projects = projectsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Project> result = await this.projects.GetAllCancelledAsync();

        // Assert
        result.Count().ShouldBe(1);
        result.First().Status.ShouldBe("Cancelled");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/projects?view=cancelled");
    }

    [TestMethod]
    public async Task GetAllHiddenAsync_ReturnsHiddenProjects()
    {
        // Arrange
        ImmutableList<Project> projectsList = ImmutableList.Create(
            new Project
            {
                Url = new Uri("https://api.freeagent.com/v2/projects/1"),
                Contact = new Uri("https://api.freeagent.com/v2/contacts/1"),
                Name = "Hidden Project",
                Status = "Hidden",
                Currency = "GBP",
                BudgetUnits = "Hours",
                Budget = 100.00m,
                UsesProjectInvoiceSequence = false
            }
        );

        ProjectsRoot responseRoot = new() { Projects = projectsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Project> result = await this.projects.GetAllHiddenAsync();

        // Assert
        result.Count().ShouldBe(1);
        result.First().Status.ShouldBe("Hidden");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/projects?view=hidden");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithNewProperties_DeserializesCorrectly()
    {
        // Arrange
        Project project = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/projects/50"),
            Contact = new Uri("https://api.freeagent.com/v2/contacts/60"),
            ContactName = "Test Client Ltd",
            Name = "Full Featured Project",
            Status = "Active",
            Currency = "GBP",
            BudgetUnits = "Hours",
            Budget = 100.00m,
            UsesProjectInvoiceSequence = false,
            IncludeUnbilledTimeInProfitability = true,
            IsDeletable = false,
            CreatedAt = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        ProjectRoot responseRoot = new() { Project = project };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Project result = await this.projects.GetByIdAsync("50");

        // Assert
        result.ShouldNotBeNull();
        result.ContactName.ShouldBe("Test Client Ltd");
        result.IncludeUnbilledTimeInProfitability.ShouldBe(true);
        result.IsDeletable.ShouldBe(false);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/projects/50");
    }

    [TestMethod]
    public async Task GetAllAsync_WithSortingByUpdatedAtDescending_RequestsCorrectEndpoint()
    {
        // Arrange
        Project testProject = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/projects/1"),
            Contact = new Uri("https://api.freeagent.com/v2/contacts/1"),
            Name = "Test Project",
            Status = "Active",
            Currency = "GBP",
            BudgetUnits = "Hours",
            Budget = 100m,
            UsesProjectInvoiceSequence = false
        };
        ProjectsRoot projectsData = new() { Projects = [testProject] };
        string responseJson = JsonSerializer.Serialize(projectsData, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Project> result = await this.projects.GetAllAsync(sort: "-updated_at", nested: null);

        // Assert
        result.Count().ShouldBe(1);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/projects?sort=-updated_at");
    }
}
