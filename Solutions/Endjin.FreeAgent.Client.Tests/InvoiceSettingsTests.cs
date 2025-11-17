// <copyright file="InvoiceSettingsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class InvoiceSettingsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private InvoiceSettings invoiceSettings = null!;
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
        this.invoiceSettings = new InvoiceSettings(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetDefaultAdditionalTextAsync_ReturnsDefaultText()
    {
        // Arrange
        InvoiceDefaultAdditionalText additionalText = new()
        {
            Text = "Payment due within 30 days. Bank details: Sort Code 12-34-56, Account 87654321."
        };

        InvoiceDefaultAdditionalTextRoot responseRoot = new() { Invoice = additionalText };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        InvoiceDefaultAdditionalText result = await this.invoiceSettings.GetDefaultAdditionalTextAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Text.ShouldBe("Payment due within 30 days. Bank details: Sort Code 12-34-56, Account 87654321.");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/invoices/default_additional_text");
    }

    [TestMethod]
    public async Task GetDefaultAdditionalTextAsync_CachesResult()
    {
        // Arrange
        InvoiceDefaultAdditionalText additionalText = new()
        {
            Text = "Cached invoice text"
        };

        InvoiceDefaultAdditionalTextRoot responseRoot = new() { Invoice = additionalText };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        InvoiceDefaultAdditionalText result1 = await this.invoiceSettings.GetDefaultAdditionalTextAsync();
        InvoiceDefaultAdditionalText result2 = await this.invoiceSettings.GetDefaultAdditionalTextAsync();

        // Assert
        result1.ShouldNotBeNull();
        result2.ShouldNotBeNull();
        result1.Text.ShouldBe("Cached invoice text");

        // Mock Verification - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task UpdateDefaultAdditionalTextAsync_UpdatesAndReturnsText()
    {
        // Arrange
        string newText = "Updated payment terms: Net 60 days.";

        InvoiceDefaultAdditionalText additionalText = new()
        {
            Text = newText
        };

        InvoiceDefaultAdditionalTextRoot responseRoot = new() { Invoice = additionalText };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        InvoiceDefaultAdditionalText result = await this.invoiceSettings.UpdateDefaultAdditionalTextAsync(newText);

        // Assert
        result.ShouldNotBeNull();
        result.Text.ShouldBe(newText);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/invoices/default_additional_text");
    }

    [TestMethod]
    public async Task UpdateDefaultAdditionalTextAsync_InvalidatesCache()
    {
        // Arrange - First get to populate cache
        InvoiceDefaultAdditionalText originalText = new() { Text = "Original invoice text" };
        InvoiceDefaultAdditionalTextRoot originalRoot = new() { Invoice = originalText };
        string originalJson = JsonSerializer.Serialize(originalRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(originalJson, Encoding.UTF8, "application/json")
        };

        await this.invoiceSettings.GetDefaultAdditionalTextAsync();

        // Update
        InvoiceDefaultAdditionalText updatedText = new() { Text = "Updated invoice text" };
        InvoiceDefaultAdditionalTextRoot updatedRoot = new() { Invoice = updatedText };
        string updatedJson = JsonSerializer.Serialize(updatedRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(updatedJson, Encoding.UTF8, "application/json")
        };

        // Act
        await this.invoiceSettings.UpdateDefaultAdditionalTextAsync("Updated invoice text");

        // Get again to verify cache was invalidated
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(updatedJson, Encoding.UTF8, "application/json")
        };

        InvoiceDefaultAdditionalText result = await this.invoiceSettings.GetDefaultAdditionalTextAsync();

        // Assert
        result.Text.ShouldBe("Updated invoice text");

        // Mock Verification - Should have made 3 calls (initial get, update, second get after cache invalidation)
        this.messageHandler.CallCount.ShouldBe(3);
    }

    [TestMethod]
    public async Task DeleteDefaultAdditionalTextAsync_DeletesText()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        await this.invoiceSettings.DeleteDefaultAdditionalTextAsync();

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/invoices/default_additional_text");
    }

    [TestMethod]
    public async Task DeleteDefaultAdditionalTextAsync_InvalidatesCache()
    {
        // Arrange - First get to populate cache
        InvoiceDefaultAdditionalText text = new() { Text = "Invoice text to delete" };
        InvoiceDefaultAdditionalTextRoot root = new() { Invoice = text };
        string json = JsonSerializer.Serialize(root, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        await this.invoiceSettings.GetDefaultAdditionalTextAsync();

        // Delete
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        await this.invoiceSettings.DeleteDefaultAdditionalTextAsync();

        // Get again to verify cache was invalidated
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        await this.invoiceSettings.GetDefaultAdditionalTextAsync();

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
