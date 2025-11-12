// <copyright file="HttpResponseMessageExtensionsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class HttpResponseMessageExtensionsTests
{
    [TestMethod]
    public async Task ReadAsAsync_WithValidJson_DeserializesCorrectly()
    {
        // Arrange
        Contact testData = new()
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            OrganisationName = "Test Org"
        };

        string jsonContent = JsonSerializer.Serialize(testData, SharedJsonOptions.Instance);
        StringContent httpContent = new(jsonContent, Encoding.UTF8, "application/json");

        // Act
        Contact result = await httpContent.ReadAsAsync<Contact>();

        // Assert
        result.ShouldNotBeNull();
        result.FirstName.ShouldBe("John");
        result.LastName.ShouldBe("Doe");
        result.Email.ShouldBe("john@example.com");
        result.OrganisationName.ShouldBe("Test Org");
    }

    [TestMethod]
    public async Task ReadAsAsync_WithComplexObject_DeserializesNestedProperties()
    {
        // Arrange
        List<Contact> contactsList =
        [
            new() { FirstName = "Alice", LastName = "Smith" },
            new() { FirstName = "Bob", LastName = "Jones" }
        ];

        ContactsRoot testData = new() { Contacts = contactsList.ToImmutableList() };

        string jsonContent = JsonSerializer.Serialize(testData, SharedJsonOptions.Instance);
        StringContent httpContent = new(jsonContent, Encoding.UTF8, "application/json");

        // Act
        ContactsRoot result = await httpContent.ReadAsAsync<ContactsRoot>();

        // Assert
        result.ShouldNotBeNull();
        result.Contacts.ShouldNotBeNull();
        result.Contacts.Count.ShouldBe(2);
        result.Contacts[0].FirstName.ShouldBe("Alice");
        result.Contacts[1].FirstName.ShouldBe("Bob");
    }

    [TestMethod]
    public async Task ReadAsAsync_WithEmptyJson_ReturnsEmptyObject()
    {
        // Arrange
        string jsonContent = "{}";
        StringContent httpContent = new(jsonContent, Encoding.UTF8, "application/json");

        // Act
        Contact result = await httpContent.ReadAsAsync<Contact>();

        // Assert
        result.ShouldNotBeNull();
        result.FirstName.ShouldBeNull();
        result.LastName.ShouldBeNull();
    }

    [TestMethod]
    public async Task ReadAsAsync_WithNullJson_ThrowsInvalidOperationException()
    {
        // Arrange
        string jsonContent = "null";
        StringContent httpContent = new(jsonContent, Encoding.UTF8, "application/json");

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(async () =>
            await httpContent.ReadAsAsync<Contact>());
    }

    [TestMethod]
    public async Task ReadAsAsync_WithInvalidJson_ThrowsJsonException()
    {
        // Arrange
        string jsonContent = "{ invalid json }";
        StringContent httpContent = new(jsonContent, Encoding.UTF8, "application/json");

        // Act & Assert
        await Should.ThrowAsync<JsonException>(async () =>
            await httpContent.ReadAsAsync<Contact>());
    }

    [TestMethod]
    public async Task ReadAsAsync_WithArrayJson_DeserializesCorrectly()
    {
        // Arrange
        List<string> testData = ["one", "two", "three"];
        string jsonContent = JsonSerializer.Serialize(testData, SharedJsonOptions.Instance);
        StringContent httpContent = new(jsonContent, Encoding.UTF8, "application/json");

        // Act
        List<string> result = await httpContent.ReadAsAsync<List<string>>();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(3);
        result[0].ShouldBe("one");
        result[1].ShouldBe("two");
        result[2].ShouldBe("three");
    }

    [TestMethod]
    public async Task ReadAsAsync_WithDateTimeOffset_DeserializesCorrectly()
    {
        // Arrange
        User testData = new()
        {
            FirstName = "Test",
            LastName = "User",
            CreatedAt = DateTimeOffset.Parse("2024-01-15T10:30:00Z"),
            UpdatedAt = DateTimeOffset.Parse("2024-01-16T14:45:00Z")
        };

        string jsonContent = JsonSerializer.Serialize(testData, SharedJsonOptions.Instance);
        StringContent httpContent = new(jsonContent, Encoding.UTF8, "application/json");

        // Act
        User result = await httpContent.ReadAsAsync<User>();

        // Assert
        result.ShouldNotBeNull();
        result.CreatedAt.ShouldNotBeNull();
        result.UpdatedAt.ShouldNotBeNull();
        result.CreatedAt?.Year.ShouldBe(2024);
        result.UpdatedAt?.Year.ShouldBe(2024);
    }

    [TestMethod]
    public async Task ReadAsAsync_WithEnumProperties_DeserializesCorrectly()
    {
        // Arrange
        User testData = new()
        {
            FirstName = "Director",
            LastName = "User",
            Role = Role.Director
        };

        string jsonContent = JsonSerializer.Serialize(testData, SharedJsonOptions.Instance);
        StringContent httpContent = new(jsonContent, Encoding.UTF8, "application/json");

        // Act
        User result = await httpContent.ReadAsAsync<User>();

        // Assert
        result.ShouldNotBeNull();
        result.Role.ShouldBe(Role.Director);
    }

    [TestMethod]
    public async Task ReadAsAsync_WithNumericProperties_DeserializesDecimalCorrectly()
    {
        // Arrange
        Project testData = new()
        {
            Name = "Test Project",
            NormalBillingRate = 125.50m,
            HoursPerDay = 7.5m,
            Budget = 10000.00m
        };

        string jsonContent = JsonSerializer.Serialize(testData, SharedJsonOptions.Instance);
        StringContent httpContent = new(jsonContent, Encoding.UTF8, "application/json");

        // Act
        Project result = await httpContent.ReadAsAsync<Project>();

        // Assert
        result.ShouldNotBeNull();
        result.NormalBillingRate.ShouldBe(125.50m);
        result.HoursPerDay.ShouldBe(7.5m);
        result.Budget.ShouldBe(10000.00m);
    }

    [TestMethod]
    public async Task ReadAsAsync_WithUriProperties_DeserializesCorrectly()
    {
        // Arrange
        Contact testData = new()
        {
            FirstName = "Test",
            Url = new Uri("https://api.freeagent.com/v2/contacts/123")
        };

        string jsonContent = JsonSerializer.Serialize(testData, SharedJsonOptions.Instance);
        StringContent httpContent = new(jsonContent, Encoding.UTF8, "application/json");

        // Act
        Contact result = await httpContent.ReadAsAsync<Contact>();

        // Assert
        result.ShouldNotBeNull();
        result.Url.ShouldNotBeNull();
        result.Url?.ToString().ShouldBe("https://api.freeagent.com/v2/contacts/123");
    }
}
