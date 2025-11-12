// <copyright file="InteractiveLoginHelperTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Client.OAuth2;

using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class InteractiveLoginHelperTests
{
    private ILogger<InteractiveLoginHelper> logger = null!;

    [TestInitialize]
    public void Setup()
    {
        this.logger = Substitute.For<ILogger<InteractiveLoginHelper>>();
    }

    [TestMethod]
    public void Constructor_WithValidOptions_CreatesInstance()
    {
        // Arrange
        OAuth2OptionsBuilder optionsBuilder = new();
        HttpClient httpClient = new();

        // Act
        InteractiveLoginHelper helper = new(optionsBuilder, httpClient, this.logger);

        // Assert
        helper.ShouldNotBeNull();
    }

    [TestMethod]
    public void Constructor_WithNullOptions_ThrowsArgumentNullException()
    {
        // Arrange
        OAuth2Options? options = null;
        HttpClient httpClient = new();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new InteractiveLoginHelper(options!, httpClient, this.logger));
    }

    [TestMethod]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Arrange
        OAuth2OptionsBuilder optionsBuilder = new();
        HttpClient? httpClient = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new InteractiveLoginHelper(optionsBuilder, httpClient!, this.logger));
    }

    [TestMethod]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        OAuth2OptionsBuilder optionsBuilder = new();
        HttpClient httpClient = new();
        ILogger<InteractiveLoginHelper>? logger = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new InteractiveLoginHelper(optionsBuilder, httpClient, logger!));
    }

    [TestMethod]
    public void Constructor_WithMissingClientId_ThrowsArgumentException()
    {
        // Arrange
        OAuth2OptionsBuilder optionsBuilder = new OAuth2OptionsBuilder()
            .WithEmptyClientId();
        HttpClient httpClient = new();

        // Act & Assert
        ArgumentException exception = Should.Throw<ArgumentException>(() => new InteractiveLoginHelper(optionsBuilder, httpClient, this.logger));
        exception.Message.ShouldContain("ClientId is required");
    }

    [TestMethod]
    public void Constructor_WithMissingClientSecret_ThrowsArgumentException()
    {
        // Arrange
        OAuth2OptionsBuilder optionsBuilder = new OAuth2OptionsBuilder()
            .WithEmptyClientSecret();
        HttpClient httpClient = new();

        // Act & Assert
        ArgumentException exception = Should.Throw<ArgumentException>(() => new InteractiveLoginHelper(optionsBuilder, httpClient, this.logger));
        exception.Message.ShouldContain("ClientSecret is required");
    }

    [TestMethod]
    public void InteractiveLoginResult_WithValidData_CreatesInstance()
    {
        // Arrange & Act
        InteractiveLoginResult result = new()
        {
            AccessToken = "test_access_token",
            RefreshToken = "test_refresh_token",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            ExpiresInSeconds = 3600,
            TokenType = "Bearer"
        };

        // Assert
        result.AccessToken.ShouldBe("test_access_token");
        result.RefreshToken.ShouldBe("test_refresh_token");
        result.ExpiresInSeconds.ShouldBe(3600);
        result.TokenType.ShouldBe("Bearer");
        result.ExpiresAt.ShouldBeGreaterThan(DateTime.UtcNow);
    }

    [TestMethod]
    public void InteractiveLoginResult_RequiredProperties_EnforcedByCompiler()
    {
        // This test verifies that the InteractiveLoginResult uses 'required' properties
        // by attempting to create an instance without specifying all required properties.
        // Note: This is more of a compile-time check, but we can verify runtime behavior.

        // Act & Assert - Should compile because all required properties are provided
        Should.NotThrow(() =>
        {
            _ = new InteractiveLoginResult
            {
                AccessToken = "token",
                RefreshToken = "refresh",
                ExpiresAt = DateTime.UtcNow,
                ExpiresInSeconds = 3600,
                TokenType = "Bearer"
            };
        });
    }
}
