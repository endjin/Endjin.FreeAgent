// <copyright file="ExpensesTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class ExpensesTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private Expenses expenses = null!;
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

        // Use proper initialization with TestOAuth2Service
        await TestHelper.SetupForTestingAsync(this.freeAgentClient, this.httpClientFactory);
        this.expenses = new Expenses(this.freeAgentClient);
    }

    [TestMethod]
    public async Task CreateAsync_WithValidExpense_ReturnsCreatedExpense()
    {
        // Arrange
        Expense inputExpense = new()
        {
            DatedOn = "2024-01-15",
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "GBP",
            GrossValue = 100.00,
            Description = "Client lunch meeting",
            User = "https://api.freeagent.com/v2/users/1"
        };

        Expense responseExpense = new()
        {
            DatedOn = "2024-01-15",
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "GBP",
            GrossValue = 100.00,
            Description = "Client lunch meeting",
            User = "https://api.freeagent.com/v2/users/1",
            Url = "https://api.freeagent.com/v2/expenses/12345"
        };

        ExpenseRoot responseRoot = new() { Expense = responseExpense };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Expense result = await this.expenses.CreateAsync(inputExpense);

        // Assert
        result.ShouldNotBeNull();
        result.DatedOn.ShouldBe("2024-01-15");
        result.Category.ShouldBe("https://api.freeagent.com/v2/categories/285");
        result.Currency.ShouldBe("GBP");
        result.GrossValue.ShouldBe(100.00);
        result.Description.ShouldBe("Client lunch meeting");
        result.Url.ShouldBe("https://api.freeagent.com/v2/expenses/12345");
    }

    [TestMethod]
    public async Task CreateAsync_WithAttachment_IncludesAttachmentData()
    {
        // Arrange
        ExpenseAttachment attachment = new()
        {
            Data = "base64encodeddata==",
            FileName = "receipt.pdf",
            ContentType = "application/pdf",
            Description = "Receipt for lunch"
        };

        Expense inputExpense = new()
        {
            DatedOn = "2024-01-15",
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "GBP",
            GrossValue = 50.00,
            Description = "Office supplies",
            User = "https://api.freeagent.com/v2/users/1",
            Attachment = attachment
        };

        Expense responseExpense = new()
        {
            DatedOn = "2024-01-15",
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "GBP",
            GrossValue = 50.00,
            Description = "Office supplies",
            User = "https://api.freeagent.com/v2/users/1",
            Attachment = attachment,
            Url = "https://api.freeagent.com/v2/expenses/12346"
        };

        ExpenseRoot responseRoot = new() { Expense = responseExpense };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Expense result = await this.expenses.CreateAsync(inputExpense);

        // Assert
        result.ShouldNotBeNull();
        result.Attachment.ShouldNotBeNull();
        result.Attachment!.FileName.ShouldBe("receipt.pdf");
        result.Attachment.ContentType.ShouldBe("application/pdf");
        result.Attachment.Description.ShouldBe("Receipt for lunch");
    }

    [TestMethod]
    public async Task CreateAsync_WithInvalidData_ThrowsHttpRequestException()
    {
        // Arrange
        Expense inputExpense = new()
        {
            // Missing required fields
            Description = "Invalid expense"
        };

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Validation failed", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(async () =>
            await this.expenses.CreateAsync(inputExpense));
    }

    [TestMethod]
    public async Task CreateAsync_WithMultipleCurrencies_HandlesCorrectly()
    {
        // Arrange
        Expense inputExpense = new()
        {
            DatedOn = "2024-01-15",
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "USD",
            GrossValue = 150.00,
            Description = "International conference",
            User = "https://api.freeagent.com/v2/users/1"
        };

        Expense responseExpense = new()
        {
            DatedOn = "2024-01-15",
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "USD",
            GrossValue = 150.00,
            Description = "International conference",
            User = "https://api.freeagent.com/v2/users/1",
            Url = "https://api.freeagent.com/v2/expenses/12347"
        };

        ExpenseRoot responseRoot = new() { Expense = responseExpense };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Expense result = await this.expenses.CreateAsync(inputExpense);

        // Assert
        result.ShouldNotBeNull();
        result.Currency.ShouldBe("USD");
        result.GrossValue.ShouldBe(150.00);
    }

    [TestMethod]
    public async Task CreateAsync_WithProject_AssociatesExpenseToProject()
    {
        // Arrange
        Expense inputExpense = new()
        {
            DatedOn = "2024-01-15",
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "GBP",
            GrossValue = 75.00,
            Description = "Project materials",
            User = "https://api.freeagent.com/v2/users/1",
            Project = "https://api.freeagent.com/v2/projects/5678"
        };

        Expense responseExpense = new()
        {
            DatedOn = "2024-01-15",
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "GBP",
            GrossValue = 75.00,
            Description = "Project materials",
            User = "https://api.freeagent.com/v2/users/1",
            Project = "https://api.freeagent.com/v2/projects/5678",
            Url = "https://api.freeagent.com/v2/expenses/12348"
        };

        ExpenseRoot responseRoot = new() { Expense = responseExpense };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Expense result = await this.expenses.CreateAsync(inputExpense);

        // Assert
        result.ShouldNotBeNull();
        result.Project.ShouldBe("https://api.freeagent.com/v2/projects/5678");
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.cache?.Dispose();
        this.httpClient?.Dispose();
        this.messageHandler?.Dispose();
    }

}
