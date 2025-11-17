// <copyright file="WebhooksTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class WebhooksTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private Webhooks webhooks = null!;
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
        this.webhooks = new Webhooks(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task CreateAsync_WithValidWebhook_ReturnsCreatedWebhook()
    {
        // Arrange
        Webhook inputWebhook = new()
        {
            PayloadUrl = new Uri("https://example.com/webhook"),
            Events = ["invoice.created"]
        };

        Webhook responseWebhook = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/webhooks/123"),
            PayloadUrl = new Uri("https://example.com/webhook"),
            Events = ["invoice.created"]
        };

        WebhookRoot responseRoot = new() { Webhook = responseWebhook };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Webhook result = await this.webhooks.CreateAsync(inputWebhook);

        // Assert
        result.ShouldNotBeNull();
        result.Url.ShouldNotBeNull();
        result.PayloadUrl.ShouldBe(new Uri("https://example.com/webhook"));

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/webhooks");
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllWebhooks()
    {
        // Arrange
        List<Webhook> webhooksList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/webhooks/1"),
                PayloadUrl = new Uri("https://example.com/webhook1"),
                Events = ["invoice.created"]
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/webhooks/2"),
                PayloadUrl = new Uri("https://example.com/webhook2"),
                Events = ["invoice.updated"]
            }
        ];

        WebhooksRoot responseRoot = new() { Webhooks = webhooksList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Webhook> result = await this.webhooks.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/webhooks");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsWebhook()
    {
        // Arrange
        Webhook webhook = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/webhooks/456"),
            PayloadUrl = new Uri("https://example.com/webhook"),
            Events = ["estimate.sent"]
        };

        WebhookRoot responseRoot = new() { Webhook = webhook };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Webhook result = await this.webhooks.GetByIdAsync("456");

        // Assert
        result.ShouldNotBeNull();
        result.Events.ShouldNotBeNull();
        result.Events.ShouldContain("estimate.sent");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/webhooks/456");
    }

    [TestMethod]
    public async Task UpdateAsync_WithValidWebhook_ReturnsUpdatedWebhook()
    {
        // Arrange
        Webhook updatedWebhook = new()
        {
            PayloadUrl = new Uri("https://example.com/updated-webhook"),
            Events = ["invoice.paid"]
        };

        Webhook responseWebhook = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/webhooks/789"),
            PayloadUrl = new Uri("https://example.com/updated-webhook"),
            Events = ["invoice.paid"]
        };

        WebhookRoot responseRoot = new() { Webhook = responseWebhook };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Webhook result = await this.webhooks.UpdateAsync("789", updatedWebhook);

        // Assert
        result.ShouldNotBeNull();
        result.PayloadUrl.ShouldBe(new Uri("https://example.com/updated-webhook"));

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/webhooks/789");
    }

    [TestMethod]
    public async Task DeleteAsync_WithValidId_DeletesWebhook()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        await this.webhooks.DeleteAsync("999");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/webhooks/999");
    }
}
