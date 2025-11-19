// <copyright file="AttachmentsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class AttachmentsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private Attachments attachments = null!;
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
        this.attachments = new Attachments(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsAttachment()
    {
        // Arrange
        Attachment attachment = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/attachments/20"),
            Filename = "document.pdf",
            Size = 98765,
            ContentType = "application/pdf",
            Description = "Important document",
            CreatedAt = new DateTime(2024, 3, 15, 10, 30, 0),
            ContentSrc = new Uri("https://s3.amazonaws.com/freeagent/attachments/20/document.pdf?expires=1234567890"),
            ExpiresAt = new DateTime(2024, 3, 15, 12, 30, 0, DateTimeKind.Utc)
        };

        AttachmentRoot responseRoot = new() { Attachment = attachment };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Attachment result = await this.attachments.GetByIdAsync("20");

        // Assert
        result.ShouldNotBeNull();
        result.Filename.ShouldBe("document.pdf");
        result.Size.ShouldBe(98765);
        result.Description.ShouldBe("Important document");
        result.ContentSrc.ShouldNotBeNull();
        result.ContentSrc.ToString().ShouldContain("document.pdf");
        result.ExpiresAt.ShouldNotBeNull();
        result.ExpiresAt.Value.ShouldBe(new DateTime(2024, 3, 15, 12, 30, 0, DateTimeKind.Utc));

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/attachments/20");
    }

    [TestMethod]
    public async Task GetByIdAsync_CachesResult()
    {
        // Arrange
        Attachment attachment = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/attachments/30"),
            Filename = "cached.jpg",
            Size = 2048,
            ContentType = "image/jpeg"
        };

        AttachmentRoot responseRoot = new() { Attachment = attachment };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        Attachment result1 = await this.attachments.GetByIdAsync("30");
        Attachment result2 = await this.attachments.GetByIdAsync("30");

        // Assert
        result1.ShouldNotBeNull();
        result2.ShouldNotBeNull();
        result1.Filename.ShouldBe("cached.jpg");

        // Mock Verification - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task DeleteAsync_WithValidId_DeletesAttachment()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        await this.attachments.DeleteAsync("50");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/attachments/50");
    }

    [TestMethod]
    public async Task DeleteAsync_InvalidatesCacheEntry()
    {
        // Arrange
        string attachmentId = "60";
        Attachment attachment = new()
        {
            Url = new Uri($"https://api.freeagent.com/v2/attachments/{attachmentId}"),
            Filename = "to_delete.pdf",
            Size = 512
        };

        AttachmentRoot root = new() { Attachment = attachment };

        // First call to GetByIdAsync to populate cache
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(root, SharedJsonOptions.Instance), Encoding.UTF8, "application/json")
        };
        await this.attachments.GetByIdAsync(attachmentId);

        // Delete call
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        await this.attachments.DeleteAsync(attachmentId);

        // Setup response for second GetByIdAsync (after cache invalidation)
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(root, SharedJsonOptions.Instance), Encoding.UTF8, "application/json")
        };
        await this.attachments.GetByIdAsync(attachmentId);

        // Assert - Mock Verification: Should have made 3 calls (initial get, delete, second get after cache invalidation)
        this.messageHandler.CallCount.ShouldBe(3);
    }

    [TestMethod]
    public async Task GetByIdAsync_WithImageAttachment_ReturnsThumbnailUrls()
    {
        // Arrange
        DateTime expiresAt = new(2024, 3, 15, 14, 0, 0, DateTimeKind.Utc);
        Attachment attachment = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/attachments/70"),
            Filename = "receipt.jpg",
            Size = 245678,
            ContentType = "image/jpeg",
            Description = "Expense receipt",
            CreatedAt = new DateTime(2024, 3, 15, 10, 0, 0, DateTimeKind.Utc),
            ContentSrc = new Uri("https://s3.amazonaws.com/freeagent/attachments/70/receipt.jpg?expires=1234567890"),
            ContentSrcMedium = new Uri("https://s3.amazonaws.com/freeagent/attachments/70/receipt_medium.jpg?expires=1234567890"),
            ContentSrcSmall = new Uri("https://s3.amazonaws.com/freeagent/attachments/70/receipt_small.jpg?expires=1234567890"),
            ExpiresAt = expiresAt
        };

        AttachmentRoot responseRoot = new() { Attachment = attachment };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Attachment result = await this.attachments.GetByIdAsync("70");

        // Assert
        result.ShouldNotBeNull();
        result.Filename.ShouldBe("receipt.jpg");
        result.ContentType.ShouldBe("image/jpeg");
        result.ContentSrc.ShouldNotBeNull();
        result.ContentSrc.ToString().ShouldContain("receipt.jpg");
        result.ContentSrcMedium.ShouldNotBeNull();
        result.ContentSrcMedium.ToString().ShouldContain("receipt_medium.jpg");
        result.ContentSrcSmall.ShouldNotBeNull();
        result.ContentSrcSmall.ToString().ShouldContain("receipt_small.jpg");
        result.ExpiresAt.ShouldNotBeNull();
        result.ExpiresAt.Value.ShouldBe(expiresAt);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/attachments/70");
    }
}
