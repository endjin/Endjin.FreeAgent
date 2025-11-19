// <copyright file="BillsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class BillsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private Bills bills = null!;
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
        this.bills = new Bills(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task CreateAsync_WithValidBill_ReturnsCreatedBill()
    {
        // Arrange
        Bill inputBill = new()
        {
            Contact = new Uri("https://api.freeagent.com/v2/contacts/789"),
            DatedOn = new DateOnly(2024, 1, 10),
            DueOn = new DateOnly(2024, 2, 10),
            Reference = "BILL-001",
            TotalValue = 600.00m
        };

        Bill responseBill = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/bills/456"),
            Contact = new Uri("https://api.freeagent.com/v2/contacts/789"),
            Reference = "BILL-001",
            Status = "Open",
            TotalValue = 600.00m
        };

        BillRoot responseRoot = new() { Bill = responseBill };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Bill result = await this.bills.CreateAsync(inputBill);

        // Assert
        result.ShouldNotBeNull();
        result.Reference.ShouldBe("BILL-001");
        result.Status.ShouldBe("Open");
        result.TotalValue.ShouldBe(600.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bills");
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllBills()
    {
        // Arrange
        List<Bill> billsList =
        [
            new() { Reference = "BILL-001", Status = "Open", TotalValue = 500.00m },
            new() { Reference = "BILL-002", Status = "Paid", TotalValue = 750.00m }
        ];

        BillsRoot responseRoot = new() { Bills = billsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Bill> result = await this.bills.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);
        result.Any(b => b.Reference == "BILL-001").ShouldBeTrue();
        result.Any(b => b.Reference == "BILL-002").ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bills?view=all");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsBill()
    {
        // Arrange
        Bill bill = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/bills/456"),
            Reference = "BILL-001",
            Status = "Open",
            TotalValue = 600.00m
        };

        BillRoot responseRoot = new() { Bill = bill };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Bill result = await this.bills.GetByIdAsync("456");

        // Assert
        result.ShouldNotBeNull();
        result.Reference.ShouldBe("BILL-001");
        result.Status.ShouldBe("Open");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bills/456");
    }

    [TestMethod]
    public async Task UpdateAsync_WithValidChanges_ReturnsUpdatedBill()
    {
        // Arrange
        Bill updateBill = new()
        {
            Reference = "BILL-001-UPDATED"
        };

        Bill responseBill = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/bills/456"),
            Reference = "BILL-001-UPDATED",
            Status = "Open"
        };

        BillRoot responseRoot = new() { Bill = responseBill };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Bill result = await this.bills.UpdateAsync("456", updateBill);

        // Assert
        result.ShouldNotBeNull();
        result.Reference.ShouldBe("BILL-001-UPDATED");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bills/456");
    }

    [TestMethod]
    public async Task DeleteAsync_WithValidId_DeletesSuccessfully()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK);

        // Act
        await this.bills.DeleteAsync("456");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bills/456");
    }

    [TestMethod]
    public async Task GetAllByContactAsync_WithValidContactUri_ReturnsBillsForContact()
    {
        // Arrange
        Uri contactUri = new("https://api.freeagent.com/v2/contacts/789");
        List<Bill> billsList =
        [
            new() { Reference = "BILL-001", Status = "Open", TotalValue = 500.00m, Contact = contactUri },
            new() { Reference = "BILL-002", Status = "Paid", TotalValue = 750.00m, Contact = contactUri }
        ];

        BillsRoot responseRoot = new() { Bills = billsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Bill> result = await this.bills.GetAllByContactAsync(contactUri);

        // Assert
        result.Count().ShouldBe(2);
        result.All(b => b.Contact == contactUri).ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bills?contact=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Fcontacts%2F789");
    }

    [TestMethod]
    public async Task GetAllByProjectAsync_WithValidProjectUri_ReturnsBillsForProject()
    {
        // Arrange
        Uri projectUri = new("https://api.freeagent.com/v2/projects/123");
        List<Bill> billsList =
        [
            new() { Reference = "BILL-003", Status = "Open", TotalValue = 1200.00m, Project = projectUri },
            new() { Reference = "BILL-004", Status = "Open", TotalValue = 850.00m, Project = projectUri }
        ];

        BillsRoot responseRoot = new() { Bills = billsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Bill> result = await this.bills.GetAllByProjectAsync(projectUri);

        // Assert
        result.Count().ShouldBe(2);
        result.All(b => b.Project == projectUri).ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bills?project=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Fprojects%2F123");
    }

    [TestMethod]
    public async Task GetAllAsync_WithViewFilter_UsesCorrectViewParameter()
    {
        // Arrange
        List<Bill> billsList =
        [
            new() { Reference = "BILL-001", Status = "Open", TotalValue = 500.00m }
        ];

        BillsRoot responseRoot = new() { Bills = billsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Bill> result = await this.bills.GetAllAsync(view: "open");

        // Assert
        result.Count().ShouldBe(1);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bills?view=open");
    }

    [TestMethod]
    public async Task GetAllAsync_WithDateRangeFilters_UsesCorrectDateParameters()
    {
        // Arrange
        DateOnly fromDate = new(2024, 1, 1);
        DateOnly toDate = new(2024, 3, 31);

        List<Bill> billsList =
        [
            new() { Reference = "BILL-001", Status = "Open", TotalValue = 500.00m, DatedOn = new DateOnly(2024, 2, 15) }
        ];

        BillsRoot responseRoot = new() { Bills = billsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Bill> result = await this.bills.GetAllAsync(fromDate: fromDate, toDate: toDate);

        // Assert
        result.Count().ShouldBe(1);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bills?view=all&from_date=2024-01-01&to_date=2024-03-31");
    }

    [TestMethod]
    public async Task GetAllAsync_WithUpdatedSinceFilter_UsesCorrectDateTimeParameter()
    {
        // Arrange
        DateTime updatedSince = new(2024, 3, 1, 10, 30, 0, DateTimeKind.Utc);

        List<Bill> billsList =
        [
            new() { Reference = "BILL-001", Status = "Open", TotalValue = 500.00m }
        ];

        BillsRoot responseRoot = new() { Bills = billsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Bill> result = await this.bills.GetAllAsync(updatedSince: updatedSince);

        // Assert
        result.Count().ShouldBe(1);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bills?view=all&updated_since=2024-03-01T10%3A30%3A00.000Z");
    }

    [TestMethod]
    public async Task GetAllAsync_WithNestedBillItems_UsesCorrectParameter()
    {
        // Arrange
        List<Bill> billsList =
        [
            new()
            {
                Reference = "BILL-001",
                Status = "Open",
                TotalValue = 500.00m,
                BillItems =
                [
                    new BillItem
                    {
                        Description = "Item 1",
                        TotalValue = 250.00m
                    },
                    new BillItem
                    {
                        Description = "Item 2",
                        TotalValue = 250.00m
                    }
                ]
            }
        ];

        BillsRoot responseRoot = new() { Bills = billsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Bill> result = await this.bills.GetAllAsync(nestedBillItems: true);

        // Assert
        result.Count().ShouldBe(1);
        result.First().BillItems.Count().ShouldBe(2);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bills?view=all&nested_bill_items=true");
    }

    [TestMethod]
    public async Task GetAllAsync_WithMultipleFilters_CombinesAllParameters()
    {
        // Arrange
        DateOnly fromDate = new(2024, 1, 1);
        DateOnly toDate = new(2024, 3, 31);

        List<Bill> billsList =
        [
            new() { Reference = "BILL-001", Status = "Open", TotalValue = 500.00m }
        ];

        BillsRoot responseRoot = new() { Bills = billsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Bill> result = await this.bills.GetAllAsync(
            view: "open",
            nestedBillItems: true,
            fromDate: fromDate,
            toDate: toDate);

        // Assert
        result.Count().ShouldBe(1);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bills?view=open&nested_bill_items=true&from_date=2024-01-01&to_date=2024-03-31");
    }

    [TestMethod]
    public async Task CreateAsync_WithAttachment_SerializesAttachmentCorrectly()
    {
        // Arrange
        Bill inputBill = new()
        {
            Contact = new Uri("https://api.freeagent.com/v2/contacts/789"),
            DatedOn = new DateOnly(2024, 1, 10),
            DueOn = new DateOnly(2024, 2, 10),
            Reference = "BILL-001",
            TotalValue = 600.00m,
            Attachment = new Attachment
            {
                Data = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==",
                Filename = "receipt.png",
                ContentType = "image/png",
                Description = "Receipt for purchase"
            }
        };

        Bill responseBill = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/bills/456"),
            Contact = new Uri("https://api.freeagent.com/v2/contacts/789"),
            Reference = "BILL-001",
            Status = "Open",
            TotalValue = 600.00m
        };

        BillRoot responseRoot = new() { Bill = responseBill };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Bill result = await this.bills.CreateAsync(inputBill);

        // Assert
        result.ShouldNotBeNull();
        result.Reference.ShouldBe("BILL-001");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bills");
    }

    [TestMethod]
    public async Task CreateAsync_WithCisDeduction_SerializesCisFieldsCorrectly()
    {
        // Arrange
        Bill inputBill = new()
        {
            Contact = new Uri("https://api.freeagent.com/v2/contacts/789"),
            DatedOn = new DateOnly(2024, 1, 10),
            DueOn = new DateOnly(2024, 2, 10),
            Reference = "BILL-CIS-001",
            TotalValue = 1000.00m,
            CisDeductionBand = "standard",
            CisDeductionRate = 20.0m
        };

        Bill responseBill = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/bills/456"),
            Contact = new Uri("https://api.freeagent.com/v2/contacts/789"),
            Reference = "BILL-CIS-001",
            Status = "Open",
            TotalValue = 1000.00m,
            CisDeductionBand = "standard",
            CisDeductionRate = 20.0m,
            CisDeduction = 200.00m
        };

        BillRoot responseRoot = new() { Bill = responseBill };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Bill result = await this.bills.CreateAsync(inputBill);

        // Assert
        result.ShouldNotBeNull();
        result.Reference.ShouldBe("BILL-CIS-001");
        result.CisDeductionBand.ShouldBe("standard");
        result.CisDeductionRate.ShouldBe(20.0m);
        result.CisDeduction.ShouldBe(200.00m);
    }

    [TestMethod]
    public async Task CreateAsync_WithMultiCurrency_SerializesCurrencyFieldsCorrectly()
    {
        // Arrange
        Bill inputBill = new()
        {
            Contact = new Uri("https://api.freeagent.com/v2/contacts/789"),
            DatedOn = new DateOnly(2024, 1, 10),
            DueOn = new DateOnly(2024, 2, 10),
            Reference = "BILL-USD-001",
            TotalValue = 500.00m,
            Currency = "USD",
            ExchangeRate = 1.27m
        };

        Bill responseBill = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/bills/456"),
            Contact = new Uri("https://api.freeagent.com/v2/contacts/789"),
            Reference = "BILL-USD-001",
            Status = "Open",
            TotalValue = 500.00m,
            Currency = "USD",
            ExchangeRate = 1.27m
        };

        BillRoot responseRoot = new() { Bill = responseBill };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Bill result = await this.bills.CreateAsync(inputBill);

        // Assert
        result.ShouldNotBeNull();
        result.Reference.ShouldBe("BILL-USD-001");
        result.Currency.ShouldBe("USD");
        result.ExchangeRate.ShouldBe(1.27m);
    }

    [TestMethod]
    public async Task CreateAsync_WithRebilling_SerializesRebillingFieldsCorrectly()
    {
        // Arrange
        Uri projectUri = new("https://api.freeagent.com/v2/projects/123");
        Bill inputBill = new()
        {
            Contact = new Uri("https://api.freeagent.com/v2/contacts/789"),
            DatedOn = new DateOnly(2024, 1, 10),
            DueOn = new DateOnly(2024, 2, 10),
            Reference = "BILL-REBILL-001",
            TotalValue = 600.00m,
            Project = projectUri,
            RebillFactor = 1.2m,
            RebillType = "markup"
        };

        Bill responseBill = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/bills/456"),
            Contact = new Uri("https://api.freeagent.com/v2/contacts/789"),
            Reference = "BILL-REBILL-001",
            Status = "Open",
            TotalValue = 600.00m,
            Project = projectUri,
            RebillFactor = 1.2m,
            RebillType = "markup",
            RebillToProject = projectUri
        };

        BillRoot responseRoot = new() { Bill = responseBill };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Bill result = await this.bills.CreateAsync(inputBill);

        // Assert
        result.ShouldNotBeNull();
        result.Reference.ShouldBe("BILL-REBILL-001");
        result.RebillFactor.ShouldBe(1.2m);
        result.RebillType.ShouldBe("markup");
        result.RebillToProject.ShouldBe(projectUri);
    }

    [TestMethod]
    public async Task CreateAsync_WithRecurringBill_SerializesRecurringFieldsCorrectly()
    {
        // Arrange
        Bill inputBill = new()
        {
            Contact = new Uri("https://api.freeagent.com/v2/contacts/789"),
            DatedOn = new DateOnly(2024, 1, 10),
            DueOn = new DateOnly(2024, 2, 10),
            Reference = "BILL-RECURRING-001",
            TotalValue = 600.00m,
            Recurring = "Monthly",
            RecurringEndDate = new DateOnly(2024, 12, 31)
        };

        Bill responseBill = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/bills/456"),
            Contact = new Uri("https://api.freeagent.com/v2/contacts/789"),
            Reference = "BILL-RECURRING-001",
            Status = "Open",
            TotalValue = 600.00m,
            Recurring = "Monthly",
            RecurringEndDate = new DateOnly(2024, 12, 31),
            RecurringBill = new Uri("https://api.freeagent.com/v2/recurring_bills/789")
        };

        BillRoot responseRoot = new() { Bill = responseBill };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Bill result = await this.bills.CreateAsync(inputBill);

        // Assert
        result.ShouldNotBeNull();
        result.Reference.ShouldBe("BILL-RECURRING-001");
        result.Recurring.ShouldBe("Monthly");
        result.RecurringEndDate.ShouldBe(new DateOnly(2024, 12, 31));
        result.RecurringBill.ShouldNotBeNull();
    }

    [TestMethod]
    public async Task GetByIdAsync_WithRebilledOnInvoiceItem_DeserializesCorrectly()
    {
        // Arrange
        Bill bill = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/bills/456"),
            Reference = "BILL-001",
            Status = "Open",
            TotalValue = 600.00m,
            RebilledOnInvoiceItem = new Uri("https://api.freeagent.com/v2/invoice_items/999")
        };

        BillRoot responseRoot = new() { Bill = bill };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Bill result = await this.bills.GetByIdAsync("456");

        // Assert
        result.ShouldNotBeNull();
        result.Reference.ShouldBe("BILL-001");
        result.RebilledOnInvoiceItem.ShouldNotBeNull();
        result.RebilledOnInvoiceItem.ShouldBe(new Uri("https://api.freeagent.com/v2/invoice_items/999"));
    }
}
