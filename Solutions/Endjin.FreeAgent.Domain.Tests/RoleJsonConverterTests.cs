// <copyright file="RoleJsonConverterTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Converters;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Endjin.FreeAgent.Domain.Tests;

[TestClass]
public class RoleJsonConverterTests
{
    private readonly JsonSerializerOptions options;

    public RoleJsonConverterTests()
    {
        this.options = new JsonSerializerOptions
        {
            Converters = { new RoleJsonConverter() }
        };
    }

    [TestMethod]
    public void RoleConverter_DeserializesSnakeCase_CompanySecretary()
    {
        // Arrange
        string json = """{"role": "company_secretary"}""";

        // Act
        TestRole? result = JsonSerializer.Deserialize<TestRole>(json, options);

        // Assert
        result.ShouldNotBeNull();
        result.Role.ShouldBe(Role.CompanySecretary);
    }

    [TestMethod]
    public void RoleConverter_DeserializesLowercase_Employee()
    {
        // Arrange
        string json = """{"role": "employee"}""";

        // Act
        TestRole? result = JsonSerializer.Deserialize<TestRole>(json, options);

        // Assert
        result.ShouldNotBeNull();
        result.Role.ShouldBe(Role.Employee);
    }

    [TestMethod]
    public void RoleConverter_DeserializesLowercase_Director()
    {
        // Arrange
        string json = """{"role": "director"}""";

        // Act
        TestRole? result = JsonSerializer.Deserialize<TestRole>(json, options);

        // Assert
        result.ShouldNotBeNull();
        result.Role.ShouldBe(Role.Director);
    }

    [TestMethod]
    public void RoleConverter_DeserializesNull_ReturnsNull()
    {
        // Arrange
        string json = """{"role": null}""";

        // Act
        TestRole? result = JsonSerializer.Deserialize<TestRole>(json, options);

        // Assert
        result.ShouldNotBeNull();
        result.Role.ShouldBeNull();
    }

    [TestMethod]
    public void RoleConverter_SerializesCompanySecretary_AsTitleCase()
    {
        // Arrange
        TestRole obj = new() { Role = Role.CompanySecretary };

        // Act
        string json = JsonSerializer.Serialize(obj, options);

        // Assert
        json.ShouldContain("\"role\":\"Company Secretary\"");
    }

    [TestMethod]
    public void RoleConverter_SerializesEmployee_AsTitleCase()
    {
        // Arrange
        TestRole obj = new() { Role = Role.Employee };

        // Act
        string json = JsonSerializer.Serialize(obj, options);

        // Assert
        json.ShouldContain("\"role\":\"Employee\"");
    }

    [TestMethod]
    public void RoleConverter_HandlesAllRoleValues()
    {
        // Arrange
        (string, Role)[] testCases =
        [
            ("owner", Role.Owner),
            ("director", Role.Director),
            ("partner", Role.Partner),
            ("company_secretary", Role.CompanySecretary),
            ("employee", Role.Employee),
            ("shareholder", Role.Shareholder),
            ("accountant", Role.Accountant)
        ];

        foreach ((string jsonValue, Role expectedRole) in testCases)
        {
            // Act
            string json = $"{{\"role\": \"{jsonValue}\"}}";
            TestRole? result = JsonSerializer.Deserialize<TestRole>(json, options);

            // Assert
            result.ShouldNotBeNull();
            result.Role.ShouldBe(expectedRole, $"Failed for JSON value: {jsonValue}");
        }
    }

    [TestMethod]
    public void RoleConverter_ThrowsForInvalidValue()
    {
        // Arrange
        string json = """{"role": "invalid_role"}""";

        // Act & Assert
        JsonException exception = Should.Throw<JsonException>(() =>
            JsonSerializer.Deserialize<TestRole>(json, options));

        exception.Message.ShouldContain("Unable to convert 'invalid_role' to Role enum");
    }

    private class TestRole
    {
        [JsonPropertyName("role")]
        [JsonConverter(typeof(RoleJsonConverter))]
        public Role? Role { get; init; }
    }
}
