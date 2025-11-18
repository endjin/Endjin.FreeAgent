// <copyright file="RecurringInvoicesTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class RecurringInvoicesTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private RecurringInvoices recurringInvoices = null!;
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
        this.recurringInvoices = new RecurringInvoices(this.freeAgentClient, this.cache);
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.httpClient?.Dispose();
        this.messageHandler?.Dispose();
        this.cache?.Dispose();
    }

    [TestMethod]
    public async Task GetAllAsync_WithoutView_ReturnsActiveRecurringInvoices()
    {
        // Arrange
        List<RecurringInvoice> invoicesList =
        [
            new() { Frequency = "Monthly", RecurringStatus = "Active" },
            new() { Frequency = "Quarterly", RecurringStatus = "Active" }
        ];

        RecurringInvoicesRoot responseRoot = new() { RecurringInvoices = invoicesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<RecurringInvoice> result = await this.recurringInvoices.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);
        result.All(i => i.RecurringStatus == "Active").ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task GetAllAsync_CachesResults()
    {
        // Arrange
        List<RecurringInvoice> invoicesList =
        [
            new() { Frequency = "Monthly", RecurringStatus = "Active" }
        ];

        RecurringInvoicesRoot responseRoot = new() { RecurringInvoices = invoicesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        IEnumerable<RecurringInvoice> result1 = await this.recurringInvoices.GetAllAsync();
        IEnumerable<RecurringInvoice> result2 = await this.recurringInvoices.GetAllAsync();

        // Assert
        result1.Count().ShouldBe(1);
        result2.Count().ShouldBe(1);

        // Mock Verification - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsRecurringInvoice()
    {
        // Arrange
        RecurringInvoice invoice = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/recurring_invoices/789"),
            Frequency = "Quarterly",
            RecurringStatus = "Active",
            RecurringStartsOn = new DateOnly(2024, 1, 1)
        };

        RecurringInvoiceRoot responseRoot = new() { RecurringInvoice = invoice };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        RecurringInvoice result = await this.recurringInvoices.GetByIdAsync("789");

        // Assert
        result.ShouldNotBeNull();
        result.Frequency.ShouldBe("Quarterly");
        result.RecurringStatus.ShouldBe("Active");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task GetByIdAsync_CachesResult()
    {
        // Arrange
        RecurringInvoice invoice = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/recurring_invoices/999"),
            Frequency = "Monthly",
            RecurringStatus = "Active"
        };

        RecurringInvoiceRoot responseRoot = new() { RecurringInvoice = invoice };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        RecurringInvoice result1 = await this.recurringInvoices.GetByIdAsync("999");
        RecurringInvoice result2 = await this.recurringInvoices.GetByIdAsync("999");

        // Assert
        result1.ShouldNotBeNull();
        result2.ShouldNotBeNull();

        // Mock Verification - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task GetAllAsync_WithDraftView_ReturnsDraftRecurringInvoices()
    {
        // Arrange
        List<RecurringInvoice> invoicesList =
        [
            new() { Frequency = "Monthly", RecurringStatus = "Draft" },
            new() { Frequency = "Quarterly", RecurringStatus = "Draft" }
        ];

        RecurringInvoicesRoot responseRoot = new() { RecurringInvoices = invoicesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<RecurringInvoice> result = await this.recurringInvoices.GetAllAsync("draft");

        // Assert
        result.Count().ShouldBe(2);
        result.All(i => i.RecurringStatus == "Draft").ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task GetAllAsync_WithInactiveView_ReturnsInactiveRecurringInvoices()
    {
        // Arrange
        List<RecurringInvoice> invoicesList =
        [
            new() { Frequency = "Monthly", RecurringStatus = "Inactive" },
            new() { Frequency = "Annually", RecurringStatus = "Inactive" }
        ];

        RecurringInvoicesRoot responseRoot = new() { RecurringInvoices = invoicesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<RecurringInvoice> result = await this.recurringInvoices.GetAllAsync("inactive");

        // Assert
        result.Count().ShouldBe(2);
        result.All(i => i.RecurringStatus == "Inactive").ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task GetAllByContactAsync_WithValidContact_ReturnsRecurringInvoicesForContact()
    {
        // Arrange
        Uri contactUri = new("https://api.freeagent.com/v2/contacts/123");

        List<RecurringInvoice> invoicesList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/recurring_invoices/456"),
                Contact = contactUri,
                Frequency = "Monthly",
                RecurringStatus = "Active"
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/recurring_invoices/789"),
                Contact = contactUri,
                Frequency = "Quarterly",
                RecurringStatus = "Active"
            }
        ];

        RecurringInvoicesRoot responseRoot = new() { RecurringInvoices = invoicesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<RecurringInvoice> result = await this.recurringInvoices.GetAllByContactAsync(contactUri);

        // Assert
        result.Count().ShouldBe(2);
        result.All(i => i.Contact == contactUri).ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task GetAllByContactAsync_CachesResults()
    {
        // Arrange
        Uri contactUri = new("https://api.freeagent.com/v2/contacts/123");

        List<RecurringInvoice> invoicesList =
        [
            new() { Contact = contactUri, Frequency = "Monthly", RecurringStatus = "Active" }
        ];

        RecurringInvoicesRoot responseRoot = new() { RecurringInvoices = invoicesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        IEnumerable<RecurringInvoice> result1 = await this.recurringInvoices.GetAllByContactAsync(contactUri);
        IEnumerable<RecurringInvoice> result2 = await this.recurringInvoices.GetAllByContactAsync(contactUri);

        // Assert
        result1.Count().ShouldBe(1);
        result2.Count().ShouldBe(1);

        // Mock Verification - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task GetByIdAsync_WithEmptyId_ThrowsArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(() => this.recurringInvoices.GetByIdAsync(string.Empty));
    }

    [TestMethod]
    public async Task GetByIdAsync_WithNullId_ThrowsArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(() => this.recurringInvoices.GetByIdAsync(null!));
    }

    [TestMethod]
    public async Task GetByIdAsync_WithWhitespaceId_ThrowsArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(() => this.recurringInvoices.GetByIdAsync("   "));
    }

    [TestMethod]
    public async Task GetAllByContactAsync_WithNullUri_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(() => this.recurringInvoices.GetAllByContactAsync(null!));
    }

    [TestMethod]
    public async Task GetByIdAsync_WithAllProperties_ReturnsRecurringInvoiceWithAllProperties()
    {
        // Arrange
        RecurringInvoice invoice = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/recurring_invoices/123"),
            Contact = new Uri("https://api.freeagent.com/v2/contacts/456"),
            Project = new Uri("https://api.freeagent.com/v2/projects/789"),
            ContactName = "Test Contact",
            ClientContactName = "John Smith",
            DatedOn = new DateOnly(2024, 1, 1),
            Frequency = "Monthly",
            RecurringStartsOn = new DateOnly(2024, 1, 1),
            RecurringEndDate = new DateOnly(2025, 12, 31),
            NextRecursOn = new DateOnly(2024, 2, 1),
            ProfileName = "Monthly Retainer",
            Reference = "INV-001",
            PoReference = "PO-12345",
            Currency = "GBP",
            ExchangeRate = 1.0m,
            DiscountPercent = 10.0m,
            NetValue = 1000.00m,
            SalesTaxValue = 200.00m,
            SecondSalesTaxValue = 50.00m,
            TotalValue = 1200.00m,
            InvolvesSalesTax = true,
            RecurringStatus = "Active",
            OmitHeader = false,
            ShowProjectName = true,
            AlwaysShowBicAndIban = true,
            PaymentMethods = new PaymentMethods
            {
                Paypal = true,
                Stripe = true
            },
            PaymentTermsInDays = 30,
            BankAccount = new Uri("https://api.freeagent.com/v2/bank_accounts/111"),
            EcStatus = "UK/Non-EC",
            PlaceOfSupply = "GB",
            CisRate = "standard",
            CisDeductionRate = 20.0m,
            SendNewInvoiceEmails = true,
            SendReminderEmails = true,
            SendThankYouEmails = true,
            InvoiceItems =
            [
                new InvoiceItem
                {
                    Description = "Consulting Services",
                    ItemType = "Hours",
                    Quantity = 10.0m,
                    Price = 100.00m,
                    SalesTaxRate = 20.0m
                }
            ],
            Comments = "Thank you for your business",
            Property = new Uri("https://api.freeagent.com/v2/properties/222"),
            CreatedAt = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2024, 1, 15, 0, 0, 0, TimeSpan.Zero)
        };

        RecurringInvoiceRoot responseRoot = new() { RecurringInvoice = invoice };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        RecurringInvoice result = await this.recurringInvoices.GetByIdAsync("123");

        // Assert - Core properties
        result.ShouldNotBeNull();
        result.Url.ShouldBe(new Uri("https://api.freeagent.com/v2/recurring_invoices/123"));
        result.Contact.ShouldBe(new Uri("https://api.freeagent.com/v2/contacts/456"));
        result.ContactName.ShouldBe("Test Contact");
        result.Frequency.ShouldBe("Monthly");
        result.RecurringStatus.ShouldBe("Active");

        // Assert - New properties
        result.Project.ShouldBe(new Uri("https://api.freeagent.com/v2/projects/789"));
        result.ClientContactName.ShouldBe("John Smith");
        result.PoReference.ShouldBe("PO-12345");
        result.DiscountPercent.ShouldBe(10.0m);
        result.ShowProjectName.ShouldBe(true);
        result.BankAccount.ShouldBe(new Uri("https://api.freeagent.com/v2/bank_accounts/111"));
        result.PlaceOfSupply.ShouldBe("GB");
        result.SendNewInvoiceEmails.ShouldBe(true);
        result.SendReminderEmails.ShouldBe(true);
        result.SendThankYouEmails.ShouldBe(true);
        result.Comments.ShouldBe("Thank you for your business");
        result.Property.ShouldBe(new Uri("https://api.freeagent.com/v2/properties/222"));

        // Assert - Tax-related properties
        result.SecondSalesTaxValue.ShouldBe(50.00m);
        result.InvolvesSalesTax.ShouldBe(true);
        result.CisRate.ShouldBe("standard");
        result.CisDeductionRate.ShouldBe(20.0m);

        // Assert - Invoice items
        result.InvoiceItems.ShouldNotBeNull();
        result.InvoiceItems.Count.ShouldBe(1);
        result.InvoiceItems[0].Description.ShouldBe("Consulting Services");

        // Assert - Payment methods
        result.PaymentMethods.ShouldNotBeNull();
        result.PaymentMethods.Paypal.ShouldBe(true);
        result.PaymentMethods.Stripe.ShouldBe(true);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task GetByIdAsync_WithNotFoundResponse_ThrowsHttpRequestException()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("Not Found", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(() => this.recurringInvoices.GetByIdAsync("999"));
    }

    [TestMethod]
    public async Task GetByIdAsync_WithServerError_ThrowsHttpRequestException()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("Internal Server Error", Encoding.UTF8, "text/plain")
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(() => this.recurringInvoices.GetByIdAsync("123"));
    }

    [TestMethod]
    public async Task GetAllAsync_WithServerError_ThrowsHttpRequestException()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("Internal Server Error", Encoding.UTF8, "text/plain")
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(() => this.recurringInvoices.GetAllAsync());
    }

    [TestMethod]
    public async Task GetAllByContactAsync_WithNotFoundResponse_ThrowsHttpRequestException()
    {
        // Arrange
        Uri contactUri = new("https://api.freeagent.com/v2/contacts/999");
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("Contact not found", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(() => this.recurringInvoices.GetAllByContactAsync(contactUri));
    }

    [TestMethod]
    public async Task GetByIdAsync_WithNullRecurringInvoiceInResponse_ThrowsInvalidOperationException()
    {
        // Arrange
        RecurringInvoiceRoot responseRoot = new() { RecurringInvoice = null };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(() => this.recurringInvoices.GetByIdAsync("123"));
    }

    [TestMethod]
    public async Task GetAllAsync_WithMalformedJson_ThrowsJsonException()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{ invalid json }", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<JsonException>(() => this.recurringInvoices.GetAllAsync());
    }

    [TestMethod]
    public async Task GetByIdAsync_WithMalformedJson_ThrowsJsonException()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{ \"recurring_invoice\": \"not an object\" }", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<JsonException>(() => this.recurringInvoices.GetByIdAsync("123"));
    }

}
