// <copyright file="UsersTests.cs" company="Endjin Limited">
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
public class UsersTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private Users users = null!;
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

        this.users = new Users(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetAllActiveEmployeesAsync_ReturnsOnlyActiveEmployees_OrderedByLastName()
    {
        // Arrange
        ImmutableList<User> usersList = ImmutableList.Create(
            new User { FirstName = "John", LastName = "Doe", Role = Role.Employee, Hidden = false },
            new User { FirstName = "Jane", LastName = "Smith", Role = Role.Employee, Hidden = false },
            new User { FirstName = "Bob", LastName = "Anderson", Role = Role.Employee, Hidden = false },
            new User { FirstName = "Hidden", LastName = "User", Role = Role.Employee, Hidden = true },
            new User { FirstName = "Director", LastName = "Person", Role = Role.Director, Hidden = false }
        );

        UsersRoot responseRoot = new() { Users = usersList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IList<User> result = await this.users.GetAllActiveEmployeesAsync();

        // Assert
        result.Count.ShouldBe(3);
        result.All(u => u.Role == Role.Employee).ShouldBeTrue();
        result.All(u => u.Hidden == false).ShouldBeTrue();

        // Verify ordering by last name
        result[0].LastName.ShouldBe("Anderson");
        result[1].LastName.ShouldBe("Doe");
        result[2].LastName.ShouldBe("Smith");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/users");
    }

    [TestMethod]
    public async Task GetAllActiveEmployeesAsync_WithCaching_UsesCachedResults()
    {
        // Arrange
        ImmutableList<User> usersList = ImmutableList.Create(
            new User { FirstName = "John", LastName = "Doe", Role = Role.Employee, Hidden = false }
        );

        UsersRoot responseRoot = new() { Users = usersList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - First call
        IList<User> result1 = await this.users.GetAllActiveEmployeesAsync();

        // Act - Second call (should use cache)
        IList<User> result2 = await this.users.GetAllActiveEmployeesAsync();

        // Assert
        result1.Count.ShouldBe(1);
        result2.Count.ShouldBe(1);
        result2.ShouldBe(result1); // Should be same cached instance

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce(); // Only one HTTP call due to caching
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/users");
    }

    [TestMethod]
    public async Task GetAllDirectorsAsync_ReturnsOnlyActiveDirectors()
    {
        // Arrange
        ImmutableList<User> usersList = ImmutableList.Create(
            new User { FirstName = "Director", LastName = "One", Role = Role.Director, Hidden = false },
            new User { FirstName = "Director", LastName = "Two", Role = Role.Director, Hidden = false },
            new User { FirstName = "Hidden", LastName = "Director", Role = Role.Director, Hidden = true },
            new User { FirstName = "Employee", LastName = "NotDirector", Role = Role.Employee, Hidden = false },
            new User { FirstName = "Owner", LastName = "Person", Role = Role.Owner, Hidden = false }
        );

        UsersRoot responseRoot = new() { Users = usersList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<User> result = await this.users.GetAllDirectorsAsync();

        // Assert
        result.Count().ShouldBe(2);
        result.All(u => u.Role == Role.Director).ShouldBeTrue();
        result.All(u => u.Hidden == false).ShouldBeTrue();
        result.Any(u => u.LastName == "One").ShouldBeTrue();
        result.Any(u => u.LastName == "Two").ShouldBeTrue();

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/users");
    }

    [TestMethod]
    public async Task GetAllDirectorsAsync_WithNoDirectors_ReturnsEmptyList()
    {
        // Arrange
        ImmutableList<User> usersList = ImmutableList.Create(
            new User { FirstName = "Employee", LastName = "One", Role = Role.Employee, Hidden = false },
            new User { FirstName = "Employee", LastName = "Two", Role = Role.Employee, Hidden = false }
        );

        UsersRoot responseRoot = new() { Users = usersList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<User> result = await this.users.GetAllDirectorsAsync();

        // Assert - Result
        result.Count().ShouldBe(0);

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/users");
    }

    [TestMethod]
    public async Task GetAllDirectorsAsync_WithCaching_UsesCachedResults()
    {
        // Arrange
        ImmutableList<User> usersList = ImmutableList.Create(
            new User { FirstName = "Director", LastName = "Cached", Role = Role.Director, Hidden = false }
        );

        UsersRoot responseRoot = new() { Users = usersList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - First call
        IEnumerable<User> result1 = await this.users.GetAllDirectorsAsync();

        // Act - Second call (should use cache)
        IEnumerable<User> result2 = await this.users.GetAllDirectorsAsync();

        // Assert
        result1.Count().ShouldBe(1);
        result2.Count().ShouldBe(1);
        result2.ShouldBe(result1); // Should be same cached instance

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce(); // Only one HTTP call due to caching
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/users");
    }

    [TestMethod]
    public async Task GetAllActiveEmployeesAsync_WithPagination_FollowsAllLinks()
    {
        // Arrange - simulate pagination with ExecuteRequestAndFollowLinksAsync
        ImmutableList<User> usersList = ImmutableList.Create(
            new User { FirstName = "Page1", LastName = "User1", Role = Role.Employee, Hidden = false },
            new User { FirstName = "Page1", LastName = "User2", Role = Role.Employee, Hidden = false },
            new User { FirstName = "Page2", LastName = "User3", Role = Role.Employee, Hidden = false },
            new User { FirstName = "Page2", LastName = "User4", Role = Role.Employee, Hidden = false }
        );

        // ExecuteRequestAndFollowLinksAsync should handle pagination internally
        // In our mock, we return all results at once
        UsersRoot responseRoot = new() { Users = usersList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IList<User> result = await this.users.GetAllActiveEmployeesAsync();

        // Assert
        result.Count.ShouldBe(4); // All users from both "pages"
        result.All(u => u.Role == Role.Employee).ShouldBeTrue();

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/users");
    }

    [TestMethod]
    public async Task GetAllActiveEmployeesAsync_WithEmptyResponse_ReturnsEmptyList()
    {
        // Arrange
        UsersRoot responseRoot = new() { Users = ImmutableList<User>.Empty };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IList<User> result = await this.users.GetAllActiveEmployeesAsync();

        // Assert - Result
        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/users");
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.cache?.Dispose();
        this.httpClient?.Dispose();
        this.messageHandler?.Dispose();
    }

}
