// <copyright file="EstimateSettingsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class EstimateSettingsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private EstimateSettings estimateSettings = null!;
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
        this.estimateSettings = new EstimateSettings(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetDefaultAdditionalTextAsync_ReturnsDefaultText()
    {
        // Arrange
        EstimateDefaultAdditionalText additionalText = new()
        {
            Text = "This quote is valid for 30 days. Terms and conditions apply."
        };

        EstimateDefaultAdditionalTextRoot responseRoot = new() { Estimate = additionalText };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        EstimateDefaultAdditionalText result = await this.estimateSettings.GetDefaultAdditionalTextAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Text.ShouldBe("This quote is valid for 30 days. Terms and conditions apply.");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/estimates/default_additional_text");
    }

    [TestMethod]
    public async Task GetDefaultAdditionalTextAsync_CachesResult()
    {
        // Arrange
        EstimateDefaultAdditionalText additionalText = new()
        {
            Text = "Cached text"
        };

        EstimateDefaultAdditionalTextRoot responseRoot = new() { Estimate = additionalText };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        EstimateDefaultAdditionalText result1 = await this.estimateSettings.GetDefaultAdditionalTextAsync();
        EstimateDefaultAdditionalText result2 = await this.estimateSettings.GetDefaultAdditionalTextAsync();

        // Assert
        result1.ShouldNotBeNull();
        result2.ShouldNotBeNull();
        result1.Text.ShouldBe("Cached text");

        // Mock Verification - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task UpdateDefaultAdditionalTextAsync_UpdatesAndReturnsText()
    {
        // Arrange
        string newText = "Updated estimate terms and conditions.";

        EstimateDefaultAdditionalText additionalText = new()
        {
            Text = newText
        };

        EstimateDefaultAdditionalTextRoot responseRoot = new() { Estimate = additionalText };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        EstimateDefaultAdditionalText result = await this.estimateSettings.UpdateDefaultAdditionalTextAsync(newText);

        // Assert
        result.ShouldNotBeNull();
        result.Text.ShouldBe(newText);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/estimates/default_additional_text");
    }

    [TestMethod]
    public async Task UpdateDefaultAdditionalTextAsync_InvalidatesCache()
    {
        // Arrange - First get to populate cache
        EstimateDefaultAdditionalText originalText = new() { Text = "Original text" };
        EstimateDefaultAdditionalTextRoot originalRoot = new() { Estimate = originalText };
        string originalJson = JsonSerializer.Serialize(originalRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(originalJson, Encoding.UTF8, "application/json")
        };

        await this.estimateSettings.GetDefaultAdditionalTextAsync();

        // Update
        EstimateDefaultAdditionalText updatedText = new() { Text = "Updated text" };
        EstimateDefaultAdditionalTextRoot updatedRoot = new() { Estimate = updatedText };
        string updatedJson = JsonSerializer.Serialize(updatedRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(updatedJson, Encoding.UTF8, "application/json")
        };

        // Act
        await this.estimateSettings.UpdateDefaultAdditionalTextAsync("Updated text");

        // Get again to verify cache was invalidated
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(updatedJson, Encoding.UTF8, "application/json")
        };

        EstimateDefaultAdditionalText result = await this.estimateSettings.GetDefaultAdditionalTextAsync();

        // Assert
        result.Text.ShouldBe("Updated text");

        // Mock Verification - Should have made 3 calls (initial get, update, second get after cache invalidation)
        this.messageHandler.CallCount.ShouldBe(3);
    }

    [TestMethod]
    public async Task DeleteDefaultAdditionalTextAsync_DeletesText()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        await this.estimateSettings.DeleteDefaultAdditionalTextAsync();

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/estimates/default_additional_text");
    }

    [TestMethod]
    public async Task DeleteDefaultAdditionalTextAsync_InvalidatesCache()
    {
        // Arrange - First get to populate cache
        EstimateDefaultAdditionalText text = new() { Text = "Text to delete" };
        EstimateDefaultAdditionalTextRoot root = new() { Estimate = text };
        string json = JsonSerializer.Serialize(root, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        await this.estimateSettings.GetDefaultAdditionalTextAsync();

        // Delete
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        await this.estimateSettings.DeleteDefaultAdditionalTextAsync();

        // Get again to verify cache was invalidated
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        await this.estimateSettings.GetDefaultAdditionalTextAsync();

        // Assert - Mock Verification: Should have made 3 calls (initial get, delete, second get after cache invalidation)
        this.messageHandler.CallCount.ShouldBe(3);
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.cache?.Dispose();
        this.httpClient?.Dispose();
        this.messageHandler?.Dispose();
    }
}
