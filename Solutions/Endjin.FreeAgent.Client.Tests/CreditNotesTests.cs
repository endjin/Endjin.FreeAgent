// <copyright file="CreditNotesTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class CreditNotesTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private CreditNotes creditNotes = null!;
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
        this.creditNotes = new CreditNotes(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task CreateAsync_WithValidCreditNote_ReturnsCreatedCreditNote()
    {
        // Arrange
        CreditNote inputCreditNote = new()
        {
            Contact = new Uri("https://api.freeagent.com/v2/contacts/123"),
            DatedOn = new DateOnly(2024, 1, 20),
            Reference = "CN-001",
            Reason = "Items returned"
        };

        CreditNote responseCreditNote = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/credit_notes/456"),
            Reference = "CN-001",
            Status = "Draft",
            TotalValue = 240.00m
        };

        CreditNoteRoot responseRoot = new() { CreditNote = responseCreditNote };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        CreditNote result = await this.creditNotes.CreateAsync(inputCreditNote);

        // Assert
        result.ShouldNotBeNull();
        result.Reference.ShouldBe("CN-001");
        result.Status.ShouldBe("Draft");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/credit_notes");
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllCreditNotes()
    {
        // Arrange
        List<CreditNote> notesList =
        [
            new() { Reference = "CN-001", Status = "Draft" },
            new() { Reference = "CN-002", Status = "Sent" }
        ];

        CreditNotesRoot responseRoot = new() { CreditNotes = notesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<CreditNote> result = await this.creditNotes.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);
        result.Any(cn => cn.Reference == "CN-001").ShouldBeTrue();
        result.Any(cn => cn.Reference == "CN-002").ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task MarkAsRefundedAsync_WithValidDetails_UpdatesCreditNoteAsRefunded()
    {
        // Arrange
        DateOnly refundedOn = new(2024, 1, 25);
        Uri bankAccountUri = new("https://api.freeagent.com/v2/bank_accounts/123");

        CreditNote responseCreditNote = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/credit_notes/456"),
            Reference = "CN-001",
            Status = "Refunded",
            RefundedOn = refundedOn
        };

        CreditNoteRoot responseRoot = new() { CreditNote = responseCreditNote };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        CreditNote result = await this.creditNotes.MarkAsRefundedAsync("456", refundedOn, bankAccountUri);

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe("Refunded");
        result.RefundedOn.ShouldBe(refundedOn);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/credit_notes/456/mark_as_refunded");
    }

    [TestMethod]
    public async Task SendEmailAsync_WithValidEmail_SendsCreditNoteEmail()
    {
        // Arrange
        InvoiceEmail email = new()
        {
            To = "client@example.com",
            Subject = "Credit Note CN-001",
            Body = "Please find attached credit note."
        };

        CreditNote responseCreditNote = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/credit_notes/456"),
            Reference = "CN-001",
            Status = "Sent",
            SentAt = DateTime.UtcNow
        };

        CreditNoteRoot responseRoot = new() { CreditNote = responseCreditNote };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        CreditNote result = await this.creditNotes.SendEmailAsync("456", email);

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe("Sent");
        result.SentAt.ShouldNotBeNull();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/credit_notes/456/send_email");
    }

    [TestMethod]
    public async Task DeleteAsync_WithValidId_DeletesSuccessfully()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        await this.creditNotes.DeleteAsync("456");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/credit_notes/456");
    }
}
