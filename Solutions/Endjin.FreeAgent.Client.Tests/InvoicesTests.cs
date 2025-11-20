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
            new() { Reference = "INV-002", Status = "Open", TotalValue = 2500.00m },
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
            Status = "Open",
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
        result.Status.ShouldBe("Open");
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
            Status = "Open",
            SentAt = DateTimeOffset.UtcNow
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
        result.Status.ShouldBe("Open");
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
            Body = "Please find attached invoice."
        };

        Invoice responseInvoice = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/invoices/456"),
            Reference = "INV-001",
            Status = "Open",
            SentAt = DateTimeOffset.UtcNow
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
        result.Status.ShouldBe("Open");
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
            Status = "Draft"
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
        result.Status.ShouldBe("Draft");

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
        string base64Content = Convert.ToBase64String(pdfContent);

        InvoicePdfRoot pdfRoot = new()
        {
            Pdf = new InvoicePdf { Content = base64Content }
        };
        string responseJson = JsonSerializer.Serialize(pdfRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
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
            Status = "Scheduled To Email"
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
        result.Status.ShouldBe("Scheduled To Email");

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

    [TestMethod]
    public async Task ConvertToCreditNoteAsync_WithValidId_ConvertsInvoiceToCreditNote()
    {
        // Arrange
        Invoice creditNoteInvoice = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/invoices/456"),
            Reference = "CN-001",
            Status = "Draft",
            TotalValue = -1000.00m
        };

        InvoiceRoot responseRoot = new() { Invoice = creditNoteInvoice };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Invoice result = await this.invoices.ConvertToCreditNoteAsync("456");

        // Assert
        result.ShouldNotBeNull();
        result.Reference.ShouldBe("CN-001");
        result.Status.ShouldBe("Draft");
        result.TotalValue.ShouldBe(-1000.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/invoices/456/transitions/convert_to_credit_note");
    }

    [TestMethod]
    public async Task InitiateDirectDebitAsync_WithValidId_InitiatesDirectDebit()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        await this.invoices.InitiateDirectDebitAsync("456");

        // Assert - No exception thrown means success

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/invoices/456/direct_debit");
    }

    [TestMethod]
    public async Task GetTimelineAsync_ReturnsInvoiceTimelineEntries()
    {
        // Arrange
        List<InvoiceTimelineEntry> timelineEntries =
        [
            new()
            {
                Reference = "INV-001",
                Summary = "Invoice created",
                Description = "Invoice INV-001 was created",
                DatedOn = new DateOnly(2024, 1, 15),
                Amount = 1000.00m
            },
            new()
            {
                Reference = "INV-001",
                Summary = "Invoice sent",
                Description = "Invoice INV-001 was sent to customer",
                DatedOn = new DateOnly(2024, 1, 16),
                Amount = 1000.00m
            }
        ];

        InvoiceTimelineRoot responseRoot = new() { TimelineEntries = timelineEntries };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<InvoiceTimelineEntry> result = await this.invoices.GetTimelineAsync();

        // Assert
        result.Count().ShouldBe(2);
        InvoiceTimelineEntry firstEntry = result.First();
        firstEntry.Reference.ShouldBe("INV-001");
        firstEntry.Summary.ShouldBe("Invoice created");
        firstEntry.DatedOn.ShouldBe(new DateOnly(2024, 1, 15));
        firstEntry.Amount.ShouldBe(1000.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/invoices/timeline");
    }

    [TestMethod]
    public async Task GetAllAsync_WithUpdatedSince_IncludesParameterInRequest()
    {
        // Arrange
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

        DateTimeOffset updatedSince = new(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);

        // Act
        IEnumerable<Invoice> result = await this.invoices.GetAllAsync(updatedSince: updatedSince);

        // Assert
        result.Count().ShouldBe(1);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.LastRequest?.RequestUri?.Query.ShouldContain("updated_since=");
    }

    [TestMethod]
    public async Task GetAllAsync_WithSortParameter_IncludesParameterInRequest()
    {
        // Arrange
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

        // Act
        IEnumerable<Invoice> result = await this.invoices.GetAllAsync(sort: "-updated_at");

        // Assert
        result.Count().ShouldBe(1);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.LastRequest?.RequestUri?.Query.ShouldContain("sort=-updated_at");
    }

    [TestMethod]
    public async Task GetAllAsync_WithNestedInvoiceItems_IncludesParameterInRequest()
    {
        // Arrange
        List<Invoice> invoicesList =
        [
            new()
            {
                Reference = "INV-001",
                Status = "Draft",
                InvoiceItems =
                [
                    new InvoiceItem { Description = "Item 1", Price = 100.00m }
                ]
            }
        ];

        InvoicesRoot responseRoot = new() { Invoices = invoicesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Invoice> result = await this.invoices.GetAllAsync(nestedInvoiceItems: true);

        // Assert
        result.Count().ShouldBe(1);
        result.First().InvoiceItems.ShouldNotBeNull();
        result.First().InvoiceItems!.Count.ShouldBe(1);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.LastRequest?.RequestUri?.Query.ShouldContain("nested_invoice_items=true");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithNotFoundResponse_ThrowsHttpRequestException()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("{\"error\": \"Invoice not found\"}", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(async () => await this.invoices.GetByIdAsync("999"));
    }

    [TestMethod]
    public async Task CreateAsync_WithServerError_ThrowsHttpRequestException()
    {
        // Arrange
        Invoice inputInvoice = new()
        {
            Contact = new Uri("https://api.freeagent.com/v2/contacts/123"),
            DatedOn = new DateOnly(2024, 1, 15)
        };

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("{\"error\": \"Internal server error\"}", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(async () => await this.invoices.CreateAsync(inputInvoice));
    }

    [TestMethod]
    public async Task UpdateAsync_InvalidatesCache_AfterSuccessfulUpdate()
    {
        // Arrange - First, populate cache by getting the invoice
        Invoice cachedInvoice = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/invoices/456"),
            Reference = "INV-001",
            Status = "Draft"
        };

        InvoiceRoot cachedRoot = new() { Invoice = cachedInvoice };
        string cachedJson = JsonSerializer.Serialize(cachedRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(cachedJson, Encoding.UTF8, "application/json")
        };

        await this.invoices.GetByIdAsync("456");

        // Prepare update response
        Invoice updatedInvoice = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/invoices/456"),
            Reference = "INV-001-UPDATED",
            Status = "Draft"
        };

        InvoiceRoot updatedRoot = new() { Invoice = updatedInvoice };
        string updatedJson = JsonSerializer.Serialize(updatedRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(updatedJson, Encoding.UTF8, "application/json")
        };

        // Act
        Invoice updateInput = new() { Reference = "INV-001-UPDATED" };
        await this.invoices.UpdateAsync("456", updateInput);

        // Now get again to verify cache was invalidated
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(updatedJson, Encoding.UTF8, "application/json")
        };

        Invoice result = await this.invoices.GetByIdAsync("456");

        // Assert
        result.Reference.ShouldBe("INV-001-UPDATED");

        // Mock Verification - Should have 3 calls: initial get, update, get after cache invalidation
        this.messageHandler.CallCount.ShouldBe(3);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        this.httpClient?.Dispose();
        this.messageHandler?.Dispose();
    }
}
