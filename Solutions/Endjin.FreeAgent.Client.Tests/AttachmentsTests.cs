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
    public async Task UploadAsync_WithByteArray_ReturnsCreatedAttachment()
    {
        // Arrange
        byte[] fileData = [0x25, 0x50, 0x44, 0x46]; // PDF file signature
        string fileName = "test.pdf";
        string contentType = "application/pdf";
        string description = "Test PDF file";

        Attachment responseAttachment = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/attachments/123"),
            Filename = fileName,
            Size = fileData.Length,
            ContentType = contentType,
            Description = description,
            CreatedAt = new DateTime(2024, 3, 15, 10, 0, 0)
        };

        AttachmentRoot responseRoot = new() { Attachment = responseAttachment };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Attachment result = await this.attachments.UploadAsync(fileData, fileName, contentType, description);

        // Assert
        result.ShouldNotBeNull();
        result.Url.ShouldNotBeNull();
        result.Filename.ShouldBe(fileName);
        result.Size.ShouldBe(fileData.Length);
        result.ContentType.ShouldBe(contentType);
        result.Description.ShouldBe(description);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/attachments");
    }

    [TestMethod]
    public async Task UploadAsync_WithByteArrayNoDescription_ReturnsCreatedAttachment()
    {
        // Arrange
        byte[] fileData = [0xFF, 0xD8, 0xFF, 0xE0]; // JPEG file signature
        string fileName = "photo.jpg";
        string contentType = "image/jpeg";

        Attachment responseAttachment = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/attachments/456"),
            Filename = fileName,
            Size = fileData.Length,
            ContentType = contentType,
            CreatedAt = new DateTime(2024, 3, 15, 11, 0, 0)
        };

        AttachmentRoot responseRoot = new() { Attachment = responseAttachment };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Attachment result = await this.attachments.UploadAsync(fileData, fileName, contentType);

        // Assert
        result.ShouldNotBeNull();
        result.Url.ShouldNotBeNull();
        result.Filename.ShouldBe(fileName);
        result.ContentType.ShouldBe(contentType);
        result.Description.ShouldBeNull();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
    }

    [TestMethod]
    public async Task UploadAsync_WithStream_ReturnsCreatedAttachment()
    {
        // Arrange
        byte[] fileData = [0x89, 0x50, 0x4E, 0x47]; // PNG file signature
        using MemoryStream stream = new(fileData);
        string fileName = "image.png";
        string contentType = "image/png";

        Attachment responseAttachment = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/attachments/789"),
            Filename = fileName,
            Size = fileData.Length,
            ContentType = contentType
        };

        AttachmentRoot responseRoot = new() { Attachment = responseAttachment };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Attachment result = await this.attachments.UploadAsync(stream, fileName, contentType);

        // Assert
        result.ShouldNotBeNull();
        result.Filename.ShouldBe(fileName);
        result.Size.ShouldBe(fileData.Length);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
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
            CreatedAt = new DateTime(2024, 3, 15, 10, 30, 0)
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
    public async Task DownloadAsync_WithValidId_ReturnsFileData()
    {
        // Arrange
        byte[] expectedData = [0x25, 0x50, 0x44, 0x46, 0x2D, 0x31, 0x2E, 0x34]; // PDF header

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(expectedData)
        };

        // Act
        byte[] result = await this.attachments.DownloadAsync("40");

        // Assert
        result.ShouldNotBeNull();
        result.Length.ShouldBe(expectedData.Length);
        result.ShouldBe(expectedData);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/attachments/40/download");
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
}
