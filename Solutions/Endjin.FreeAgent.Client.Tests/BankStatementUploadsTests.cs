// <copyright file="BankStatementUploadsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class BankStatementUploadsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private BankStatementUploads bankStatementUploads = null!;
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
        this.bankStatementUploads = new BankStatementUploads(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task UploadStatementAsync_WithValidData_ReturnsUploadResponse()
    {
        // Arrange
        Uri bankAccountUrl = new("https://api.freeagent.com/v2/bank_accounts/123");
        string statementData = Convert.ToBase64String(Encoding.UTF8.GetBytes("OFX,DATA,HERE"));
        string fileType = "ofx";

        BankStatementUploadResponse uploadResponse = new()
        {
            ImportedTransactionCount = 15,
            DuplicateTransactionCount = 2
        };

        BankStatementUploadResponseRoot responseRoot = new() { ImportSummary = uploadResponse };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        BankStatementUploadResponse result = await this.bankStatementUploads.UploadStatementAsync(
            bankAccountUrl,
            statementData,
            fileType);

        // Assert
        result.ShouldNotBeNull();
        result.ImportedTransactionCount.ShouldBe(15);
        result.DuplicateTransactionCount.ShouldBe(2);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bank_transactions/statement");
    }
}
