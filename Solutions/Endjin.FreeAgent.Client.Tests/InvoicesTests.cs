// <copyright file="InvoicesTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class InvoicesTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private Invoices invoices = null!;
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

        this.invoices = new Invoices(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task CreateAsync_WithValidInvoice_ReturnsCreatedInvoice()
    {
        // Arrange
        Invoice inputInvoice = new()
        {
            Contact = new Uri("https://api.freeagent.com/v2/contacts/123"),
            DatedOn = new DateOnly(2024, 1, 15),
            DueOn = new DateOnly(2024, 2, 15),
            Reference = "INV-001",
            Currency = "GBP",
            InvoiceItems =
            [
                new InvoiceItem
                {
                    Description = "Consulting Services",
                    ItemType = "Services",
                    Quantity = 10,
                    Price = 150.00m,
                    SalesTaxRate = 20.0m
                }
            ]
        };

        Invoice responseInvoice = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/invoices/456"),
            Contact = new Uri("https://api.freeagent.com/v2/contacts/123"),
            DatedOn = new DateOnly(2024, 1, 15),
            DueOn = new DateOnly(2024, 2, 15),
            Reference = "INV-001",
            Status = "Draft",
            Currency = "GBP",
            NetValue = 1500.00m,
            TotalValue = 1800.00m,
            DueValue = 1800.00m
        };

        InvoiceRoot responseRoot = new() { Invoice = responseInvoice };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Invoice result = await this.invoices.CreateAsync(inputInvoice);

        // Assert
        result.ShouldNotBeNull();
        result.Reference.ShouldBe("INV-001");
        result.Status.ShouldBe("Draft");
        result.TotalValue.ShouldBe(1800.00m);
        result.Url?.ToString().ShouldBe("https://api.freeagent.com/v2/invoices/456");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/invoices");
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllInvoices()
    {
        // Arrange
        List<Invoice> invoicesList =
        [
            new() { Reference = "INV-001", Status = "Draft", TotalValue = 1000.00m },
            new() { Reference = "INV-002", Status = "Sent", TotalValue = 2500.00m },
            new() { Reference = "INV-003", Status = "Paid", TotalValue = 5000.00m }
        ];

        InvoicesRoot responseRoot = new() { Invoices = invoicesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Invoice> result = await this.invoices.GetAllAsync();

        // Assert
        result.Count().ShouldBe(3);
        result.Any(i => i.Reference == "INV-001").ShouldBeTrue();
        result.Any(i => i.Reference == "INV-002").ShouldBeTrue();
        result.Any(i => i.Reference == "INV-003").ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/invoices?view=all");
    }

    [TestMethod]
    public async Task GetAllByStatusAsync_WithDraftStatus_ReturnsOnlyDraftInvoices()
    {
        // Arrange
        List<Invoice> draftInvoices =
        [
            new() { Reference = "INV-001", Status = "Draft", TotalValue = 1000.00m },
            new() { Reference = "INV-004", Status = "Draft", TotalValue = 750.00m }
        ];

        InvoicesRoot responseRoot = new() { Invoices = draftInvoices };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Invoice> result = await this.invoices.GetAllByStatusAsync("draft");

        // Assert
        result.Count().ShouldBe(2);
        result.All(i => i.Status == "Draft").ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/invoices?view=draft");
    }

    [TestMethod]
    public async Task GetAllByContactAsync_ReturnsInvoicesForSpecificContact()
    {
        // Arrange
        Uri contactUri = new("https://api.freeagent.com/v2/contacts/123");
        List<Invoice> contactInvoices =
        [
            new() { Reference = "INV-001", Contact = contactUri, TotalValue = 1000.00m },
            new() { Reference = "INV-002", Contact = contactUri, TotalValue = 2000.00m }
        ];

        InvoicesRoot responseRoot = new() { Invoices = contactInvoices };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Invoice> result = await this.invoices.GetAllByContactAsync(contactUri);

        // Assert
        result.Count().ShouldBe(2);
        result.All(i => i.Contact == contactUri).ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsInvoice()
    {
        // Arrange
        Invoice invoice = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/invoices/456"),
            Reference = "INV-001",
            Status = "Sent",
            TotalValue = 1800.00m
        };

        InvoiceRoot responseRoot = new() { Invoice = invoice };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Invoice result = await this.invoices.GetByIdAsync("456");

        // Assert
        result.ShouldNotBeNull();
        result.Reference.ShouldBe("INV-001");
        result.Status.ShouldBe("Sent");
        result.TotalValue.ShouldBe(1800.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/invoices/456");
    }

    [TestMethod]
    public async Task UpdateAsync_WithValidChanges_ReturnsUpdatedInvoice()
    {
        // Arrange
        Invoice updateInvoice = new()
        {
            Reference = "INV-001-UPDATED",
            Comments = "Updated comments"
        };

        Invoice responseInvoice = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/invoices/456"),
            Reference = "INV-001-UPDATED",
            Comments = "Updated comments",
            Status = "Draft"
        };

        InvoiceRoot responseRoot = new() { Invoice = responseInvoice };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Invoice result = await this.invoices.UpdateAsync("456", updateInvoice);

        // Assert
        result.ShouldNotBeNull();
        result.Reference.ShouldBe("INV-001-UPDATED");
        result.Comments.ShouldBe("Updated comments");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/invoices/456");
    }

    [TestMethod]
    public async Task DeleteAsync_WithValidId_DeletesSuccessfully()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        await this.invoices.DeleteAsync("456");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/invoices/456");
    }

    [TestMethod]
    public async Task MarkAsSentAsync_WithValidId_UpdatesInvoiceStatus()
    {
        // Arrange
        Invoice responseInvoice = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/invoices/456"),
            Reference = "INV-001",
            Status = "Sent",
            SentAt = DateTime.UtcNow
        };

        InvoiceRoot responseRoot = new() { Invoice = responseInvoice };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Invoice result = await this.invoices.MarkAsSentAsync("456");

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe("Sent");
        result.SentAt.ShouldNotBeNull();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/invoices/456/transitions/mark_as_sent");
    }

    [TestMethod]
    public async Task SendEmailAsync_WithValidEmailDetails_SendsInvoiceEmail()
    {
        // Arrange
        InvoiceEmail email = new()
        {
            To = "client@example.com",
            Subject = "Invoice INV-001",
            Body = "Please find attached invoice.",
            SendPdfAttachment = true
        };

        Invoice responseInvoice = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/invoices/456"),
            Reference = "INV-001",
            Status = "Sent",
            SentAt = DateTime.UtcNow
        };

        InvoiceRoot responseRoot = new() { Invoice = responseInvoice };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Invoice result = await this.invoices.SendEmailAsync("456", email);

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe("Sent");
        result.SentAt.ShouldNotBeNull();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/invoices/456/send_email");
    }

    [TestMethod]
    public async Task MarkAsCancelledAsync_WithValidId_CancelsInvoice()
    {
        // Arrange
        Invoice responseInvoice = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/invoices/456"),
            Reference = "INV-001",
            Status = "Cancelled"
        };

        InvoiceRoot responseRoot = new() { Invoice = responseInvoice };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Invoice result = await this.invoices.MarkAsCancelledAsync("456");

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe("Cancelled");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/invoices/456/transitions/mark_as_cancelled");
    }

    [TestMethod]
    public async Task GetPdfAsync_WithValidId_ReturnsPdfBytes()
    {
        // Arrange
        byte[] pdfContent = Encoding.UTF8.GetBytes("PDF_CONTENT_HERE");

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(pdfContent)
        };

        // Act
        byte[] result = await this.invoices.GetPdfAsync("456");

        // Assert
        result.ShouldNotBeNull();
        result.Length.ShouldBe(pdfContent.Length);
        result.SequenceEqual(pdfContent).ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/invoices/456/pdf");
    }

    [TestMethod]
    public async Task GetAllAsync_WithCachedData_ReturnsCachedInvoices()
    {
        // Arrange - First call
        List<Invoice> invoicesList =
        [
            new() { Reference = "INV-001", Status = "Draft" }
        ];

        InvoicesRoot responseRoot = new() { Invoices = invoicesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - First call (populates cache)
        await this.invoices.GetAllAsync();

        // Act - Second call (should use cache)
        IEnumerable<Invoice> result = await this.invoices.GetAllAsync();

        // Assert
        result.Count().ShouldBe(1);
        result.First().Reference.ShouldBe("INV-001");

        // Mock Verification - Should only be called once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task GetAllByProjectAsync_ReturnsInvoicesForSpecificProject()
    {
        // Arrange
        Uri projectUri = new("https://api.freeagent.com/v2/projects/333");
        List<Invoice> projectInvoices =
        [
            new() { Reference = "INV-001", Project = projectUri, TotalValue = 1000.00m },
            new() { Reference = "INV-002", Project = projectUri, TotalValue = 2000.00m }
        ];

        InvoicesRoot responseRoot = new() { Invoices = projectInvoices };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Invoice> result = await this.invoices.GetAllByProjectAsync(projectUri);

        // Assert
        result.Count().ShouldBe(2);
        result.All(i => i.Project == projectUri).ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task MarkAsScheduledAsync_WithValidId_SchedulesInvoice()
    {
        // Arrange
        Invoice responseInvoice = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/invoices/456"),
            Reference = "INV-001",
            Status = "Scheduled"
        };

        InvoiceRoot responseRoot = new() { Invoice = responseInvoice };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Invoice result = await this.invoices.MarkAsScheduledAsync("456");

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe("Scheduled");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/invoices/456/transitions/mark_as_scheduled");
    }

    [TestMethod]
    public async Task MarkAsDraftAsync_WithValidId_ReturnsInvoiceToDraft()
    {
        // Arrange
        Invoice responseInvoice = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/invoices/456"),
            Reference = "INV-001",
            Status = "Draft"
        };

        InvoiceRoot responseRoot = new() { Invoice = responseInvoice };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Invoice result = await this.invoices.MarkAsDraftAsync("456");

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe("Draft");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/invoices/456/transitions/mark_as_draft");
    }

    [TestMethod]
    public async Task DuplicateAsync_WithValidId_ReturnsDuplicatedInvoice()
    {
        // Arrange
        Invoice responseInvoice = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/invoices/789"),
            Reference = "INV-002",
            Status = "Draft",
            Contact = new Uri("https://api.freeagent.com/v2/contacts/123"),
            TotalValue = 1500.00m
        };

        InvoiceRoot responseRoot = new() { Invoice = responseInvoice };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Invoice result = await this.invoices.DuplicateAsync("456");

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe("Draft");
        result.TotalValue.ShouldBe(1500.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/invoices/456/duplicate");
    }
}
