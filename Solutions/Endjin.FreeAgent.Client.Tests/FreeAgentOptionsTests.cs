// <copyright file="FreeAgentOptionsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class FreeAgentOptionsTests
{
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;

    [TestInitialize]
    public void Setup()
    {
        this.httpClientFactory = Substitute.For<IHttpClientFactory>();
        this.httpClientFactory.CreateClient(Arg.Any<string>()).Returns(new HttpClient());
        this.loggerFactory = Substitute.For<ILoggerFactory>();
    }

    [TestMethod]
    public void FreeAgentOptions_WithValidConfiguration_PassesValidation()
    {
        // Arrange
        FreeAgentOptionsBuilder options = new();

        // Act & Assert - Should not throw
        Should.NotThrow(() => options.Build().Validate());
    }

    [TestMethod]
    public void FreeAgentOptions_WithMissingClientId_ThrowsInvalidOperationException()
    {
        // Arrange
        FreeAgentOptionsBuilder options = new FreeAgentOptionsBuilder()
            .WithEmptyClientId();

        // Act & Assert
        InvalidOperationException exception = Should.Throw<InvalidOperationException>(() => options.Build().Validate());
        exception.Message.ShouldBe(
            "Invalid FreeAgent configuration: ClientId is required for FreeAgent authentication");
    }

    [TestMethod]
    public void FreeAgentOptions_WithMissingClientSecret_ThrowsInvalidOperationException()
    {
        // Arrange
        FreeAgentOptionsBuilder options = new FreeAgentOptionsBuilder()
            .WithEmptyClientSecret();

        // Act & Assert
        InvalidOperationException exception = Should.Throw<InvalidOperationException>(() => options.Build().Validate());
        exception.Message.ShouldBe(
            "Invalid FreeAgent configuration: ClientSecret is required for FreeAgent authentication");
    }

    [TestMethod]
    public void FreeAgentOptions_WithMissingRefreshToken_ThrowsInvalidOperationException()
    {
        // Arrange
        FreeAgentOptionsBuilder options = new FreeAgentOptionsBuilder()
            .WithEmptyRefreshToken();

        // Act & Assert
        InvalidOperationException exception = Should.Throw<InvalidOperationException>(() => options.Build().Validate());
        exception.Message.ShouldBe(
            "Invalid FreeAgent configuration: RefreshToken is required for FreeAgent authentication");
    }

    [TestMethod]
    public void FreeAgentOptions_SectionName_ReturnsCorrectValue()
    {
        // Act & Assert
        FreeAgentOptions.SectionName.ShouldBe("FreeAgent");
    }

    [TestMethod]
    public void FreeAgentOptions_FromConfiguration_BindsCorrectly()
    {
        // Arrange
        Dictionary<string, string?> inMemorySettings = new()
        {
            ["FreeAgent:ClientId"] = "config_client_id",
            ["FreeAgent:ClientSecret"] = "config_client_secret",
            ["FreeAgent:RefreshToken"] = "config_refresh_token"
        };

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Act
        FreeAgentOptions options = new();
        configuration.GetSection(FreeAgentOptions.SectionName).Bind(options);

        // Assert
        options.ClientId.ShouldBe("config_client_id");
        options.ClientSecret.ShouldBe("config_client_secret");
        options.RefreshToken.ShouldBe("config_refresh_token");
        Should.NotThrow(() => options.Validate());
    }

    [TestMethod]
    public void FreeAgentClient_ConstructorWithOptions_InitializesAllServices()
    {
        // Arrange
        FreeAgentOptionsBuilder options = new();
        MemoryCache cache = new(new MemoryCacheOptions());

        // Act
        FreeAgentClient client = new(options, cache, this.httpClientFactory, this.loggerFactory);

        // Assert
        client.ShouldNotBeNull();
        client.Contacts.ShouldNotBeNull();
        client.Projects.ShouldNotBeNull();
        client.Timeslips.ShouldNotBeNull();
        client.Users.ShouldNotBeNull();
        client.Notes.ShouldNotBeNull();
        client.Estimates.ShouldNotBeNull();
        client.Expenses.ShouldNotBeNull();
        client.JournalSets.ShouldNotBeNull();
        client.Tasks.ShouldNotBeNull();
    }

    [TestMethod]
    public void FreeAgentClient_ConstructorWithIOptions_InitializesAllServices()
    {
        // Arrange
        FreeAgentOptionsBuilder options = new();
        IOptions<FreeAgentOptions> optionsWrapper = Options.Create(options.Build());
        MemoryCache cache = new(new MemoryCacheOptions());

        // Act
        FreeAgentClient client = new(optionsWrapper, cache, this.httpClientFactory, this.loggerFactory);

        // Assert
        client.ShouldNotBeNull();
        client.Contacts.ShouldNotBeNull();
        client.Projects.ShouldNotBeNull();
    }

    [TestMethod]
    public void FreeAgentClient_ConstructorWithExplicitParams_InitializesAllServices()
    {
        // Arrange
        MemoryCache cache = new(new MemoryCacheOptions());

        // Act
        FreeAgentClient client = new("client_id", "client_secret", "refresh_token", cache, this.httpClientFactory,
            this.loggerFactory);

        // Assert
        client.ShouldNotBeNull();
        client.Contacts.ShouldNotBeNull();
        client.Projects.ShouldNotBeNull();
    }

    [TestMethod]
    public void FreeAgentClient_ConstructorWithInvalidOptions_ThrowsInvalidOperationException()
    {
        // Arrange
        FreeAgentOptionsBuilder options = new FreeAgentOptionsBuilder()
            .WithEmptyClientId();
        MemoryCache cache = new(new MemoryCacheOptions());

        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
            new FreeAgentClient(options, cache, this.httpClientFactory, this.loggerFactory));
    }

    [TestMethod]
    public void FreeAgentClient_ConstructorWithNullOptions_ThrowsArgumentNullException()
    {
        // Arrange
        FreeAgentOptions? options = null;
        MemoryCache cache = new(new MemoryCacheOptions());

        // Act & Assert
        Should.Throw<ArgumentNullException>(() =>
            new FreeAgentClient(options!, cache, this.httpClientFactory, this.loggerFactory));
    }

    [TestMethod]
    public void FreeAgentClient_ConstructorWithNullCache_ThrowsArgumentNullException()
    {
        // Arrange
        FreeAgentOptionsBuilder options = new();
        IMemoryCache? cache = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() =>
            new FreeAgentClient(options, cache!, this.httpClientFactory, this.loggerFactory));
    }

    [TestMethod]
    public void FreeAgentClient_ConstructorWithEmptyClientId_ThrowsArgumentException()
    {
        // Arrange
        MemoryCache cache = new(new MemoryCacheOptions());

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            new FreeAgentClient("", "secret", "token", cache, this.httpClientFactory, this.loggerFactory));
    }

    [TestMethod]
    public void FreeAgentOptionsValidator_WithValidOptions_ReturnsSuccess()
    {
        // Arrange
        FreeAgentOptionsValidator validator = new();
        FreeAgentOptionsBuilder options = new();

        // Act
        ValidateOptionsResult result = validator.Validate(null, options);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.FailureMessage.ShouldBeNull();
    }

    [TestMethod]
    public void FreeAgentOptionsValidator_WithInvalidOptions_ReturnsFailureWithMessage()
    {
        // Arrange
        FreeAgentOptionsValidator validator = new();
        FreeAgentOptionsBuilder options = new FreeAgentOptionsBuilder()
            .WithEmptyClientId();

        // Act
        ValidateOptionsResult result = validator.Validate(null, options);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.FailureMessage.ShouldBe(
            "Invalid FreeAgent configuration: ClientId is required for FreeAgent authentication");
    }

    [TestMethod]
    public void FreeAgentOptionsValidator_WithNullOptions_ReturnsFailure()
    {
        // Arrange
        FreeAgentOptionsValidator validator = new();

        // Act
        ValidateOptionsResult result = validator.Validate(null, null!);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.FailureMessage.ShouldBe("FreeAgentOptions cannot be null");
    }
}
