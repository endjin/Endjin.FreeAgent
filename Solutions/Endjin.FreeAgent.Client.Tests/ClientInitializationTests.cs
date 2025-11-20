// <copyright file="ClientInitializationTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class ClientInitializationTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;

    [TestInitialize]
    public void Setup()
    {
        this.cache = new MemoryCache(new MemoryCacheOptions());
        this.httpClientFactory = Substitute.For<IHttpClientFactory>();
        this.httpClientFactory.CreateClient(Arg.Any<string>()).Returns(new HttpClient());
        this.loggerFactory = Substitute.For<ILoggerFactory>();
    }

    [TestMethod]
    public void FreeAgentClient_WhenInitialized_IsInitializedPropertyShouldBeFalse()
    {
        // Arrange & Act
        FreeAgentClient client = new(new FreeAgentOptionsBuilder(), this.cache, this.httpClientFactory, this.loggerFactory);

        // Assert
        client.IsInitialized.ShouldBeFalse();
    }




    [TestMethod]
    public void FreeAgentClient_InitializesAllServiceEndpoints()
    {
        // Arrange & Act
        FreeAgentClient client = new(new FreeAgentOptionsBuilder(), this.cache, this.httpClientFactory, this.loggerFactory);

        // Assert - All service endpoints should be initialized
        client.Contacts.ShouldNotBeNull();
        client.Estimates.ShouldNotBeNull();
        client.Expenses.ShouldNotBeNull();
        client.JournalSets.ShouldNotBeNull();
        client.Projects.ShouldNotBeNull();
        client.Tasks.ShouldNotBeNull();
        client.Timeslips.ShouldNotBeNull();
        client.Users.ShouldNotBeNull();
        client.Notes.ShouldNotBeNull();
    }

    [TestMethod]
    public void FreeAgentClient_WithNSubstituteCache_WorksCorrectly()
    {
        // Arrange
        IMemoryCache mockCache = Substitute.For<IMemoryCache>();
        ICacheEntry cacheEntry = Substitute.For<ICacheEntry>();

        mockCache.CreateEntry(Arg.Any<object>()).Returns(cacheEntry);

        // Act
        FreeAgentClient client = new(new FreeAgentOptionsBuilder(), mockCache, this.httpClientFactory, this.loggerFactory);

        // Assert
        client.ShouldNotBeNull();
        client.Contacts.ShouldNotBeNull();
    }

    [TestMethod]
    public void FreeAgentClient_WithDefaultOptions_UsesProductionApiBaseUrl()
    {
        // Arrange
        FreeAgentOptions options = new FreeAgentOptionsBuilder().Build();

        // Act
        FreeAgentClient client = new(options, this.cache, this.httpClientFactory, this.loggerFactory);

        // Assert
        client.ApiBaseUrl.ShouldBe(FreeAgentOptions.ProductionApiBaseUrl);
    }

    [TestMethod]
    public void FreeAgentClient_WithUseSandboxTrue_UsesSandboxApiBaseUrl()
    {
        // Arrange
        FreeAgentOptions options = new FreeAgentOptionsBuilder()
            .WithUseSandbox(true)
            .Build();

        // Act
        FreeAgentClient client = new(options, this.cache, this.httpClientFactory, this.loggerFactory);

        // Assert
        client.ApiBaseUrl.ShouldBe(FreeAgentOptions.SandboxApiBaseUrl);
    }

    [TestMethod]
    public void FreeAgentClient_WithExplicitCredentialsAndUseSandboxTrue_UsesSandboxApiBaseUrl()
    {
        // Arrange & Act
        FreeAgentClient client = FreeAgentClient.CreateInteractive(
            "client_id",
            "client_secret",
            useSandbox: true,
            this.cache,
            this.httpClientFactory,
            this.loggerFactory,
            "refresh_token");

        // Assert
        client.ApiBaseUrl.ShouldBe(FreeAgentOptions.SandboxApiBaseUrl);
    }

    [TestMethod]
    public void FreeAgentClient_WithExplicitCredentialsDefaultUseSandbox_UsesProductionApiBaseUrl()
    {
        // Arrange & Act
        FreeAgentClient client = FreeAgentClient.CreateInteractive(
            "client_id",
            "client_secret",
            useSandbox: false,
            this.cache,
            this.httpClientFactory,
            this.loggerFactory,
            "refresh_token");

        // Assert
        client.ApiBaseUrl.ShouldBe(FreeAgentOptions.ProductionApiBaseUrl);
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.cache?.Dispose();
    }
}
