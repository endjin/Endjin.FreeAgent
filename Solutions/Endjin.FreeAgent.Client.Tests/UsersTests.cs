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

    [TestMethod]
    public async Task GetAllUsersAsync_WithViewParameter_IncludesViewInQueryString()
    {
        // Arrange
        ImmutableList<User> usersList = ImmutableList.Create(
            new User { FirstName = "Staff", LastName = "User", Role = Role.Employee, Hidden = false }
        );

        UsersRoot responseRoot = new() { Users = usersList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<User> result = await this.users.GetAllUsersAsync("active_staff");

        // Assert
        result.Count().ShouldBe(1);

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/users?view=active_staff");
    }

    [TestMethod]
    public async Task GetCurrentUserAsync_ReturnsCurrentUser()
    {
        // Arrange
        User currentUser = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/users/1"),
            FirstName = "Current",
            LastName = "User",
            Email = "current@example.com",
            Role = Role.Owner
        };

        UserRoot responseRoot = new() { User = currentUser };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        User result = await this.users.GetCurrentUserAsync();

        // Assert
        result.ShouldNotBeNull();
        result.FirstName.ShouldBe("Current");
        result.LastName.ShouldBe("User");
        result.Email.ShouldBe("current@example.com");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/users/me");
    }

    [TestMethod]
    public async Task CreateAsync_CreatesNewUser()
    {
        // Arrange
        User newUser = new()
        {
            FirstName = "New",
            LastName = "User",
            Email = "new@example.com",
            Role = Role.Employee,
            SendInvitation = true
        };

        User createdUser = newUser with
        {
            Url = new Uri("https://api.freeagent.com/v2/users/123"),
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        UserRoot responseRoot = new() { User = createdUser };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        User result = await this.users.CreateAsync(newUser);

        // Assert
        result.ShouldNotBeNull();
        result.Url.ShouldNotBeNull();
        result.FirstName.ShouldBe("New");
        result.LastName.ShouldBe("User");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/users");
    }

    [TestMethod]
    public async Task UpdateAsync_UpdatesExistingUser()
    {
        // Arrange
        User updatedUser = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/users/123"),
            FirstName = "Updated",
            LastName = "User",
            Email = "updated@example.com",
            Role = Role.Director
        };

        UserRoot responseRoot = new() { User = updatedUser };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        User result = await this.users.UpdateAsync("123", updatedUser);

        // Assert
        result.ShouldNotBeNull();
        result.FirstName.ShouldBe("Updated");
        result.Role.ShouldBe(Role.Director);

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/users/123");
    }

    [TestMethod]
    public async Task UpdateCurrentUserAsync_UpdatesOwnProfile()
    {
        // Arrange
        User updatedProfile = new()
        {
            FirstName = "Updated",
            LastName = "Profile",
            Email = "profile@example.com"
        };

        UserRoot responseRoot = new() { User = updatedProfile };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        User result = await this.users.UpdateCurrentUserAsync(updatedProfile);

        // Assert
        result.ShouldNotBeNull();
        result.FirstName.ShouldBe("Updated");
        result.LastName.ShouldBe("Profile");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/users/me");
    }

    [TestMethod]
    public async Task DeleteAsync_DeletesUser()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK);

        // Act
        await this.users.DeleteAsync("123");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/users/123");
    }

    [TestMethod]
    public async Task GetByIdAsync_ReturnsSpecificUser()
    {
        // Arrange
        User specificUser = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/users/42"),
            FirstName = "Specific",
            LastName = "User",
            Email = "specific@example.com",
            Role = Role.Employee
        };

        UserRoot responseRoot = new() { User = specificUser };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        User result = await this.users.GetByIdAsync("42");

        // Assert
        result.ShouldNotBeNull();
        result.FirstName.ShouldBe("Specific");
        result.LastName.ShouldBe("User");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/users/42");
    }

    [TestMethod]
    public async Task GetAllUsersAsync_WithAllView_IncludesViewInQueryString()
    {
        // Arrange
        ImmutableList<User> usersList = ImmutableList.Create(
            new User { FirstName = "Test", LastName = "User", Role = Role.Employee, Hidden = false }
        );

        UsersRoot responseRoot = new() { Users = usersList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<User> result = await this.users.GetAllUsersAsync("all");

        // Assert
        result.Count().ShouldBe(1);

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/users?view=all");
    }

    [TestMethod]
    public async Task GetAllUsersAsync_WithStaffView_IncludesViewInQueryString()
    {
        // Arrange
        ImmutableList<User> usersList = ImmutableList.Create(
            new User { FirstName = "Staff", LastName = "User", Role = Role.Employee, Hidden = false }
        );

        UsersRoot responseRoot = new() { Users = usersList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<User> result = await this.users.GetAllUsersAsync("staff");

        // Assert
        result.Count().ShouldBe(1);

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/users?view=staff");
    }

    [TestMethod]
    public async Task GetAllUsersAsync_WithAdvisorsView_IncludesViewInQueryString()
    {
        // Arrange
        ImmutableList<User> usersList = ImmutableList.Create(
            new User { FirstName = "Advisor", LastName = "User", Role = Role.Accountant, Hidden = false }
        );

        UsersRoot responseRoot = new() { Users = usersList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<User> result = await this.users.GetAllUsersAsync("advisors");

        // Assert
        result.Count().ShouldBe(1);

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/users?view=advisors");
    }

    [TestMethod]
    public async Task GetAllUsersAsync_WithActiveAdvisorsView_IncludesViewInQueryString()
    {
        // Arrange
        ImmutableList<User> usersList = ImmutableList.Create(
            new User { FirstName = "Active", LastName = "Advisor", Role = Role.Accountant, Hidden = false }
        );

        UsersRoot responseRoot = new() { Users = usersList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<User> result = await this.users.GetAllUsersAsync("active_advisors");

        // Assert
        result.Count().ShouldBe(1);

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/users?view=active_advisors");
    }

    [TestMethod]
    public async Task GetAllUsersAsync_WithNoView_OmitsViewFromQueryString()
    {
        // Arrange
        ImmutableList<User> usersList = ImmutableList.Create(
            new User { FirstName = "Test", LastName = "User", Role = Role.Employee, Hidden = false }
        );

        UsersRoot responseRoot = new() { Users = usersList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<User> result = await this.users.GetAllUsersAsync();

        // Assert
        result.Count().ShouldBe(1);

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/users");
    }

    [TestMethod]
    public async Task GetCurrentUserAsync_WithPayrollProfile_DeserializesPayrollData()
    {
        // Arrange
        User currentUser = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/users/1"),
            FirstName = "Current",
            LastName = "User",
            Email = "current@example.com",
            Role = Role.Employee,
            CurrentPayrollProfile = new UserPayrollProfile
            {
                TotalPayInPreviousEmployment = 25000.50m,
                TotalTaxInPreviousEmployment = 5000.25m
            }
        };

        UserRoot responseRoot = new() { User = currentUser };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        User result = await this.users.GetCurrentUserAsync();

        // Assert
        result.ShouldNotBeNull();
        result.CurrentPayrollProfile.ShouldNotBeNull();
        result.CurrentPayrollProfile.TotalPayInPreviousEmployment.ShouldBe(25000.50m);
        result.CurrentPayrollProfile.TotalTaxInPreviousEmployment.ShouldBe(5000.25m);

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/users/me");
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.cache?.Dispose();
        this.httpClient?.Dispose();
        this.messageHandler?.Dispose();
    }

}
